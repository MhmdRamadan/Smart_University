using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcAuth.Models;
using QRCoder;
using ZXing;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using static MvcAuth.Controllers.ManageController;
using System.Data.Entity;
namespace MvcAuth.Controllers
{
    public class AdminController : Controller
    {
        UniversityDBEntities1 Db = new UniversityDBEntities1();
        public ActionResult Navigation()
        {
            if (Session["AdminID"] == null || Session["AdminName"] == null || Session["AdminEmail"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                ViewBag.Students = Db.StudentTbls.Count().ToString();
                ViewBag.Professors = Db.ProfessorTbls.Count().ToString();
                ViewBag.Admins = Db.AdminTbls.Count().ToString();
                ViewBag.Posts = Db.PostTbls.Count().ToString();
                ViewBag.Exams = Db.ExamTbls.Count().ToString();
                ViewBag.Results = Db.Exam_Result_Tbl.Count().ToString();
                ViewBag.News = Db.NewsTbls.Count().ToString();
                ViewBag.Events = Db.EventTbls.Count().ToString();
                ViewBag.Courses = Db.CourseTbls.Count().ToString();
                ViewBag.Questions = Db.QuestionsTbls.Count().ToString();
                ViewBag.Lectures = Db.Prof_Lecture_TBL.Count().ToString();
                ViewBag.Videos = Db.Prof_Video_TBL.Count().ToString(); ;
                return View();
            }
        }
        public ActionResult RenderImage()
        {
            if (Session["AdminID"] == null || Session["AdminName"] == null || Session["AdminEmail"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                int id = Convert.ToInt32(Session["AdminID"]);
                var item = Db.AdminTbls.Where(x => x.Admin_ID == id).FirstOrDefault();
                byte[] photoBack = item.Admin_Pic;
                return File(photoBack, "image/png");
            }
        }
        [HttpGet]
        public ActionResult ActionQrCode()
        {
            ViewBag.Date = DateTime.Now;
            return View();
        }

        [HttpPost]
        public ActionResult ActionQrCode(QrTbl qrcode2)
        {
            string code = qrcode2.QRCodeName;
            QRCodeGenerator ObjQr = new QRCodeGenerator();
            QRCodeData qrCodeData = ObjQr.CreateQrCode(code, QRCodeGenerator.ECCLevel.Q);
            Bitmap bitMap = new QRCode(qrCodeData).GetGraphic(20);
            Image image = (Image)bitMap;
            ViewBag.bitmap = bitMap;
            qrcode2.QRCodeImage = BitmapToBytes(bitMap);
            QrTbl tbl = new QrTbl();
            tbl.QRCodeImage = qrcode2.QRCodeImage;
            tbl.QRCodeName = qrcode2.QRCodeName;
            tbl.Date = qrcode2.Date;
            tbl.Prof_ID = (int)Session["ProfessorID"];
            Db.QrTbls.Add(tbl);
            Db.SaveChanges();
            return View("QrDetails", qrcode2);

        }
        private static byte[] BitmapToBytes(Bitmap img)
        {
            MemoryStream stream = new MemoryStream();
            img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            return stream.ToArray();
        }
        [HttpGet]
        public ActionResult DownloadQRcode(string NAME)
        {
            List<QrTbl> ObjFiles = Db.QrTbls.ToList();
            var qrcode2ById = (from qr in ObjFiles
                               where qr.QRCodeName.Equals(NAME)
                               select new { qr.QRCodeName, qr.QRCodeImage }).ToList().FirstOrDefault();
            byte[] bytes = qrcode2ById.QRCodeImage;

            string contentType = "Image/jpeg";
            Image img = byteArrayToImage(bytes);
            return File(bytes, contentType, img + ".jpg");
        }
        public Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }
        [HttpGet]
        public ActionResult CreateCourse()
        {
            if (Session["AdminID"] == null || Session["AdminName"] == null || Session["AdminEmail"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                ViewBag.Poffessors = new SelectList(Db.ProfessorTbls, "Prof_ID", "Prof_Name");
                return View();
            }

        }
        [HttpPost]
        public ActionResult CreateCourse(CourseTbl course)
        {
            if (Session["AdminID"] == null || Session["AdminName"] == null || Session["AdminEmail"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                CourseTbl course1 = new CourseTbl();
                course1.Course_Name = course.Course_Name;
                Db.CourseTbls.Add(course1);
                Db.SaveChanges();
                TempData["course"] = " ";
                return RedirectToAction("CreateCourse");
            }
        }
        [HttpGet]
        public ActionResult DeleteCourse(int id)
        {
            if (Session["AdminID"] == null || Session["AdminName"] == null || Session["AdminEmail"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                var course = Db.CourseTbls.Where(x => x.Course_ID == id).FirstOrDefault();
                Db.CourseTbls.Remove(course);
                Db.SaveChanges();
                TempData["DeletedCourse"] = " ";
                return RedirectToAction("GetCourses");
            }
        }
        [HttpGet]
        public ActionResult ProfCourse()
        {
            if (Session["AdminID"] == null || Session["AdminName"] == null || Session["AdminEmail"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                ViewBag.Poffessors = new SelectList(Db.ProfessorTbls, "Prof_Name", "Prof_Name");
                ViewBag.Courses = new SelectList(Db.CourseTbls, "Course_Name", "Course_Name");

                return View();
            }
        }
        [HttpPost]
        public ActionResult ProfCourse(Prof_Course_Tbl tbl, string Prof_Name, string Course_Name)
        {
            if (Session["AdminID"] == null || Session["AdminName"] == null || Session["AdminEmail"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                ViewBag.Poffessors = new SelectList(Db.ProfessorTbls, "Prof_Name", "Prof_Name");
                ViewBag.Courses = new SelectList(Db.CourseTbls, "Course_Name", "Course_Name");
                Prof_Course_Tbl tbl1 = new Prof_Course_Tbl();
                int prof_id = (from s in Db.ProfessorTbls
                               where s.Prof_Name == Prof_Name
                               select s.Prof_ID).FirstOrDefault();
                int course_id = (from ss in Db.CourseTbls
                                 where ss.Course_Name == Course_Name
                                 select ss.Course_ID).FirstOrDefault();
                tbl1.Prof_ID = prof_id;
                tbl1.Course_ID = course_id;
                Db.Prof_Course_Tbl.Add(tbl1);
                Db.SaveChanges();
                TempData["profcourse"] = " ";
                return RedirectToAction("ProfCourse");
            }
        }
        [HttpGet]
        public ActionResult GetCourses()
        {
            if (Session["AdminID"] == null || Session["AdminName"] == null || Session["AdminEmail"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                List<CourseTbl> courseTbls = Db.CourseTbls.ToList();
                return View("GetCourses", courseTbls);
            }

        }
        [HttpGet]
        public PartialViewResult GetProfCourses()
        {
            if (Session["AdminID"] == null || Session["AdminName"] == null || Session["AdminEmail"] == null)
            {
                RedirectToAction("Login", "Account");
            }
            else
            {
                List<Prof_Course_Tbl> ProfCourse = Db.Prof_Course_Tbl.ToList();
                return PartialView("GetProfCourses", ProfCourse);
            }
            return PartialView();
        }

        [HttpGet]
        public ActionResult DeleteProfCourse(int id)
        {
            if (Session["AdminID"] == null || Session["AdminName"] == null || Session["AdminEmail"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                var profcourse = Db.Prof_Course_Tbl.Where(x => x.ID == id).FirstOrDefault();
                Db.Prof_Course_Tbl.Remove(profcourse);
                Db.SaveChanges();
                TempData["DeletedProfCourse"] = " ";
                return RedirectToAction("ProfCourse");
            }
        }
        [HttpGet]
        public ActionResult EditCourse(int id)
        {
            if (Session["AdminID"] == null || Session["AdminName"] == null || Session["AdminEmail"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                var course = Db.CourseTbls.Where(x => x.Course_ID == id).FirstOrDefault();
                return View(course);
            }
        }
        [HttpPost]
        public ActionResult EditCourse(CourseTbl crs)
        {
            if (Session["AdminID"] == null || Session["AdminName"] == null || Session["AdminEmail"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                var CourseToUpdate = Db.CourseTbls.Single(x => x.Course_ID == crs.Course_ID);
                CourseToUpdate.Course_Name = crs.Course_Name;
                Db.SaveChanges();
                TempData["EditedCourse"] = " ";
                return RedirectToAction("GetCourses");
            }
        }
        public ActionResult MyProfile()
        {
            if (Session["AdminID"] == null || Session["AdminName"] == null || Session["AdminEmail"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                return View();
            }
        }
        [HttpGet]
        public ActionResult EditMyProfile()
        {
            if (Session["AdminID"] == null || Session["AdminName"] == null || Session["AdminEmail"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                int id = int.Parse(Session["AdminID"].ToString());
                return View(Db.AdminTbls.SingleOrDefault(a => a.Admin_ID == id));
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMyProfile(AdminTbl admin)
        {
            HttpPostedFileBase file = Request.Files["fileInput"];
            int id = int.Parse(Session["AdminID"].ToString());
            var user = Db.AdminTbls.SingleOrDefault(a => a.Admin_ID == id);
            if (ModelState.IsValid)
            {
                user.Admin_Name = admin.Admin_Name;
                if (file.ContentLength > 0)
                {
                    user.Admin_Pic = ConvertToBytes(file);
                }
                var aspuser = Db.AspNetUsers.Single(a => a.Email == user.Admin_Email);
                aspuser.FullName = admin.Admin_Name;
                Db.SaveChanges();
                Session["AdminName"] = user.Admin_Name;
                return RedirectToAction("MyProfile", "Admin");

            }
            return View();
        }
        public byte[] ConvertToBytes(HttpPostedFileBase image)
        {
            byte[] imageBytes = null;
            BinaryReader reader = new BinaryReader(image.InputStream);
            imageBytes = reader.ReadBytes((int)image.ContentLength);
            return imageBytes;
        }
        [HttpGet]
        public ActionResult ChangePassword()
        {
            if (Session["AdminID"] == null || Session["AdminName"] == null || Session["AdminEmail"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                return View();
            }
        }
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = UserManager.ChangePassword(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = UserManager.FindById(User.Identity.GetUserId());
                if (user != null)
                {
                    SignInManager.SignIn(user, isPersistent: false, rememberBrowser: false);
                }
                int id = int.Parse(Session["AdminID"].ToString());
                var userAdmin = Db.AdminTbls.SingleOrDefault(a => a.Admin_ID == id);
                userAdmin.Admin_Pass = model.ConfirmPassword;
                Db.SaveChanges();
                return RedirectToAction("MyProfile", "Admin", new { Message = ManageMessageId.ChangePasswordSuccess });

            }
            AddErrors(result);
            return View(model);
        }
        [HttpGet]
        public ActionResult AddEvents()
        {
            if (Session["AdminID"] == null || Session["AdminName"] == null || Session["AdminEmail"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult SaveEvents(EventTbl event1)
        {
            if (Session["AdminID"] == null || Session["AdminName"] == null || Session["AdminEmail"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    EventTbl event2 = new EventTbl();
                    event2.Admin_ID = int.Parse(Session["AdminID"].ToString());
                    event2.Event_Name = event1.Event_Name;
                    event2.Event_Content = event1.Event_Content;
                    Db.EventTbls.Add(event2);
                    Db.SaveChanges();
                    TempData["event"] = " ";
                    return RedirectToAction("AddEvents", "Admin");
                }
            }
            return View();
        }
        [HttpGet]
        public PartialViewResult EventsDetails()
        {
            if (Session["AdminID"] == null || Session["AdminName"] == null || Session["AdminEmail"] == null)
            {
                RedirectToAction("Login", "Account");
            }
            else
            {

                List<EventTbl> EvList = Db.EventTbls.ToList();

                return PartialView("EventsDetails", EvList);
            }
            return PartialView();
        }
        [HttpGet]
        public ActionResult DeleteEvent(int id)
        {
            if (Session["AdminID"] == null || Session["AdminName"] == null || Session["AdminEmail"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                EventTbl event1 = Db.EventTbls.Find(id);
                if (event1 == null)
                {
                    return HttpNotFound();
                }
                Db.EventTbls.Remove(event1);
                Db.SaveChanges();
                TempData["DeletedEvent"] = " ";
                return RedirectToAction("AddEvents");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteEvent(EventTbl event1)
        {
            if (Session["AdminID"] == null || Session["AdminName"] == null || Session["AdminEmail"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                Db.Entry(event1).State = EntityState.Deleted;
                Db.SaveChanges();
                return RedirectToAction("AddEvents", "Admin");
            }
        }
        public ActionResult Admins()
        {
            if (Session["AdminID"] == null || Session["AdminName"] == null || Session["AdminEmail"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                return View(Db.AdminTbls.ToList());
            }
        }
        public ActionResult Professors()
        {
            if (Session["AdminID"] == null || Session["AdminName"] == null || Session["AdminEmail"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                return View(Db.ProfessorTbls.ToList());
            }
        }
        public ActionResult Students()
        {
            if (Session["AdminID"] == null || Session["AdminName"] == null || Session["AdminEmail"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                return View(Db.StudentTbls.ToList());
            }
        }
        [HttpGet]
        public ActionResult CreateNews()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateNews(NewsTbl news)
        {
           
            System.Random random = new System.Random();
            news.News_ID= random.Next(1000);
            
            Db.NewsTbls.Add(news);
            Db.SaveChanges();
            TempData["News"] = " ";

            return View();
        }
    }
}