using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using MvcAuth.Models;

namespace MvcAuth.Controllers
{
    public class CourseTrue
    {
        public string StudentName { get; set; }
        public string StudentCourse { get; set; }
    }
    public class CourseFalse
    {
        public int RegId { get; set; }
        public string StudentName { get; set; }
        public string StudentCourse { get; set; }
    }
    public class ProfessorController : Controller
    {
        // GET: Professor
        UniversityDBEntities1 Db = new UniversityDBEntities1();
        public ActionResult Navigation()
        {
            ViewBag.Students = Db.StudentTbls.Count();
            ViewBag.Professors = Db.ProfessorTbls.Count();
            ViewBag.Courses = Db.CourseTbls.Count();
            ViewBag.Posts = Db.PostTbls.Count();
            ViewBag.NewSignUp = Db.StudentTbls.Count() + Db.ProfessorTbls.Count() + Db.AdminTbls.Count() - 20;
            int id = Convert.ToInt32(Session["ProfessorID"]);
            ViewBag.img = Db.ProfessorTbls.Where(x => x.Prof_ID == id).FirstOrDefault();
            return View();
        }
        public ActionResult RenderImage()
        {

            int id = Convert.ToInt32(Session["ProfessorID"]);
            var item = Db.ProfessorTbls.Where(x => x.Prof_ID == id).FirstOrDefault();
            byte[] photoBack = item.Prof_Image;

            return File(photoBack, "image/png");
        }
        public ActionResult FileUpload()
        {

            ViewBag.Courses = new SelectList(Db.CourseTbls, "Course_Name", "Course_Name");

            return View();
        }
        [HttpPost]
        public ActionResult FileUpload(HttpPostedFileBase files, string Course_Name)
        {

            String FileExt = Path.GetExtension(files.FileName).ToUpper();

            if (FileExt == ".PDF")
            {
                Stream str = files.InputStream;
                BinaryReader Br = new BinaryReader(str);
                Byte[] FileDet = Br.ReadBytes((Int32)str.Length);

                Prof_Lecture_TBL Fd = new Prof_Lecture_TBL();
                Fd.Prof_Lecture_Name = files.FileName;
                Fd.Prof_Lecture = FileDet;

                Fd.Prof_ID = Convert.ToInt32(Session["ProfessorID"]);
                int course_id = (from ss in Db.CourseTbls
                                 where ss.Course_Name == Course_Name
                                 select ss.Course_ID).FirstOrDefault();
                Fd.Course_ID = course_id;
                Db.Prof_Lecture_TBL.Add(Fd);


                Db.SaveChanges();
                TempData["lecture"] = " ";
                return RedirectToAction("FileUpload");
            }
            else
            {

                ViewBag.FileStatus = "Invalid file format.";
                return View();

            }
        }
        [HttpGet]
        public PartialViewResult FileDetails()
        {
            List<Prof_Lecture_TBL> DetList = Db.Prof_Lecture_TBL.ToList();

            return PartialView("FileDetails", DetList);
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
        public ActionResult Deletelecture(int id)
        {
            Prof_Lecture_TBL lecture = Db.Prof_Lecture_TBL.Find(id);
            if (lecture == null)
            {
                return HttpNotFound();
            }
            Db.Prof_Lecture_TBL.Remove(lecture);
            Db.SaveChanges();
            TempData["DeletedLecture"] = " ";
            return RedirectToAction("FileUpload", "Professor");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Deletelecture(Prof_Lecture_TBL lecture)
        {
            Db.Entry(lecture).State = EntityState.Deleted;
            Db.SaveChanges();
            return RedirectToAction("FileUpload", "Professor");
        }
        public ActionResult UploadVideo()
        {
            ViewBag.Courses = new SelectList(Db.CourseTbls, "Course_Name", "Course_Name");
            return View();
        }
        private string GetYouTubeID(string youTubeUrl)
        {
            //RegEx to Find YouTube ID
            Match regexMatch = Regex.Match(youTubeUrl, "^[^v]+v=(.{11}).*",
                               RegexOptions.IgnoreCase);
            if (regexMatch.Success)
            {
                return "http://www.youtube.com/v/" + regexMatch.Groups[1].Value +
                       "&hl=en&fs=1";
            }
            return youTubeUrl;
        }
        [HttpPost]
        public ActionResult SaveURL(Prof_Video_TBL video, string Course_Name)
        {
            Prof_Video_TBL video1 = new Prof_Video_TBL();
            video1.Prof_Video_Name = video.Prof_Video_Name;
            string url = video.Prof_Video.ToString();
            video1.Prof_Video = GetYouTubeID(url);
            video1.Prof_ID = Convert.ToInt32(Session["ProfessorID"]);
            int course_id = (from ss in Db.CourseTbls
                             where ss.Course_Name == Course_Name
                             select ss.Course_ID).FirstOrDefault();
            video1.Course_ID = course_id;
            Db.Prof_Video_TBL.Add(video1);
            Db.SaveChanges();
            TempData["video"] = " ";
            return RedirectToAction("UploadVideo");
        }
        [HttpGet]
        public PartialViewResult VideoDetails()
        {
            List<Prof_Video_TBL> DetList = Db.Prof_Video_TBL.ToList();

            return PartialView("VideoDetails", DetList);
        }
        [HttpGet]
        public ActionResult DeleteVideo(int id)
        {
            Prof_Video_TBL video = Db.Prof_Video_TBL.Find(id);
            if (video == null)
            {
                return HttpNotFound();
            }
            Db.Prof_Video_TBL.Remove(video);
            Db.SaveChanges();
            TempData["DeletedVideo"] = " ";
            return RedirectToAction("UploadVideo");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteVideo(Prof_Video_TBL video)
        {
            Db.Entry(video).State = EntityState.Deleted;
            Db.SaveChanges();
            return RedirectToAction("UploadVideo", "Professor");
        }
        [HttpGet]
        public ActionResult AddEvents()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SaveEvents(EventTbl event1)
        {
            EventTbl event2 = new EventTbl();
            // event2.Admin_ID = 0;
            event2.Event_Name = event1.Event_Name;
            event2.Event_Content = event1.Event_Content;
            Db.EventTbls.Add(event2);
            Db.SaveChanges();
            TempData["event"] = " ";
            return RedirectToAction("AddEvents");
        }
        [HttpGet]
        public PartialViewResult EventsDetails()
        {
            List<EventTbl> EvList = Db.EventTbls.ToList();

            return PartialView("EventsDetails", EvList);
        }
        [HttpGet]
        public ActionResult DeleteEvent(int id)
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteEvent(EventTbl event1)
        {
            Db.Entry(event1).State = EntityState.Deleted;
            Db.SaveChanges();
            return RedirectToAction("AddEvents", "Professor");
        }
        public ActionResult ExamManagment()
        {
            int ProfId = Convert.ToInt32(Session["ProfessorID"]);
            return View(Db.ExamTbls.Where(a => a.Prof_ID == ProfId).ToList());
        }
        [HttpGet]
        public ActionResult AddExam()

        {
            int ProfId = Convert.ToInt32(Session["ProfessorID"]);
            var courses = from s in Db.CourseTbls
                          from d in Db.Prof_Course_Tbl
                          where s.Course_ID == d.Course_ID
                          where d.Prof_ID == ProfId
                          select new { s.Course_ID, s.Course_Name };
            ViewBag.Courses = new SelectList(courses, "Course_ID", "Course_Name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddExam(ExamTbl exam)
        {
            if (ModelState.IsValid)
            {
                if (exam.Ex_Passing_Mark < exam.Ex_Total)
                {
                    TimeSpan difTime = exam.Ex_End_On.Subtract(exam.Ex_Start_On);
                    if (exam.Ex_End_On > exam.Ex_Start_On && exam.Ex_Start_On >= DateTime.Now && exam.Ex_End_On > DateTime.Now && (int)difTime.TotalMinutes > exam.Ex_Munite_Duration && exam.Ex_Munite_Duration > 0)
                    {
                        int ProfIda = Convert.ToInt32(Session["ProfessorID"]);
                        ExamTbl examTbl = new ExamTbl();
                        examTbl.Ex_Name = exam.Ex_Name;
                        examTbl.Ex_Info = exam.Ex_Info;
                        examTbl.Ex_Munite_Duration = exam.Ex_Munite_Duration;
                        examTbl.Course_ID = exam.Course_ID;
                        examTbl.Prof_ID = ProfIda;
                        examTbl.Ex_Total = exam.Ex_Total;
                        examTbl.Ex_Start_On = exam.Ex_Start_On;
                        examTbl.Ex_End_On = exam.Ex_End_On;
                        examTbl.Ex_Passing_Mark = exam.Ex_Passing_Mark;
                        examTbl.IsShow = false;
                        Db.ExamTbls.Add(examTbl);
                        Db.SaveChanges();
                        return RedirectToAction("ExamManagment");
                    }
                    else
                    {
                        ViewBag.msg = "There is a mistake in choosing the time to start and end the test. The end time of the test must be greater than its start and the exam time should be sufficient.";
                    }
                }
                else
                {
                    ViewBag.msg = "There is an error in the distribution of grades, the passing grades in the exam must be less than the total number of grades.";
                }
            }
            int ProfId = Convert.ToInt32(Session["ProfessorID"]);
            var courses = from s in Db.CourseTbls
                          from d in Db.Prof_Course_Tbl
                          where s.Course_ID == d.Course_ID
                          where d.Prof_ID == ProfId
                          select new { s.Course_ID, s.Course_Name };
            ViewBag.Courses = new SelectList(courses, "Course_ID", "Course_Name");
            return View(exam);
        }
        [HttpGet]
        public ActionResult DetailsExam(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {

                var questions = Db.QuestionsTbls.Where(a => a.Ex_ID == id);
                var Exam = Db.ExamTbls.Single(a => a.Ex_ID == id);

                int Deg = questions.Sum(a => (int?)a.Degree) ?? 0;
                int TotalExam = Convert.ToInt32(Exam.Ex_Total);
                if (Deg == TotalExam)
                {
                    ViewBag.msg = "Exam is Complate";
                }
                else
                {
                    ViewBag.msg = "Exam is not Complate Please add All Questions";
                }

                int ProfId = Convert.ToInt32(Session["ProfessorID"]);
                var exam = Db.ExamTbls.Single(a => a.Ex_ID == id && a.Prof_ID == ProfId);
                return View(exam);
            }
        }
        [HttpGet]
        public ActionResult DeleteExam(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                int ProfId = Convert.ToInt32(Session["ProfessorID"]);
                var exam = Db.ExamTbls.FirstOrDefault(a => a.Ex_ID == id && a.Prof_ID == ProfId);
                return View(exam);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("DeleteExam")]
        public ActionResult DeleteExam_(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                int ProfId = Convert.ToInt32(Session["ProfessorID"]);
                var exam = Db.ExamTbls.Single(a => a.Ex_ID == id && a.Prof_ID == ProfId);
                Db.ExamTbls.Remove(exam);
                Db.SaveChanges();
                return RedirectToAction("ExamManagment");
            }
        }
        [HttpGet]
        public ActionResult EditExam(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                int ProfId = Convert.ToInt32(Session["ProfessorID"]);
                var courses = from s in Db.CourseTbls
                              from d in Db.Prof_Course_Tbl
                              where s.Course_ID == d.Course_ID
                              where d.Prof_ID == ProfId
                              select new { s.Course_ID, s.Course_Name };
                ViewBag.Courses = new SelectList(courses, "Course_ID", "Course_Name");
                var exam = Db.ExamTbls.Single(a => a.Ex_ID == id && a.Prof_ID == ProfId);
                return View(exam);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditExam(ExamTbl exam)
        {

            if (ModelState.IsValid)
            {
                if (exam.Ex_Passing_Mark < exam.Ex_Total)
                {
                    TimeSpan difTime = exam.Ex_End_On.Subtract(exam.Ex_Start_On);
                    if (exam.Ex_End_On > exam.Ex_Start_On && exam.Ex_Start_On >= DateTime.Now && exam.Ex_End_On > DateTime.Now && (int)difTime.TotalMinutes > exam.Ex_Munite_Duration && exam.Ex_Munite_Duration > 0)
                    {
                        var examTbl = Db.ExamTbls.Single(a => a.Ex_ID == exam.Ex_ID);
                        examTbl.Ex_Name = exam.Ex_Name;
                        examTbl.Ex_Info = exam.Ex_Info;
                        examTbl.Ex_Munite_Duration = exam.Ex_Munite_Duration;
                        examTbl.Course_ID = exam.Course_ID;
                        examTbl.Ex_Total = exam.Ex_Total;
                        examTbl.Ex_Start_On = exam.Ex_Start_On;
                        examTbl.Ex_End_On = exam.Ex_End_On;
                        examTbl.Ex_Passing_Mark = exam.Ex_Passing_Mark;
                        examTbl.IsShow = exam.IsShow;
                        Db.SaveChanges();
                        return RedirectToAction("ExamManagment");
                    }
                    else
                    {
                        ViewBag.msg = "There is a mistake in choosing the time to start and end the test. The end time of the test must be greater than its start and the exam time should be sufficient.";
                    }
                }
                else
                {
                    ViewBag.msg = "There is an error in the distribution of grades, the passing grades in the exam must be less than the total number of grades.";
                }
            }
            int ProfId = Convert.ToInt32(Session["ProfessorID"]);
            var courses = from s in Db.CourseTbls
                          from d in Db.Prof_Course_Tbl
                          where s.Course_ID == d.Course_ID
                          where d.Prof_ID == ProfId
                          select new { s.Course_ID, s.Course_Name };
            ViewBag.Courses = new SelectList(courses, "Course_ID", "Course_Name"); return View(exam);
        }
        public ActionResult QuestionsManagment()
        {
            int ProfId = Convert.ToInt32(Session["ProfessorID"]);
            return View(Db.QuestionsTbls.Where(a => a.ExamTbl.Prof_ID == ProfId).ToList());
        }
        [HttpGet]
        public ActionResult AddQuestion()
        {
            int ProfIdint = Convert.ToInt32(Session["ProfessorID"]);
            List<ExamTbl> exams = Db.ExamTbls.Where(a => a.Prof_ID == ProfIdint).ToList();
            ViewBag.Exames = new SelectList(exams, "Ex_ID", "Ex_Name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddQuestion(QuestionsTbl questions)
        {
            string Correct = null;
            int ProfIdint = Convert.ToInt32(Session["ProfessorID"]);
            if (ModelState.IsValid)
            {
                var questionsDeg = Db.QuestionsTbls.Where(a => a.Ex_ID == questions.Ex_ID);
                var ExamDeg = Db.ExamTbls.Single(a => a.Ex_ID == questions.Ex_ID);

                int TotalQuestionDeg = questionsDeg.Sum(a => (int?)a.Degree) ?? 0;
                int TotalExamDeg = Convert.ToInt32(ExamDeg.Ex_Total);
                if (TotalExamDeg > TotalQuestionDeg && questions.Degree != 0 && questions.Degree <= (TotalExamDeg - TotalQuestionDeg))
                {
                    if (questions.Correct_Choice.Equals("A"))
                    {
                        Correct = questions.First_Choice;
                    }
                    else if (questions.Correct_Choice.Equals("B"))
                    {
                        Correct = questions.Second_Choice;
                    }
                    else if (questions.Correct_Choice.Equals("C"))
                    {
                        Correct = questions.Third_Choice;
                    }
                    else if (questions.Correct_Choice.Equals("D"))
                    {
                        Correct = questions.Fourth_Choice;
                    }
                    QuestionsTbl questionsTbl = new QuestionsTbl();
                    questionsTbl.Ex_ID = questions.Ex_ID;
                    questionsTbl.Question = questions.Question;
                    questionsTbl.First_Choice = questions.First_Choice;
                    questionsTbl.Second_Choice = questions.Second_Choice;
                    questionsTbl.Third_Choice = questions.Third_Choice;
                    questionsTbl.Fourth_Choice = questions.Fourth_Choice;
                    questionsTbl.Correct_Choice = Correct;
                    questionsTbl.Degree = questions.Degree;
                    Db.QuestionsTbls.Add(questionsTbl);
                    Db.SaveChanges();
                    return RedirectToAction("AddQuestion", "Professor");
                }
                else
                {
                    ViewBag.msg = "Can not add More Questuions Because Degree.";
                }
            }
            List<ExamTbl> exams = Db.ExamTbls.Where(a => a.Prof_ID == ProfIdint).ToList();
            ViewBag.Exames = new SelectList(exams, "Ex_ID", "Ex_Name");
            return View(questions);
        }
        [HttpGet]
        public ActionResult DetailsQuestion(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                int ProfId = Convert.ToInt32(Session["ProfessorID"]);
                var Question = Db.QuestionsTbls.Single(a => a.Question_ID == id && a.ExamTbl.Prof_ID == ProfId);
                return View(Question);
            }
        }
        [HttpGet]
        public ActionResult DeleteQuestion(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                int ProfId = Convert.ToInt32(Session["ProfessorID"]);
                var Question = Db.QuestionsTbls.Single(a => a.Question_ID == id && a.ExamTbl.Prof_ID == ProfId);
                return View(Question);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("DeleteQuestion")]
        public ActionResult DeleteQuestion_(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                int ProfId = Convert.ToInt32(Session["ProfessorID"]);
                var Question = Db.QuestionsTbls.Single(a => a.Question_ID == id && a.ExamTbl.Prof_ID == ProfId);
                Db.QuestionsTbls.Remove(Question);
                Db.SaveChanges();
                return RedirectToAction("QuestionsManagment");
            }
        }
        [HttpGet]
        public ActionResult EditQuestion(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                int ProfId = Convert.ToInt32(Session["ProfessorID"]);
                var Question = Db.QuestionsTbls.Single(a => a.Question_ID == id && a.ExamTbl.Prof_ID == ProfId);
                if (Question.Correct_Choice == Question.First_Choice)
                {
                    Question.Correct_Choice = "A";
                }
                else if (Question.Correct_Choice == Question.Second_Choice)
                {
                    Question.Correct_Choice = "B";
                }
                else if (Question.Correct_Choice == Question.Third_Choice)
                {
                    Question.Correct_Choice = "C";
                }
                else if (Question.Correct_Choice == Question.Fourth_Choice)
                {
                    Question.Correct_Choice = "D";
                }
                TempData["oldDeg"] = Question.Degree;
                int ProfIdint = Convert.ToInt32(Session["ProfessorID"]);
                List<ExamTbl> exams = Db.ExamTbls.Where(a => a.Prof_ID == ProfIdint).ToList();
                ViewBag.Exames = new SelectList(exams, "Ex_ID", "Ex_Name");
                return View(Question);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditQuestion(QuestionsTbl questions)
        {
            string Correct = null;
            int ProfIdint = Convert.ToInt32(Session["ProfessorID"]);
            if (ModelState.IsValid)
            {
                var Question = Db.QuestionsTbls.Single(a => a.Question_ID == questions.Question_ID);
                Question.Degree = 0;
                Db.SaveChanges();
                var questionsDeg = Db.QuestionsTbls.Where(a => a.Ex_ID == questions.Ex_ID);
                var ExamDeg = Db.ExamTbls.Single(a => a.Ex_ID == questions.Ex_ID);

                int TotalQuestionDeg = questionsDeg.Sum(a => (int?)a.Degree) ?? 0;
                int TotalExamDeg = Convert.ToInt32(ExamDeg.Ex_Total);
                if (TotalExamDeg > TotalQuestionDeg && questions.Degree != 0 && questions.Degree <= (TotalExamDeg - TotalQuestionDeg))
                {
                    if (questions.Correct_Choice.Equals("A"))
                    {
                        Correct = questions.First_Choice;
                    }
                    else if (questions.Correct_Choice.Equals("B"))
                    {
                        Correct = questions.Second_Choice;
                    }
                    else if (questions.Correct_Choice.Equals("C"))
                    {
                        Correct = questions.Third_Choice;
                    }
                    else if (questions.Correct_Choice.Equals("D"))
                    {
                        Correct = questions.Fourth_Choice;
                    }
                    Question.Ex_ID = questions.Ex_ID;
                    Question.Question = questions.Question;
                    Question.First_Choice = questions.First_Choice;
                    Question.Second_Choice = questions.Second_Choice;
                    Question.Third_Choice = questions.Third_Choice;
                    Question.Fourth_Choice = questions.Fourth_Choice;
                    Question.Correct_Choice = Correct;
                    Question.Degree = questions.Degree;
                    Db.SaveChanges();
                    return RedirectToAction("QuestionsManagment", "Professor");
                }
                else
                {
                    ViewBag.msg = "Can not add More Questuions Because Degree.";
                }
            }
            List<ExamTbl> exams = Db.ExamTbls.Where(a => a.Prof_ID == ProfIdint).ToList();
            ViewBag.Exames = new SelectList(exams, "Ex_ID", "Ex_Name");
            return View(questions);

        }
        public ActionResult ExamView(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                int ProfId = Convert.ToInt32(Session["ProfessorID"]);
                var Exam = Db.ExamTbls.Single(a => a.Ex_ID == id && a.Prof_ID == ProfId);
                ViewBag.Ex_Name = Exam.Ex_Name;
                return View(Db.QuestionsTbls.Where(a => a.Ex_ID == id && a.ExamTbl.Prof_ID == ProfId).ToList());
            }
        }
        public ActionResult CourseStudents()
        {
            int ProfId = Convert.ToInt32(Session["ProfessorID"]);
            return View((from s in Db.RegisterCoursesTbls
                         from d in Db.Prof_Course_Tbl
                         where s.IsApprove == true
                         where s.Course_ID == d.Course_ID
                         where d.Prof_ID == ProfId
                         select new CourseTrue()
                         {
                             StudentName = s.StudentTbl.St_Name,
                             StudentCourse = d.CourseTbl.Course_Name
                         }).ToList());
        }
        public ActionResult ApproveCourses()
        {
            int ProfId = Convert.ToInt32(Session["ProfessorID"]);
            return View((from s in Db.RegisterCoursesTbls
                         from d in Db.Prof_Course_Tbl
                         where s.IsApprove == false
                         where s.Course_ID == d.Course_ID
                         where d.Prof_ID == ProfId
                         select new CourseFalse()
                         {
                             RegId = s.IdRegister,
                             StudentName = s.StudentTbl.St_Name,
                             StudentCourse = d.CourseTbl.Course_Name
                         }).ToList());
        }
        [HttpGet]
        public ActionResult Approve(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                TempData["idReg"] = id;
                var reg = Db.RegisterCoursesTbls.Single(a => a.IdRegister == id);
                return View(reg);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Approve()
        {
            int id = Convert.ToInt32(TempData["idReg"]);
            var reg = Db.RegisterCoursesTbls.Single(a => a.IdRegister == id);
            reg.IsApprove = true;
            Db.SaveChanges();
            return RedirectToAction("ApproveCourses");

        }
    }
}