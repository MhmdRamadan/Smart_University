using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MvcAuth.Models;
using MvcAuth.Models.ModelView;
using QRCodeDecoderLibrary;
using QRCoder;
using ZXing;
using static MvcAuth.Controllers.ManageController;

namespace MvcAuth.Controllers
{
    public class StudentsController : Controller
    {
        UniversityDBEntities1 Db = new UniversityDBEntities1();
        public ActionResult Navigation(string id)
        {
           
                return View(Db.PostTbls.Where(a => a.Post_Content.Contains(id) || id == null).OrderByDescending(a => a.Post_Date).ToList());
            
        }
        public ActionResult RenderImageUser(int id)
        {
      
                var item = Db.StudentTbls.Where(x => x.St_ID == id).FirstOrDefault();
                byte[] photoBack = item.St_Image;
                return File(photoBack, "image/png");
            
        }
        [HttpGet]
        // GET: Students
        public ActionResult Absence()
        {
           
                return View();
            
        }
        [HttpPost]
        public ActionResult ComparePic(AbsenceView img)
        {
            HttpPostedFileBase file = Request.Files["ImageData"];
            byte[] byteArray = ConvertToBytes(file);
            TypeConverter tc = TypeDescriptor.GetConverter(typeof(Bitmap));
            Bitmap bitmap1 = (Bitmap)tc.ConvertFrom(byteArray);

            QRDecoder Decoder = new QRDecoder();
            byte[][] DataByteArray = Decoder.ImageDecoder(bitmap1);
            //   string Result = StudentsController.ByteArrayToStr(DataByteArray[0]);
            BarcodeReader reader = new BarcodeReader { AutoRotate = true, TryInverted = true };
            Result result1 = reader.Decode(bitmap1);
            string qrcodename = result1.ToString();
            var Date1 = (from qr in Db.QrTbls
                         where qr.QRCodeName == qrcodename
                         select qr).FirstOrDefault();
            DateTime dt;
            if (Date1.Date.HasValue)
                dt = Date1.Date.Value;
            else
                dt = DateTime.Now;

            string formatted = GetDateFromDateTime(dt);
            string nowformatted = GetDateFromDateTime(DateTime.Now);
            if (formatted == nowformatted)
            {
                Attendtbl attend = new Attendtbl();
                attend.St_ID = Convert.ToInt32(Session["StudentID"]);
                attend.Date = GetDateFromDateTime(DateTime.Now);
                Db.Attendtbls.Add(attend);
                Db.SaveChanges();
            }


            return View("Absence");
        }
        public static string GetDateFromDateTime(DateTime datevalue)
        {
            return datevalue.ToShortDateString();
        }
        public static string ByteArrayToStr(byte[] DataArray)
        {
            Decoder Decoder = Encoding.UTF8.GetDecoder();
            int CharCount = Decoder.GetCharCount(DataArray, 0, DataArray.Length);
            char[] CharArray = new char[CharCount];
            Decoder.GetChars(DataArray, 0, DataArray.Length, CharArray, 0);
            return new string(CharArray);
        }
        public byte[] ConvertToBytes(HttpPostedFileBase image)
        {
            byte[] imageBytes = null;
            BinaryReader reader = new BinaryReader(image.InputStream);
            imageBytes = reader.ReadBytes((int)image.ContentLength);
            return imageBytes;
        }
        [HttpGet]
        public ActionResult Lectures()
        {
          
                List<Prof_Lecture_TBL> DetList = Db.Prof_Lecture_TBL.ToList();
                return View("Lectures", DetList);
            
        }
        [HttpGet]
        public FileResult DownLoadFile(int id)
        {
           
            List<Prof_Lecture_TBL> ObjFiles = Db.Prof_Lecture_TBL.ToList();
            var FileById = (from FC in ObjFiles
                            where FC.Prof_Lecture_ID.Equals(id)
                            select new { FC.Prof_Lecture_Name, FC.Prof_Lecture }).ToList().FirstOrDefault();
            return File(FileById.Prof_Lecture, "application/pdf", FileById.Prof_Lecture_Name);

        }
        [HttpGet]
        public ActionResult Videos()
        {
          
                List<Prof_Video_TBL> DetList = Db.Prof_Video_TBL.ToList();
                return View("Videos", DetList);
            
        }
        public ActionResult Courses()
        {
            
           
                int id = Convert.ToInt32(Session["StudentID"]);
                return View(Db.RegisterCoursesTbls.Where(a => a.IsApprove == true && a.St_ID == id));
            
        }
        [HttpGet]
        public ActionResult RegisterCourse()
        {
           
                List<CourseTbl> courses = Db.CourseTbls.ToList();
                ViewBag.Courses = new SelectList(courses, "Course_ID", "Course_Name");
                return View();
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterCourse(RegisterCoursesTbl registerCourses)
        {
            if (registerCourses.Course_ID < 0)
            {
                ModelState.AddModelError("Course_ID", "Please Select Course");
            }
            if (ModelState.IsValid)
            {
                int id = Convert.ToInt32(Session["StudentID"]);
                var Reg = Db.RegisterCoursesTbls.SingleOrDefault(a => a.Course_ID == registerCourses.Course_ID && a.St_ID == id);
                if (Reg == null)
                {
                    RegisterCoursesTbl register = new RegisterCoursesTbl();
                    register.St_ID = id;
                    register.Course_ID = registerCourses.Course_ID;
                    register.IsApprove = false;
                    Db.RegisterCoursesTbls.Add(register);
                    Db.SaveChanges();
                    return RedirectToAction("Courses");
                }
                else
                {
                    if (Reg.IsApprove == false)
                    {
                        ViewBag.msg = "Already registered, please wait for approval";
                    }
                    else if (Reg.IsApprove == true)
                    {
                        ViewBag.msg = "Already registered";
                    }
                }

            }
            List<CourseTbl> courses = Db.CourseTbls.ToList();
            ViewBag.Courses = new SelectList(courses, "Course_ID", "Course_Name");
            return View();
        }
        [HttpGet]
        public ActionResult NewPost()
        {
      
                return View();
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewPost(string PostContent)
        {
            if (string.IsNullOrEmpty(PostContent) || string.IsNullOrWhiteSpace(PostContent))
            {
                ModelState.AddModelError("PostContent", "Please Insert Your Post.");
            }
            else
            {
                int idCurrentStudent = Convert.ToInt32(Session["StudentID"]);
                DateTime currentDate = DateTime.Now;
                PostTbl post = new PostTbl();
                post.Post_Content = PostContent.Replace("\r\n", "<br/>");
                post.Post_Date = currentDate;
                post.St_ID = idCurrentStudent;
                Db.PostTbls.Add(post);
                Db.SaveChanges();
                return RedirectToAction("Navigation", "Students");
            }
            return View();
        }
        [HttpGet]
        public ActionResult DeletePost(int id)
        {
          

                var post = Db.PostTbls.Where(a => a.Post_ID == id).FirstOrDefault();
                if (post != null)
                {
                    return View(post);
                }
                else
                {
                    return HttpNotFound();
                }
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("DeletePost")]
        public ActionResult DeletePost_(int id)
        {
            var post = Db.PostTbls.Where(p => p.Post_ID == id).FirstOrDefault();
            if (post != null)
            {
                Db.PostTbls.Remove(post);
                Db.SaveChanges();
                return RedirectToAction("Navigation", "Students");
            }
            else
            {
                return HttpNotFound();
            }

        }
        [HttpGet]
        public ActionResult Replys(int? id)
        {
           
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                else
                {
                    TempData["postId"] = id;
                    var post = Db.PostTbls.Where(p => p.Post_ID == id).FirstOrDefault();
                    ViewBag.idS = post.StudentTbl.St_ID;
                    ViewBag.name = post.StudentTbl.St_Name;
                    ViewBag.date = post.Post_Date;
                    ViewBag.content = post.Post_Content.Replace("<br/>", "\r\n");
                    return View(Db.Post_Reply_Tbl.Where(a => a.Post_ID == id).ToList());
                }

            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Replys(string ReplyContent)
        {

            int idCurrentStudent = Convert.ToInt32(Session["StudentID"]);
            int postID = Convert.ToInt32(TempData["postId"]);
            Post_Reply_Tbl reply_Tbl = new Post_Reply_Tbl();
            if (string.IsNullOrEmpty(ReplyContent) || string.IsNullOrWhiteSpace(ReplyContent))
            {
                reply_Tbl.Reply_Content = "Empty";
            }
            else
            {
                reply_Tbl.Reply_Content = ReplyContent; reply_Tbl.Post_ID = postID;
                reply_Tbl.St_ID = idCurrentStudent;
                Db.Post_Reply_Tbl.Add(reply_Tbl);
                Db.SaveChanges();
            }

            return RedirectToAction("Replys", "Students", new { id = postID });
        }
        public ActionResult AvaliableExam()
        {
           

                return View(Db.ExamTbls.Where(a => a.IsShow == true).ToList());
            
        }
        public ActionResult StartExam(int id)
        {
          
                ViewBag.idd = id;
                Session["ExamID"] = id;
                int idCurrentStudent = Convert.ToInt32(Session["StudentID"]);

                if (Db.Exam_Result_Tbl.Where(a => a.St_ID == idCurrentStudent && a.Ex_ID == id).FirstOrDefault() != null)
                {
                    return RedirectToAction("ExamResult");
                }
                else
                {

                    var Exam = Db.ExamTbls.SingleOrDefault(a => a.Ex_ID == id);
                    ViewBag.Name = Exam.Ex_Name.ToString();
                    ViewBag.By = Exam.ProfessorTbl.Prof_Name.ToString();
                    ViewBag.Degree = Exam.Ex_Total.ToString();
                    ViewBag.Info = Exam.Ex_Info.ToString();
                    ViewBag.Mint = Exam.Ex_Munite_Duration.ToString();
                    ViewBag.Time = Convert.ToInt32(Exam.Ex_Munite_Duration) * 60;

                    var Questions = Db.QuestionsTbls.Where(a => a.Ex_ID == id).ToList();

                    return View(Questions);
                }


            
        }
        [HttpPost]
        public ActionResult StartExam(FormCollection frm)
        {
            int idCurrentStudent = Convert.ToInt32(Session["StudentID"]);
            int id = Convert.ToInt32(Session["ExamID"]);
            var Questions = Db.QuestionsTbls.Where(a => a.Ex_ID == id).ToList();

            int result = 0;
            foreach (var item in Questions)
            {
                string answer = frm["Answer" + item.Question].ToString();

                if (answer == item.Correct_Choice)
                {
                    result += item.Degree;
                }
            }
            int result1 = result;
            Exam_Result_Tbl resulttbl = new Exam_Result_Tbl();
            resulttbl.Ex_ID = id;
            resulttbl.St_ID = Convert.ToInt32(Session["StudentID"]);
            resulttbl.Res_Degree = result;
            Db.Exam_Result_Tbl.Add(resulttbl);
            Db.SaveChanges();

            return RedirectToAction("ExamResult");

        }
        public ActionResult ExamResult()
        {
           
                int idCurrentStudent = Convert.ToInt32(Session["StudentID"]);
                var CurrentSudentRes = Db.Exam_Result_Tbl.Where(a => a.St_ID == idCurrentStudent).ToList();
                return View(CurrentSudentRes);
            
        }
        // from here to end
        public ActionResult RenderImage()
        {
       
                int id = Convert.ToInt32(Session["StudentID"]);
                var item = Db.StudentTbls.Where(x => x.St_ID == id).FirstOrDefault();
                byte[] photoBack = item.St_Image;

                return File(photoBack, "image/png");
            
        }
        public ActionResult MyProfile()
        {
          
                return View();
            
        }
        [HttpGet]
        public ActionResult EditMyProfile()
        {
            if (Session["StudentID"] == null || Session["StudentName"] == null || Session["StudentEmail"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                int id = int.Parse(Session["StudentID"].ToString());
                return View(Db.StudentTbls.SingleOrDefault(a => a.St_ID == id));
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMyProfile(StudentTbl stu)
        {
            HttpPostedFileBase file = Request.Files["fileInput"];
            int id = int.Parse(Session["StudentID"].ToString());
            var user = Db.StudentTbls.SingleOrDefault(a => a.St_ID == id);
            if (ModelState.IsValid)
            {
                user.St_Name = stu.St_Name;
                if (file.ContentLength > 0)
                {
                    user.St_Image = ConvertToBytes(file);
                }
                var aspuser = Db.AspNetUsers.Single(a => a.Email == user.St_Email);
                aspuser.FullName = stu.St_Name;
                Db.SaveChanges();
                Session["StudentName"] = user.St_Name;
                return RedirectToAction("MyProfile", "Students");

            }
            return View();
        }
        [HttpGet]
        public ActionResult ChangePassword()
        {
            
                return View();
            
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
                int id = int.Parse(Session["StudentID"].ToString());
                var userStu = Db.StudentTbls.SingleOrDefault(a => a.St_ID == id);
                userStu.St_Password = model.NewPassword;
                Db.SaveChanges();
                return RedirectToAction("MyProfile", "Students", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }
    }
}