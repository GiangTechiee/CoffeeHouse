using CoffeeHouse.Application.Products.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CoffeeHouse.Application.Products.Commands.UploadProductImage;

public record UploadProductImageCommand(int ProductId, IFormFile ImageFile) : IRequest<ProductImageDto>;
