using CoffeeHouse.Application.Products.Commands.CreateProduct;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace CoffeeHouse.Application.Tests.Products.Commands;

/// <summary>
/// Tests for CreateProductCommandValidator
/// Validates: Requirements 13.2 - Validation rules and error messages
/// </summary>
public class CreateProductCommandValidatorTests
{
    private readonly CreateProductCommandValidator _validator;

    public CreateProductCommandValidatorTests()
    {
        _validator = new CreateProductCommandValidator();
    }

    [Fact]
    public void Validate_WithValidCommand_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Name = "Cappuccino",
            Price = 50000m,
            CategoryId = 1,
            Description = "Delicious coffee",
            ImageUrl = "/images/cappuccino.jpg",
            Notes = "Best seller"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithEmptyName_ShouldHaveValidationError(string emptyName)
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Name = emptyName,
            Price = 50000m,
            CategoryId = 1
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Name)
            .WithErrorMessage("Product name is required");
    }

    [Fact]
    public void Validate_WithNameTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Name = new string('a', 256),
            Price = 50000m,
            CategoryId = 1
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Name)
            .WithErrorMessage("Product name must not exceed 255 characters");
    }

    [Fact]
    public void Validate_WithNameExactly255Characters_ShouldNotHaveValidationError()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Name = new string('a', 255),
            Price = 50000m,
            CategoryId = 1
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(c => c.Name);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Validate_WithInvalidPrice_ShouldHaveValidationError(decimal invalidPrice)
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Name = "Cappuccino",
            Price = invalidPrice,
            CategoryId = 1
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Price)
            .WithErrorMessage("Price must be greater than 0");
    }

    [Fact]
    public void Validate_WithPriceTooHigh_ShouldHaveValidationError()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Name = "Cappuccino",
            Price = 1000000000m,
            CategoryId = 1
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Price)
            .WithErrorMessage("Price must not exceed 999,999,999");
    }

    [Fact]
    public void Validate_WithPriceExactlyAtMaximum_ShouldNotHaveValidationError()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Name = "Cappuccino",
            Price = 999999999m,
            CategoryId = 1
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(c => c.Price);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WithInvalidCategoryId_ShouldHaveValidationError(int invalidCategoryId)
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Name = "Cappuccino",
            Price = 50000m,
            CategoryId = invalidCategoryId
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.CategoryId)
            .WithErrorMessage("Category is required");
    }

    [Fact]
    public void Validate_WithDescriptionTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Name = "Cappuccino",
            Price = 50000m,
            CategoryId = 1,
            Description = new string('a', 1001)
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Description)
            .WithErrorMessage("Description must not exceed 1000 characters");
    }

    [Fact]
    public void Validate_WithDescriptionExactly1000Characters_ShouldNotHaveValidationError()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Name = "Cappuccino",
            Price = 50000m,
            CategoryId = 1,
            Description = new string('a', 1000)
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(c => c.Description);
    }

    [Fact]
    public void Validate_WithNullDescription_ShouldNotHaveValidationError()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Name = "Cappuccino",
            Price = 50000m,
            CategoryId = 1,
            Description = null
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(c => c.Description);
    }

    [Fact]
    public void Validate_WithImageUrlTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Name = "Cappuccino",
            Price = 50000m,
            CategoryId = 1,
            ImageUrl = new string('a', 501)
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.ImageUrl)
            .WithErrorMessage("Image URL must not exceed 500 characters");
    }

    [Fact]
    public void Validate_WithImageUrlExactly500Characters_ShouldNotHaveValidationError()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Name = "Cappuccino",
            Price = 50000m,
            CategoryId = 1,
            ImageUrl = new string('a', 500)
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(c => c.ImageUrl);
    }

    [Fact]
    public void Validate_WithNullImageUrl_ShouldNotHaveValidationError()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Name = "Cappuccino",
            Price = 50000m,
            CategoryId = 1,
            ImageUrl = null
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(c => c.ImageUrl);
    }

    [Fact]
    public void Validate_WithNotesTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Name = "Cappuccino",
            Price = 50000m,
            CategoryId = 1,
            Notes = new string('a', 501)
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Notes)
            .WithErrorMessage("Notes must not exceed 500 characters");
    }

    [Fact]
    public void Validate_WithNotesExactly500Characters_ShouldNotHaveValidationError()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Name = "Cappuccino",
            Price = 50000m,
            CategoryId = 1,
            Notes = new string('a', 500)
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(c => c.Notes);
    }

    [Fact]
    public void Validate_WithNullNotes_ShouldNotHaveValidationError()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Name = "Cappuccino",
            Price = 50000m,
            CategoryId = 1,
            Notes = null
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(c => c.Notes);
    }

    [Fact]
    public void Validate_WithMultipleErrors_ShouldReturnAllErrors()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Name = "",
            Price = -10m,
            CategoryId = 0
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Name);
        result.ShouldHaveValidationErrorFor(c => c.Price);
        result.ShouldHaveValidationErrorFor(c => c.CategoryId);
    }

    [Fact]
    public void Validate_WithMinimalValidData_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Name = "Espresso",
            Price = 0.01m,
            CategoryId = 1
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
