using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace G24.Models
{
    public class Images
    {


        [Display(Name = "Image ID")]
        public int ImgID { get; set; }

        [Required]
        [Display(Name = "Image URL")]
        public string ImgURL { get; set; }

        [Required]
        [Display(Name = "Type")]
        public string Type { get; set; }

        [Required]
        [Display(Name = "Image Name")]
        public string ImgName { get; set; }


        [Display(Name = "User ID")]
        public int UserID { get; set; }

    }
}
