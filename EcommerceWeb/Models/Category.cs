using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EcommerceWeb.Models
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }
        [Required]
        [MaxLength(30)]
        [DisplayName("Category Name")]
        public string Name { get; set; }
        [DisplayName("Display Order")]
        [Range(1,100, ErrorMessage ="Enter number between 0-100") ]
        public int DisplayOrder { get; set; }

    }
}
