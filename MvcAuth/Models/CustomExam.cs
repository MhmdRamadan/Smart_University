using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcAuth.Models
{
    [MetadataType(typeof(MetaTypeExam))]
    public partial class ExamTbl
    {

    }
    public class MetaTypeExam
    {
        [Key]
        public int Ex_ID { get; set; }
        [Required(ErrorMessage = "please Insert Exam Name")]
        [MinLength(4, ErrorMessage = "Should be more than 4 char")]
        [MaxLength(50, ErrorMessage = "Should be less than 50 char")]
        public string Ex_Name { get; set; }
        [Required(ErrorMessage = "Place Insert Exam Info")]
    
        public string Ex_Info { get; set; }
        [Required(ErrorMessage = "Place Insert Exam Total Degree")]
        [Range(10, 100, ErrorMessage = "Total should Be betwee 10  and 100")]
        [RegularExpression(@"^[0-9]{1,3}$", ErrorMessage = "Number only")]
        public Nullable<int> Ex_Total { get; set; }
        [RegularExpression(@"^[0-9]{1,3}$", ErrorMessage = "Number only")]
        [Required(ErrorMessage = "Place Insert Exam Munite Duration")]
        [Range(5, 60, ErrorMessage = "Total should Be betwee 5 munite  and 60 munite")]
        public Nullable<int> Ex_Munite_Duration { get; set; }
        [Required(ErrorMessage = "*Exam Start On is Requird")]
        public DateTime Ex_Start_On { get; set; }
        [Required(ErrorMessage = "*Exam End On is Requird")]
        public DateTime Ex_End_On { get; set; }
        [RegularExpression(@"^[0-9]{1,3}$", ErrorMessage = "Number only")]
        [Required(ErrorMessage = "Place Insert Exam Passing Degree")]
        public Nullable<int> Ex_Passing_Mark { get; set; }
        [Required(ErrorMessage = "*Course Name is Requird")]
        public Nullable<int> Course_ID { get; set; }
        public Nullable<bool> IsShow { get; set; }
    }
}