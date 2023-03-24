using System.ComponentModel;

namespace FrontToBack.ViewModels
{
    public class SliderUpdateVM
    {
        public string ImageUrl { get; set; }
        [DisplayName("lorem")]
        public IFormFile Photo { get; set; }

    }
}
