using ErrorOr;
using Hcs.Application.Diagnostics;
using Hcs.Contracts.CreateArticle;
using Hcs.Domain.Abstractions;
using Hcs.Domain.Articles;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hcs.Application.Handlers;

internal sealed class UpsertArticleContentHandler : IRequestHandler<
    UpsertArticleContentCommand,
    ErrorOr<Success>>
{
    private readonly ILocaleRepository _localeRepository;
    private readonly IArticleRepository _articleRepository;
    private readonly IUnitOfWorks _unitOfWorks;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<UpsertArticleContentHandler> _logger;

    public UpsertArticleContentHandler(
        ILocaleRepository localeRepository,
        ICabinRepository cabinRepository,
        IArticleRepository articleRepository,
        IUnitOfWorks unitOfWorks,
        TimeProvider timeProvider,
        ILogger<UpsertArticleContentHandler> logger)
    {
        _localeRepository = localeRepository;
        _articleRepository = articleRepository;
        _unitOfWorks = unitOfWorks;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public async Task<ErrorOr<Success>> Handle(
        UpsertArticleContentCommand request,
        CancellationToken cancellationToken)
    {
        var locale = await _localeRepository.Find(request.Locale, cancellationToken);
        if (locale is null)
        {
            return ApplicationErrors.LocaleNotFound;
        }

        var article = await _articleRepository.Get(
            id: ArticleId.From(request.ArticleId),
            ct: cancellationToken);
        if (article is null)
        {
            return ApplicationErrors.ArticleNotFound;
        }

        var errorOrContent = ArticleContent.Create(
            slug: request.Slug,
            title: request.title,
            shortTitle: request.shortTitle,
            description: request.description,
            toc: request.toc,
            content: request.content,
            tags: request.tags);

        if (errorOrContent.IsError)
        {
            return errorOrContent.Errors;
        }

        var errorOrSuccess = article.UpsertContent(
            locale: locale,
            content: errorOrContent.Value,
            now: _timeProvider.GetUtcNow());

        if (errorOrSuccess.IsError)
        {
            return errorOrSuccess.Errors;
        }

        await _unitOfWorks.SaveChanges(cancellationToken);

        _logger.LogInformation("Article content created");

        return Result.Success;
    }
}
