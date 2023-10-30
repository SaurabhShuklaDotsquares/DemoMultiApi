using System.ComponentModel.DataAnnotations;

namespace EC.API.ViewModels
{
    public class RegisterViewModels
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public byte Gender { get; set; }
        public string Token { get; set; }
        public string Name { get; set; }
    }
}
