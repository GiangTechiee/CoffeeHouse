using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Application.Roles.DTOs;
using CoffeeHouse.Application.Authentication.DTOs;
using CoffeeHouse.Controllers.API.Admin;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace CoffeeHouse.Integration.Tests
{
    public class AdminSecuritySeedingTests : BaseIntegrationTest
    {
        public AdminSecuritySeedingTests(WebApplicationFactory<Program> factory) : base(factory)
        {
        }

        [Fact]
        public async Task SeedRolesAndAccounts()
        {
            await EnsureAuthenticatedAsync();

            // 1. Seed Roles (Identity + Legacy)
            await SeedRoles();

            // 2. Seed Legacy Accounts (connecting to seeded employees/customers)
            await SeedLegacyAccounts();
        }

        private async Task SeedRoles()
        {
            var rolesToSeed = new List<LegacyRoleRequest>
            {
                new() { Name = "StoreManager", Description = "Quản lý cửa hàng" },
                new() { Name = "Barista", Description = "Nhân viên pha chế" },
                new() { Name = "Waiter", Description = "Nhân viên phục vụ" },
                new() { Name = "Employee", Description = "Nhân viên" },
                new() { Name = "User", Description = "Khách hàng" }
            };

            var existing = await GetAsync<List<CoffeeHouse.Application.LegacyRoles.DTOs.LegacyRoleDto>>("/api/v1/admin/legacy-roles");
            var existingNames = existing?.Select(r => r.RoleName).ToHashSet() ?? new HashSet<string>();

            foreach (var role in rolesToSeed)
            {
                if (!existingNames.Contains(role.Name))
                {
                    await PostAsync<LegacyRoleRequest, CoffeeHouse.Application.LegacyRoles.DTOs.LegacyRoleDto>("/api/v1/admin/legacy-roles", role);
                }
            }
        }

        private async Task SeedLegacyAccounts()
        {
            // Fetch employees and customers to link accounts
            var emps = await GetPaginatedAsync<CoffeeHouse.Application.Employees.DTOs.EmployeeDto>("/api/v1/admin/employees");
            var custs = await GetPaginatedAsync<CoffeeHouse.Application.Customers.DTOs.CustomerDto>("/api/v1/admin/customers");
            var roles = await GetAsync<List<CoffeeHouse.Application.LegacyRoles.DTOs.LegacyRoleDto>>("/api/v1/admin/legacy-roles");

            if (emps == null || custs == null || roles == null || emps.Items.Count == 0 || custs.Items.Count == 0) return;

            var staffRole = roles.FirstOrDefault(r => r.RoleName == "Employee")?.RoleId ?? roles.First().RoleId;
            var userRole = roles.FirstOrDefault(r => r.RoleName == "User")?.RoleId ?? roles.First().RoleId;

            var accountsToSeed = new List<CreateLegacyAccountRequest>
            {
                new() { Username = "nhanvien_test", Password = "Password123!", RoleId = staffRole, EmployeeId = emps.Items.First().Id, Status = "Active" },
                new() { Username = "khachhang_test", Password = "Password123!", RoleId = userRole, CustomerId = custs.Items.First().Id, Status = "Active" }
            };

            var existing = await GetPaginatedAsync<AccountInfoDto>("/api/v1/admin/legacy-accounts");
            var existingNames = existing?.Items.Select(a => a.Username).ToHashSet() ?? new HashSet<string>();

            foreach (var acc in accountsToSeed)
            {
                if (!existingNames.Contains(acc.Username))
                {
                    await PostAsync<CreateLegacyAccountRequest, AccountInfoDto>("/api/v1/admin/legacy-accounts", acc);
                }
            }
        }
    }
}
