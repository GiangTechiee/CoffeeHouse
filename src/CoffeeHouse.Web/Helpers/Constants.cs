namespace CoffeeHouse.Helpers
{
    /// <summary>
    /// Contains application-wide constants to avoid magic strings and numbers
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// User role constants
        /// </summary>
        public static class Roles
        {
            public const string Admin = "Admin";
            public const string Employee = "Employee";
            public const string User = "User";
        }

        /// <summary>
        /// Session key constants
        /// </summary>
        public static class SessionKeys
        {
            public const string Username = "Username";
            public const string Role = "Role";
            public const string CustomerId = "CustomerId";
            public const string EmployeeId = "EmployeeId";
        }

        /// <summary>
        /// Status constants
        /// </summary>
        public static class Status
        {
            public const string Active = "Active";
            public const string Inactive = "Inactive";
            public const string Pending = "Pending";
            public const string Completed = "Completed";
            public const string Cancelled = "Cancelled";
        }

        /// <summary>
        /// Error message constants
        /// </summary>
        public static class ErrorMessages
        {
            public const string Unauthorized = "Bạn không có quyền truy cập trang này.";
            public const string NotAuthenticated = "Vui lòng đăng nhập để tiếp tục.";
            public const string InvalidCredentials = "Tên đăng nhập hoặc mật khẩu không đúng.";
            public const string UserNotFound = "Không tìm thấy người dùng.";
            public const string ProductNotFound = "Không tìm thấy sản phẩm.";
            public const string OrderNotFound = "Không tìm thấy đơn hàng.";
            public const string InvalidInput = "Dữ liệu đầu vào không hợp lệ.";
            public const string DatabaseError = "Đã xảy ra lỗi khi truy cập cơ sở dữ liệu.";
            public const string UnexpectedError = "Đã xảy ra lỗi không mong muốn.";
        }

        /// <summary>
        /// Success message constants
        /// </summary>
        public static class SuccessMessages
        {
            public const string LoginSuccess = "Đăng nhập thành công.";
            public const string LogoutSuccess = "Đăng xuất thành công.";
            public const string RegisterSuccess = "Đăng ký tài khoản thành công.";
            public const string ProductCreated = "Thêm sản phẩm thành công.";
            public const string ProductUpdated = "Cập nhật sản phẩm thành công.";
            public const string ProductDeleted = "Xóa sản phẩm thành công.";
            public const string OrderCreated = "Đặt hàng thành công.";
            public const string OrderUpdated = "Cập nhật đơn hàng thành công.";
            public const string OrderCancelled = "Hủy đơn hàng thành công.";
        }

        /// <summary>
        /// Pagination constants
        /// </summary>
        public static class Pagination
        {
            public const int DefaultPageSize = 30;
            public const int DefaultPageNumber = 1;
            public const int MaxPageSize = 100;
        }

        /// <summary>
        /// Currency constants
        /// </summary>
        public static class Currency
        {
            public const string VND = "VND";
            public const string USD = "USD";
        }

        /// <summary>
        /// Area names
        /// </summary>
        public static class Areas
        {
            public const string Admin = "Admin";
        }

        /// <summary>
        /// Permission/Role IDs
        /// </summary>
        public static class RoleIds
        {
            public const int Admin = 1;
            public const int User = 2;
            public const int Employee = 3;
        }
    }
}
