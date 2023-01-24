using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Pagination;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Tokens.Domain.Entities;

namespace Tokens.Application.Infrastructure;

public interface ITokenRepository : IRepository<Token, TokenId>
{
    Task<DbPaginationResult<Token>> FindAllWithIds(IEnumerable<TokenId> ids, PaginationFilter paginationFilter);
    Task<DbPaginationResult<Token>> FindAllOfOwner(IdentityAddress owner, PaginationFilter paginationFilter);
    Task<IdentityAddress> GetOwner(TokenId tokenId);
}
