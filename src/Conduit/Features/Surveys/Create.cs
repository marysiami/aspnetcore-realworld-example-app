using Conduit.Domain;
using Conduit.Infrastructure;
using Conduit.Infrastructure.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Conduit.Features.Surveys
{
    public class Create
    {
        public class SurveyData
        {
            public Person Author { get; set; }

            public string Title { get; set; }

            public List<AnswerAndQuestion> AnswersAndQuestions { get; set; }

            public Person FillingUser { get; set; }
        }

        public class Command : IRequest<SurveysEnvelope>
        {
            public SurveyData Survey { get; set; }
        }

        public class Handler : IRequestHandler<Command, SurveysEnvelope>
        {
            private readonly ConduitContext _context;
            private readonly ICurrentUserAccessor _currentUserAccessor;

            public Handler(ConduitContext context, ICurrentUserAccessor currentUserAccessor)
            {
                _context = context;
                _currentUserAccessor = currentUserAccessor;
            }

            public async Task<SurveysEnvelope> Handle(Command message, CancellationToken cancellationToken)
            {
                var surveyExist = await _context.Surveys
                    .Where(x => x.Author.Email == message.Survey.Author.Email && x.Title == message.Survey.Title)
                    .FirstOrDefaultAsync();

                if (surveyExist != null)
                {
                    throw new RestException(System.Net.HttpStatusCode.Found, new { Error = "Survey exist." });
                }

                foreach (var answerAndQuestion in message.Survey.AnswersAndQuestions)
                {
                    await _context.AnswersAndQuestions.AddAsync(answerAndQuestion);
                }

                var survey = new Survey()
                {
                    Author = message.Survey.Author,
                    Title = message.Survey.Title,
                    FillingUser = message.Survey.FillingUser,
                    AnswersAndQuestions = message.Survey.AnswersAndQuestions
                };

                await _context.Surveys.AddAsync(survey, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                return new SurveysEnvelope(survey);
            }
        }
    }
}
