using System.ComponentModel.DataAnnotations;

namespace FrontToBack.ViewModels
{
    public class SliderCreateVM
    {
        [Required(ErrorMessage="Can't be empty")]
        public IFormFile Photo { get; set; }
    }
}
