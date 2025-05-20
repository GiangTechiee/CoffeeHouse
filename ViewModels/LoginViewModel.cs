using System.ComponentModel.DataAnnotations;

namespace CoffeeHouse.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Tên tài khoản là bắt buộc")]
        public string TenTaiKhoan { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; }
    }
}