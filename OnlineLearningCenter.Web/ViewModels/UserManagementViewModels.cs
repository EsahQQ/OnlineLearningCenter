using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineLearningCenter.Web.ViewModels
{
    public class RoleViewModel
    {
        public string RoleName { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }
    public class ManageUserRolesViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public List<RoleViewModel> Roles { get; set; } = new();
    }

    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Пароль обязателен")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}