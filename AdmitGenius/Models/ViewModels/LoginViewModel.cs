using System.ComponentModel.DataAnnotations;

namespace AdmitGenius.Models.ViewModels
{
    public class LoginViewModel
    {

        public string UserName { get; set; }


        [Required(ErrorMessage = "Please enter your email")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string UserEmail { get; set; }

        [Required(ErrorMessage = "Please enter your password")]
        public string UserPassword { get; set; }

        public string SelectedRole { get; set; }

    }
}
