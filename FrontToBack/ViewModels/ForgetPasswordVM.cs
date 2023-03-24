using System.ComponentModel.DataAnnotations;

namespace FrontToBack.ViewModels
{
    public class ForgetPasswordVM
    {
        [Required, DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
