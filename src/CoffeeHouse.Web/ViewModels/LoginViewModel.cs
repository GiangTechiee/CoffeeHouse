using System.ComponentModel.DataAnnotations;

namespace CoffeeHouse.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Tên tài khoản là bắt buộc")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
