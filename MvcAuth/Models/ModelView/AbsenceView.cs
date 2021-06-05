using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcAuth.Models.ModelView
{
    public class AbsenceView
    {
        [Required]
        public byte[] Image { get; set; }
    }
}