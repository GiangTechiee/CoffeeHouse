using CoffeeHouse.Domain.Entities;
using CoffeeHouse.Domain.Specifications;

namespace CoffeeHouse.Application.LegacyAccounts.Specifications;

public class LegacyAccountByIdSpec : BaseSpecification<Account>
{
    public LegacyAccountByIdSpec(int accountId)
        : base(a => a.AccountId == accountId)
    {
        AddInclude(a => a.Role);
        AddInclude(a => a.Employee);
        AddInclude(a => a.Customer);
    }
}
