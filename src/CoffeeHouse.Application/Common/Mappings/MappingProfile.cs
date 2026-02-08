using AutoMapper;
using CoffeeHouse.Application.Customers.DTOs;
using CoffeeHouse.Application.Employees.DTOs;
using CoffeeHouse.Application.Orders.DTOs;
using CoffeeHouse.Application.Authentication.DTOs;
using CoffeeHouse.Application.Products.DTOs;
using CoffeeHouse.Application.Categories.DTOs;
using CoffeeHouse.Domain.Entities;


namespace CoffeeHouse.Application.Common.Mappings;

/// <summary>
/// AutoMapper profile for mapping between Domain entities and DTOs
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateProductMaps();
        CreateOrderMaps();
        CreateCustomerMaps();
        CreateEmployeeMaps();
        CreateAccountMaps();
        CreateCategoryMaps();
    }

    /// <summary>
    /// Creates mappings for Product entity and DTOs
    /// </summary>
    private void CreateProductMaps()
    {
        // Entity to DTO
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ProductName))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
            .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

        // DTO to Entity - Note: This creates a new entity, not for updates
        CreateMap<ProductDto, Product>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
            .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes));
    }

    /// <summary>
    /// Creates mappings for Order entity and DTOs
    /// </summary>
    private void CreateOrderMaps()
    {
        // Entity to DTO
        CreateMap<SalesOrder, OrderDto>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderId))
            .ForMember(dest => dest.CoffeeShopId, opt => opt.MapFrom(src => src.StoreId))
            .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate))
            .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EmployeeId))
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
            .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.SalesOrderItems))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

        // SalesOrderItem to OrderItemDto
        CreateMap<SalesOrderItem, OrderItemDto>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.ProductName : string.Empty))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.LineTotal));
    }

    /// <summary>
    /// Creates mappings for Customer entity and DTOs
    /// </summary>
    private void CreateCustomerMaps()
    {
        // Entity to DTO
        CreateMap<Customer, CustomerDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CustomerId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.CustomerName))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

        // DTO to Entity - Note: This creates a new entity, not for updates
        CreateMap<CustomerDto, Customer>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address));
    }

    /// <summary>
    /// Creates mappings for Employee entity and DTOs
    /// </summary>
    private void CreateEmployeeMaps()
    {
        // Entity to DTO
        CreateMap<Employee, EmployeeDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.EmployeeId))
            .ForMember(dest => dest.CoffeeShopId, opt => opt.MapFrom(src => src.StoreId))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
            .ForMember(dest => dest.Position, opt => opt.MapFrom(src => src.Position))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.IdCardNumber, opt => opt.MapFrom(src => src.IdentityCardNumber))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.BaseSalary, opt => opt.MapFrom(src => src.BaseSalary))
            .ForMember(dest => dest.SalaryCoefficient, opt => opt.MapFrom(src => src.SalaryCoefficient))
            .ForMember(dest => dest.TotalSalary, opt => opt.MapFrom(src => src.BaseSalary * src.SalaryCoefficient))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

        // DTO to Entity - Note: This creates a new entity, not for updates
        CreateMap<EmployeeDto, Employee>()
            .ForMember(dest => dest.StoreId, opt => opt.MapFrom(src => src.CoffeeShopId))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.Position, opt => opt.MapFrom(src => src.Position))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.IdentityCardNumber, opt => opt.MapFrom(src => src.IdCardNumber))
            .ForMember(dest => dest.BaseSalary, opt => opt.MapFrom(src => src.BaseSalary))
            .ForMember(dest => dest.SalaryCoefficient, opt => opt.MapFrom(src => src.SalaryCoefficient))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));
    }
    /// <summary>
    /// Creates mappings for Account entity and DTOs
    /// </summary>
    private void CreateAccountMaps()
    {
        CreateMap<Account, AccountInfoDto>()
            .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.AccountId))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.RoleName))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee))
            .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Customer));
    }

    /// <summary>
    /// Creates mappings for ProductCategory entity and DTOs
    /// </summary>
    private void CreateCategoryMaps()
    {
        CreateMap<ProductCategory, CategoryDto>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.CategoryId))
            .ForMember(d => d.Name, o => o.MapFrom(s => s.CategoryName));
    }
}
