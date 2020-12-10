using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Conduit.Domain
{
    public class AnswerAndQuestion
    {
        [JsonIgnore]
        public int AnswerAndQuestionId { get; set; }

        public string Question { get; set; }

        public string Answer { get; set; }

        [JsonIgnore]
        public int SurveyId { get; set; }
    }
}
