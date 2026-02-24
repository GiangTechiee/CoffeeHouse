using CoffeeHouse.Domain.Entities;
using CoffeeHouse.Domain.Specifications;

namespace CoffeeHouse.Application.LegacyAccounts.Specifications;

public class LegacyAccountsSpec : BaseSpecification<Account>
{
    public LegacyAccountsSpec(string? search, int skip, int take)
    {
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            Criteria = a => a.Username.ToLower().Contains(term);
        }

        AddInclude(a => a.Role);
        AddInclude(a => a.Employee);
        AddInclude(a => a.Customer);
        ApplyOrderBy(a => a.AccountId);
        ApplyPaging(skip, take);
    }
}
