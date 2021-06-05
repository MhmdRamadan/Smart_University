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

namespace MvcAuth.Controllers
{
    public class AdminController : Controller
    {
        UniversityDBEntities1 Db = new UniversityDBEntities1();
        public ActionResult Navigation()
        {
            ViewBag.Students = Db.StudentTbls.Count();
            ViewBag.Professors = Db.ProfessorTbls.Count();
            ViewBag.Courses = Db.CourseTbls.Count();
            ViewBag.Posts = Db.PostTbls.Count();
            ViewBag.NewSignUp = Db.StudentTbls.Count() + Db.ProfessorTbls.Count() + Db.AdminTbls.Count() - 20;
            int id = Convert.ToInt32(Session["AdminID"]);
            ViewBag.img = Db.AdminTbls.Where(x => x.Admin_ID == id).FirstOrDefault();

            return View();
        }
        public ActionResult RenderImage()
        {
            int id = Convert.ToInt32(Session["AdminID"]);
           var item= Db.AdminTbls.Where(x => x.Admin_ID == id).FirstOrDefault();
            byte[] photoBack = item.Admin_Pic;

            return File(photoBack, "image/png");
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

            ViewBag.Poffessors = new SelectList(Db.ProfessorTbls, "Prof_ID", "Prof_Name");
            return View();
        }
        [HttpPost]
        public ActionResult CreateCourse(CourseTbl course)
        {
            CourseTbl course1 = new CourseTbl();
            course1.Course_Name = course.Course_Name;
            Db.CourseTbls.Add(course1);
            Db.SaveChanges();
            TempData["course"] = " ";
            return RedirectToAction("CreateCourse");
        }
        [HttpGet]
        public ActionResult DeleteCourse(int id)
        {
            var course = Db.CourseTbls.Where(x => x.Course_ID == id).FirstOrDefault();
            Db.CourseTbls.Remove(course);
            Db.SaveChanges();
            TempData["DeletedCourse"] = " ";
            return RedirectToAction("GetCourses");
        }
        [HttpGet]
        public ActionResult ProfCourse()
        {
            ViewBag.Poffessors = new SelectList(Db.ProfessorTbls, "Prof_Name", "Prof_Name");
            ViewBag.Courses = new SelectList(Db.CourseTbls, "Course_Name", "Course_Name");
           
            return View();
        }
        [HttpPost]
        public ActionResult ProfCourse(Prof_Course_Tbl tbl, string Prof_Name, string Course_Name)
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
        [HttpGet]
        public ActionResult GetCourses()
        {
            List<CourseTbl> courseTbls = Db.CourseTbls.ToList();
            return View("GetCourses", courseTbls);
        }
        [HttpGet]
        public PartialViewResult GetProfCourses()
        {
            List<Prof_Course_Tbl> ProfCourse = Db.Prof_Course_Tbl.ToList();

            return PartialView("GetProfCourses", ProfCourse);
        }
        [HttpGet]
        public ActionResult DeleteProfCourse(int id)
        {
            var profcourse = Db.Prof_Course_Tbl.Where(x => x.ID == id).FirstOrDefault();
            Db.Prof_Course_Tbl.Remove(profcourse);
            Db.SaveChanges();
            TempData["DeletedProfCourse"] = " ";
            return RedirectToAction("ProfCourse");
        }
        [HttpGet]
        public ActionResult EditCourse(int id)
        {
            var course = Db.CourseTbls.Where(x => x.Course_ID == id).FirstOrDefault();
            return View(course);
        }
        [HttpPost]
        public ActionResult EditCourse(CourseTbl crs)
        {
            var CourseToUpdate = Db.CourseTbls.Single(x=>x.Course_ID==crs.Course_ID);
            CourseToUpdate.Course_Name = crs.Course_Name;
            Db.SaveChanges();
            TempData["EditedCourse"] = " ";
            return RedirectToAction("GetCourses");
        }
    }
}