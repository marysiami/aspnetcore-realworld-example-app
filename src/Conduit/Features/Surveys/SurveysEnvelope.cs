using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Features.Surveys
{
    public class SurveysEnvelope
    {
        public Conduit.Domain.Survey Survey { get; set; }

        public List<Conduit.Domain.Survey> Surveys { get; set; }

        public int SurveysCount { get; set; }

        public SurveysEnvelope()
        {

        }

        public SurveysEnvelope(Conduit.Domain.Survey survey)
        {
            Survey = survey;
        }
    }
}
