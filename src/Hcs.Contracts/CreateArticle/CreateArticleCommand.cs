using ErrorOr;
using MediatR;

namespace Hcs.Contracts.CreateArticle;

public record CreateArticleCommand(
    Guid CabinId,
    string OriginalLocale,
    bool MultiLangSupport,
    bool DraftSupport) : IRequest<ErrorOr<Guid>>;
