using CoffeeHouse.Application.Common.Models;
using MediatR;

namespace CoffeeHouse.Application.Employees.Commands.DeleteEmployee;

/// <summary>
/// Command to delete an employee
/// </summary>
public record DeleteEmployeeCommand(int Id) : IRequest<Result>;
