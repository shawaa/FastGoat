using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FastGoat.Models;
using Dapper;
using Dapper.Contrib.Extensions;

namespace FastGoat.Controllers
{
    public class QuestionController : Controller
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public QuestionController(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        [HttpGet("Questions", Name = "questions")]
        public async Task<IActionResult> Questions()
        {
            System.Data.IDbConnection dbConnection = _connectionFactory.Create();
            dbConnection.Open();

            IEnumerable<DbQuestion> result = await dbConnection.QueryAsync<DbQuestion>("SELECT * FROM dbo.Question WHERE Id = @id", 
                new { id = 1 } );

            IEnumerable<Question> QuestionList1 = result.Select(x => new Question
            {
                QuestionText = x.Text,
                QuestionAnswer = null
            });
            
            List<Question> QuestionList2 = new List<Question>
                            {
                                new Question{ QuestionText = "QG2 Q1"},
                                new Question{ QuestionText = "QG2 Q2"},
                                new Question{ QuestionText = "QG2 Q3"},
                                new Question{ QuestionText = "QG2 Q4"}
                            };
            List<Question> QuestionList3 = new List<Question>
                            {
                                new Question{ QuestionText = "QG3 Q1"},
                                new Question{ QuestionText = "QG3 Q2"},
                                new Question{ QuestionText = "QG3 Q3"}
                            };

            List<QuestionGroup> Group1 = new List<QuestionGroup>
                    {
                        new QuestionGroup
                        {
                            Questions = QuestionList1.ToList(),
                            GroupTitle = "Lashunta"
                        },
                        new QuestionGroup
                        {
                            Questions = QuestionList2,
                            GroupTitle = "Shirren"
                        },new QuestionGroup
                        {
                            Questions = QuestionList3,
                            GroupTitle = "Vesk"
                        }
                    };
            Questionnaire questionnaire = new Questionnaire
            {
                QuestionGroups = Group1
            };

            return View(questionnaire); //View(questionnaire);
        }

        [HttpPost("Questions")]
        public async Task<IActionResult> Questions(Questionnaire questionnaire)
        {
            return Redirect("../Home");
        }
    }

    public class DbQuestion
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public int Questionnaire { get; set; }
    }
}
