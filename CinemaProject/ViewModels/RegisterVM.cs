using System.ComponentModel.DataAnnotations;

namespace CinemaProject.ViewModels
{
    public class RegisterVM
    {
        public int Id { get; set; }
        [Required]
        public String Name { get; set; } = string.Empty;

        [Required]
        public String UserName { get; set; } = string.Empty;

        [Required , DataType(DataType.EmailAddress)]
        public String Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
       public String Password { get; set; } = string.Empty;

        [Required, DataType(DataType.Password),Compare(nameof(Password))]
        public String ConfirmPassword { get; set; } = string.Empty;
        public String? Address { get; set; }
    }
}
