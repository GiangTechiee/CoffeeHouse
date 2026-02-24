using CoffeeHouse.Domain.Entities;
using CoffeeHouse.Domain.Specifications;

namespace CoffeeHouse.Application.LegacyAccounts.Specifications;

public class LegacyAccountsCountSpec : BaseSpecification<Account>
{
    public LegacyAccountsCountSpec(string? search)
    {
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            Criteria = a => a.Username.ToLower().Contains(term);
        }
    }
}
