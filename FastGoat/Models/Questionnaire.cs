using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FastGoat.Models
{
    public class Questionnaire
    {
        public IList<QuestionGroup> QuestionGroups
        {
            get;
            set;
        }
    }

    public class QuestionGroup
    {
        public IList<Question> Questions
        {
            get;
            set;
        }
        public String GroupTitle { get; set; }
    }

    public class Question
    {
        public String QuestionText { get; set; }
        public String QuestionAnswer { get; set; }
    }
}
