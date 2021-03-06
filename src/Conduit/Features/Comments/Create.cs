using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Conduit.Domain;
using Conduit.Infrastructure;
using Conduit.Infrastructure.Errors;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Conduit.Features.Comments
{
    public class Create
    {
        public class CommentData
        {
            public string Body { get; set; }
        }

        public class CommentDataValidator : AbstractValidator<CommentData>
        {
            public CommentDataValidator()
            {
                RuleFor(x => x.Body).NotNull().NotEmpty();
            }
        }

        public class Command : IRequest<CommentEnvelope>
        {
            public CommentData Comment { get; set; }

            public string Slug { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Comment).NotNull().SetValidator(new CommentDataValidator());
            }
        }

        public class Handler : IRequestHandler<Command, CommentEnvelope>
        {
            private readonly ConduitContext _context;
            private readonly ICurrentUserAccessor _currentUserAccessor;

            public Handler(ConduitContext context, ICurrentUserAccessor currentUserAccessor)
            {
                _context = context;
                _currentUserAccessor = currentUserAccessor;
            }

            public async Task<CommentEnvelope> Handle(Command message, CancellationToken cancellationToken)
            {
                var article = await _context.Articles
                    .Include(x => x.Comments)
                    .FirstOrDefaultAsync(x => x.Slug == message.Slug, cancellationToken);

                if (article == null)
                {
                    throw new RestException(HttpStatusCode.NotFound, new { Article = Constants.NOT_FOUND });
                }

                var author = await _context.Persons.FirstAsync(x => x.Username == _currentUserAccessor.GetCurrentUsername(), cancellationToken);

                var comment = new Comment()
                {
                    Author = author,
                    Body = message.Comment.Body,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                if (ValidateBody(message.Comment.Body))
                {
                    comment.IsReported = true;
                }                

                await _context.Comments.AddAsync(comment, cancellationToken);

                article.Comments.Add(comment);

                await _context.SaveChangesAsync(cancellationToken);

                return new CommentEnvelope(comment);
            }

            private bool ValidateBody(string body)
            {
                var blackWords = new List<string>() { "KLAWIATURA", "PARAPET", "KOMPUTER", "TEST", "MYSZKA", "TABLICA", "EKRAN", "G�UPI", "BRZYDKI", "PISUAR" };

                var textToValidate = body.Replace(",", String.Empty);
                textToValidate = textToValidate.Replace(".", String.Empty);
                var wordsArray = textToValidate.Split(' ');

                foreach (var word in wordsArray)
                {
                    foreach (var blackword in blackWords)
                    {
                        if (word.ToUpper().Contains(blackword))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }
    }
}
