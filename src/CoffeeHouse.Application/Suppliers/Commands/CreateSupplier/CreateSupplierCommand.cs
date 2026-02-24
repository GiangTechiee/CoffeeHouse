using CoffeeHouse.Application.Suppliers.DTOs;
using MediatR;

namespace CoffeeHouse.Application.Suppliers.Commands.CreateSupplier;

public record CreateSupplierCommand(
    string SupplierName,
    string PhoneNumber,
    string? Address,
    string? Stk,
    string? Status) : IRequest<SupplierDto>;
