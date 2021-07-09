using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using MvcAuth.Models;
namespace MvcAuth.Controllers
{
    public class HomeController : Controller
    {
        UniversityDBEntities1 Db = new UniversityDBEntities1();
        public ActionResult Index()
        {
            List<EventTbl> _Events = Db.EventTbls.OrderByDescending(a => a.Event_ID).Take(3).ToList();
            List<NewsTbl> _News = Db.NewsTbls.OrderByDescending(a => a.News_ID).Take(3).ToList(); ;
            Data data = new Data();
            data.News = _News;
            data.Events = _Events;
            return View(data);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendMessage(string email, string msg, string name)
        {
            string message = $" <h3> name : {name} </h3> <br> email {email}<br>, <h4> messge is : </h4>  <br>" +
                           $"<h5>{msg}</h5>";
            var mail = new MailMessage();
            var loginInfo = new NetworkCredential("universityonline", "#@!321Eid");
            mail.From = new MailAddress("universityonline@zohomail.com");
            mail.To.Add(new MailAddress("universityonline@zohomail.com"));
            mail.IsBodyHtml = true;
            mail.Subject = "Message From User in Smart University";
            mail.Body = message;
            var smptClient = new SmtpClient("smtp.zoho.com", 587);
            smptClient.EnableSsl = true;
            smptClient.UseDefaultCredentials = false;
            smptClient.Credentials = loginInfo;
            smptClient.Send(mail);
            return RedirectToAction("Index");
        }
        public ActionResult EDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                var event_ = Db.EventTbls.SingleOrDefault(a => a.Event_ID == id);
                return View(event_);
            }
        }
        public ActionResult NDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                var event_ = Db.EventTbls.SingleOrDefault(a => a.Event_ID == id);
                return View(event_);
            }
        }
    }
}