using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Conduit.Domain
{
    public class Survey
    {
        [JsonIgnore]
        public int SurveyId { get; set; }

        public Person Author { get; set; }

        public string Title { get; set; }

        public List<AnswerAndQuestion> AnswersAndQuestions { get; set; }

        //[NotMapped]
        //public List<string> questions { get; set; }

        //[NotMapped]
        //public List<string> answers { get; set; }

        public Person FillingUser { get; set; }
    }
}
