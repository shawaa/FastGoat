using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;

namespace FastGoat
{
    public class Startup
    {
        public Startup(IHostingEnvironment hostingEnvironment)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(hostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<Connections>(option => Configuration.GetSection("Connections").Bind(option));
            services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
            services
                .AddMvc()
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseMiddleware<TransactionMiddlware>();
            app.UseMiddleware<DbUpgradeMiddleware>();
            app.UseMvc();
        }
    }

    public class TransactionMiddlware : IMiddleware
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            using (var tran = _connectionFactory.Create().BeginTransaction())
            {
                await next.Invoke(context);
                tran.Commit();
            }
        }
    }

    public class DbUpgradeMiddleware : IMiddleware
    {
        private readonly IDbConnectionFactory _connectionFactory;

        private static readonly IDictionary<string, string> _upgradeScripts = new Dictionary<string, string>
        {
            { "2018-05-19", Queries.CreateInitialTables }
        };

        private static object _bolt;

        public DbUpgradeMiddleware(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            IDbConnection connection = _connectionFactory.Create();
            lock (_bolt)
            {
                using (var tran = connection.BeginTransaction())
                {
                    var result = connection.Query<Script>("select * from dbo.upgrade");

                    var scriptsToRun = _upgradeScripts.Where(runnableScript => !result.Any(runScript => runScript.Key != runnableScript.Key)).Select(x => x.Value);
                    foreach (string script in scriptsToRun)
                    {
                        connection.Execute(script);
                    }
                }
            }
            await next.Invoke(context);
        }

        private class Script
        {
            public string Key { get; set; }

            public string Value { get; set; }
        }

    }


    public class Connections
    {
        public string Database { get; set; }
    }

    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly Connections _connections;

        public DbConnectionFactory(IOptions<Connections> configuration)
        {
            _connections = configuration.Value;
        }

        public IDbConnection Create()
        {
            return new SqlConnection(_connections.Database);
        }
    }
}
