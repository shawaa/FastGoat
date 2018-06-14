using System.Data;

namespace FastGoat
{
    public interface IDbConnectionFactory
    {
        IDbConnection Create();
    }
}