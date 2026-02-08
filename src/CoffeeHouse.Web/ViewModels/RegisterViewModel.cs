using System.ComponentModel.DataAnnotations;
using CoffeeHouse.Application.Common.Attributes;

namespace CoffeeHouse.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Tên tài khoản là bắt buộc")]
    [Sanitized]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên tài khoản phải từ 3 đến 50 ký tự")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 đến 100 ký tự")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tên khách hàng là bắt buộc")]
    [Sanitized]
    [StringLength(100, ErrorMessage = "Tên khách hàng không được quá 100 ký tự")]
    public string CustomerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    [StringLength(20, ErrorMessage = "Số điện thoại không được quá 20 ký tự")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
    [Sanitized]
    [StringLength(255, ErrorMessage = "Địa chỉ không được quá 255 ký tự")]
    public string Address { get; set; } = string.Empty;
}

