using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcAuth.Models
{
    [MetadataType(typeof(MetaTypeQuestion))]
    public partial class QuestionsTbl
    {

    }
    public class MetaTypeQuestion
    {
        [Required(ErrorMessage = "Select Exam Name")]
        public int Ex_ID { get; set; }

        [Required(ErrorMessage = "Place Insert Question")]
        [MinLength(4, ErrorMessage = "Should be more than 4 char")]
        [MaxLength(100, ErrorMessage = "Should be less than 100 char")]
        public string Question { get; set; }

        [Required(ErrorMessage = "Place Insert First Choice")]
        [MinLength(1, ErrorMessage = "Should be more than 1 char")]
        [MaxLength(100, ErrorMessage = "Should be less than 100 char")]
        public string First_Choice { get; set; }
        [Required(ErrorMessage = "Place Insert Second Choice")]
        [MinLength(1, ErrorMessage = "Should be more than 1 char")]
        [MaxLength(100, ErrorMessage = "Should be less than 100 char")]
        public string Second_Choice { get; set; }
        [Required(ErrorMessage = "Place Insert Third Choice")]
        [MinLength(1, ErrorMessage = "Should be more than 1 char")]
        [MaxLength(100, ErrorMessage = "Should be less than 100 char")]
        public string Third_Choice { get; set; }
        [Required(ErrorMessage = "Place Select Correct Choice")]
        public string Correct_Choice { get; set; }
        [Required(ErrorMessage = "Place Insert Fourth Choice")]
        [MinLength(1, ErrorMessage = "Should be more than 1 char")]
        [MaxLength(100, ErrorMessage = "Should be less than 100 char")]
        public string Fourth_Choice { get; set; }
        [RegularExpression(@"^[0-9]{1,2}$", ErrorMessage = "Number only")]
        [Required(ErrorMessage = "Place Insert Question Degree")]
        public int Degree { get; set; }
    }
}