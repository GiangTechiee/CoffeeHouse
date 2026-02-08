using CoffeeHouse.Domain.Entities;
using CoffeeHouse.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoffeeHouse.Infrastructure.Persistence.Repositories;

public class CafeStoreRepository : Repository<CafeStore>, ICafeStoreRepository
{
    public CafeStoreRepository(DbContext context) : base(context)
    {
    }
}
