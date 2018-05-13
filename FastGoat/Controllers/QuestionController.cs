using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FastGoat.Models;

namespace FastGoat.Controllers
{
    public class QuestionController : Controller
    {
        [HttpGet]
        public IActionResult Questions()
        {
            List<Question> QuestionList1 = new List<Question>
                            {
                                new Question{ QuestionText = "QG1 Q1"},
                                new Question{ QuestionText = "QG1 Q2"}
                            };
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
                            Questions = QuestionList1,
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
            return View(questionnaire);
        }
        [HttpPost]
        public IActionResult Questions(Questionnaire questionnaire)
        {
            return Redirect("../Home");
        }
    }
}
