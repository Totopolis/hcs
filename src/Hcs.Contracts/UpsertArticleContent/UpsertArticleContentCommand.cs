using ErrorOr;
using MediatR;

namespace Hcs.Contracts.CreateArticle;

public record UpsertArticleContentCommand(
    Guid ArticleId,
    string Locale,
    string Slug,
    string title,
    string shortTitle,
    string description,
    string toc,
    string content,
    IReadOnlyList<string> tags) : IRequest<ErrorOr<Success>>;
