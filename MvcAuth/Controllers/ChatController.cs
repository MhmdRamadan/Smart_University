using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcAuth.Models;

namespace MvcAuth.Controllers
{
    public class ChatController : Controller
    {
        // GET: Chat
        UniversityDBEntities1 Db = new UniversityDBEntities1();
        public ActionResult Index()
        {
            if (Session["StudentName"] != null)
            {
                Session["name"] = Session["StudentName"];
                Session["type"] = "std";
               
                Session["ProfessorName"] = "";
            }
            else 
            {
                Session["name"] = Session["ProfessorName"];
              
                    Session["StudentName"] = "";
               

                Session["type"] = "prof";
            }
            return View();
        }
    }
}