using Conduit.Infrastructure;
using Conduit.Infrastructure.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Conduit.Features.Surveys
{
    public class Details
    {
        public class Query : IRequest<SurveysEnvelope>
        {
            public Query(int id)
            {
                Id = id;
            }

            public int Id { get; set; }
        }

        public class QueryHandler : IRequestHandler<Query, SurveysEnvelope>
        {
            private readonly ConduitContext _context;

            public QueryHandler(ConduitContext context)
            {
                _context = context;
            }

            public async Task<SurveysEnvelope> Handle(Query message, CancellationToken cancellationToken)
            {
                var survey = await _context.Surveys
                    .Where(x => x.SurveyId == message.Id)
                    .Include(x => x.AnswersAndQuestions)
                    .Include(x => x.Author)
                    .Include(x => x.FillingUser)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (survey == null)
                {
                    throw new RestException(HttpStatusCode.NotFound, new { Article = Constants.NOT_FOUND });
                }

                return new SurveysEnvelope(survey);
            }
        }
    }
}
