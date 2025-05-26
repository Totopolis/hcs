using ErrorOr;
using Hcs.Application.Diagnostics;
using Hcs.Contracts.CreateArticle;
using Hcs.Domain.Abstractions;
using Hcs.Domain.Articles;
using Hcs.Domain.Cabins;
using Hcs.Domain.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hcs.Application.Handlers;

internal sealed class CreateArticleHandler : IRequestHandler<
    CreateArticleCommand,
    ErrorOr<Guid>>
{
    private readonly ILocaleRepository _localeRepository;
    private readonly ICabinRepository _cabinRepository;
    private readonly IArticleRepository _articleRepository;
    private readonly IUnitOfWorks _unitOfWorks;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<CreateArticleHandler> _logger;

    public CreateArticleHandler(
        ILocaleRepository localeRepository,
        ICabinRepository cabinRepository,
        IArticleRepository articleRepository,
        IUnitOfWorks unitOfWorks,
        TimeProvider timeProvider,
        ILogger<CreateArticleHandler> logger)
    {
        _localeRepository = localeRepository;
        _cabinRepository = cabinRepository;
        _articleRepository = articleRepository;
        _unitOfWorks = unitOfWorks;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public async Task<ErrorOr<Guid>> Handle(
        CreateArticleCommand request,
        CancellationToken cancellationToken)
    {
        var locale = await _localeRepository.Find(request.OriginalLocale, cancellationToken);
        if (locale is null)
        {
            return DomainErrors.BadLocale;
        }

        var cabin = await _cabinRepository.Get(
            id: CabinId.From(request.CabinId),
            ct: cancellationToken);
        if (cabin is null)
        {
            return ApplicationErrors.CabinNotFound;
        }

        var article = Article.Create(
            cabin: cabin,
            code: AggregateCode.From("todo123"),
            originalLocale: locale,
            multiLangSupport: request.MultiLangSupport,
            draftSupport: request.DraftSupport,
            now: _timeProvider.GetUtcNow());

        _articleRepository.Add(article);
        await _unitOfWorks.SaveChanges(cancellationToken);

        _logger.LogInformation("Article created");

        return article.Id.Value;
    }
}
