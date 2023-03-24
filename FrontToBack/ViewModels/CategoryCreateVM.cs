using System.ComponentModel.DataAnnotations;

namespace FrontToBack.ViewModels
{
    public class CategoryCreateVM
    {
        [Required, MinLength(3)]
        public string Name { get; set; }
        [Required, MinLength(3)]
        public string Description { get; set; }
    }
}
