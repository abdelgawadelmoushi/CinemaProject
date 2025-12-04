using System.ComponentModel.DataAnnotations;

namespace CinemaProject.ViewModels
{
    public class ResetPasswordVM
    {
        public int Id { get; set; }

        [Required, DataType(DataType.Password)]
        public String Password { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), Compare(nameof(Password))]
        public String ConfirmPassword { get; set; } = string.Empty;
        public string UserId { get; set; }
    }
}
