using System;
using System.Collections.Generic;
using System.Linq;
using MvcAuth.Models;
using System.Web;

namespace MvcAuth.Models
{
    public class Data
    {
        public List<EventTbl> Events { get; set; }
        public List<NewsTbl> News { get; set; }
    }
}