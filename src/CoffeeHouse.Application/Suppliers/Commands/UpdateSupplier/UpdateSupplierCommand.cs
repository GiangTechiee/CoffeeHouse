using CoffeeHouse.Application.Suppliers.DTOs;
using MediatR;

namespace CoffeeHouse.Application.Suppliers.Commands.UpdateSupplier;

public record UpdateSupplierCommand(
    int SupplierId,
    string SupplierName,
    string PhoneNumber,
    string? Address,
    string? Stk,
    string? Status) : IRequest<SupplierDto>;
