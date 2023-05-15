using System.ComponentModel.DataAnnotations;

namespace FriendifyMain.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required] // Indicate that this property is required
        [DataType(DataType.Password)] // Indicate that this property is a password
        [Display(Name = "Current password")] // Specify the display name for this property
        public string OldPassword { get; set; } // Declare a property for the old password

        [Required] // Indicate that this property is required
        [DataType(DataType.Password)] // Indicate that this property is a password
        [Display(Name = "New password")] // Specify the display name for this property
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)] // Specify the length constraints and error message for this property
        public string NewPassword { get; set; } // Declare a property for the new password

        [Required] // Indicate that this property is required
        [DataType(DataType.Password)] // Indicate that this property is a password
        [Display(Name = "Confirm new password")] // Specify the display name for this property
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")] // Specify the comparison rule and error message for this property
        public string ConfirmPassword { get; set; } // Declare a property for the confirmation of the new password
    }
}
