using System;
using Main_project.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace Main_project.Controllers
{
    public class HomeController : Controller
    {
        DBQUIZ1Entities db = new DBQUIZ1Entities();
        public ActionResult Index()
        {
            if (Session["ad_id"] != null)
            {
                return RedirectToAction("Dashboard");
            }
            return View();

        }
        [HttpGet]
        public ActionResult sregister()
        {
            return View();
        }


        [HttpPost]
        public ActionResult sregister(TBL_STUDENT svw, HttpPostedFileBase imgfile)
        {
            TBL_STUDENT s = new TBL_STUDENT();
            try
            {
                s.S_NAME = svw.S_NAME;
                s.S_PASSWORD = svw.S_PASSWORD;
                s.S_IMAGE = uploadimgfile(imgfile);
                db.TBL_STUDENT.Add(s);
                db.SaveChanges();
                return RedirectToAction("slogin");
            }
            catch (Exception)
            {

                ViewBag.msg = "Data Could not be inserted......";
            }

            return View();
        }

        public string uploadimgfile(HttpPostedFileBase file)
        {
            Random r = new Random();
            string path = "-1";
            int random = r.Next();
            if (file != null && file.ContentLength > 0)
            {
                string extension = Path.GetExtension(file.FileName);
                if (extension.ToLower().Equals(".jpg") || extension.ToLower().Equals(".jpeg") || extension.ToLower().Equals(".png"))
                {
                    try
                    {

                        path = Path.Combine(Server.MapPath("~/Content/Images"), random + Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        path = "~/Content/Images/" + random + Path.GetFileName(file.FileName);

                        //    ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        path = "-1";
                    }
                }
                else
                {
                    Response.Write("<script>alert('Only jpg ,jpeg or png formats are acceptable....'); </script>");
                }
            }

            else
            {
                Response.Write("<script>alert('Please select a file'); </script>");
                path = "-1";
            }



            return path;
        }
        [HttpGet]
        public ActionResult tlogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult tlogin(TBL_ADMIN a)
        {
            TBL_ADMIN ad = db.TBL_ADMIN.Where(x => x.AD_NAME == a.AD_NAME && x.AD_PASSWORD == a.AD_PASSWORD).SingleOrDefault();
            if (ad != null)
            {
                Session["ad_id"] = ad.AD_ID;
                return RedirectToAction("Dashboard");
            }
            else
            {
                ViewBag.msg = "Invalid usename or password";
            }

            return View();
        }

        public ActionResult slogin()
        {
            return View();
        }

        public ActionResult Dashboard()
        {
            return View();
        }


        [HttpGet]
        public ActionResult Addcategory()
        {
           // Session["ad_id"] = 2; //we will remove it soon.......
            int adid = Convert.ToInt32(Session["ad_id"].ToString());
            List<tbl_categroy> li = db.tbl_categroy.Where(x => x.cat_fk_adid == adid).OrderByDescending(x => x.cat_id).ToList();
            ViewData["list"] = li;

            return View();
        }
        [HttpPost]
        public ActionResult Addcategory(tbl_categroy cat)
        {

            List<tbl_categroy> li = db.tbl_categroy.OrderByDescending(x => x.cat_id).ToList();
            ViewData["list"] = li;

            tbl_categroy c = new tbl_categroy();
            c.cat_name = cat.cat_name;
            c.cat_fk_adid = Convert.ToInt32(Session["ad_id"].ToString());
            db.tbl_categroy.Add(c);
            db.SaveChanges();
            return RedirectToAction("Addcategory");
        }


        [HttpGet]

        public ActionResult Addquestions()
        {
            int sid = Convert.ToInt32(Session["ad_id"]);
            List<tbl_categroy> li = db.tbl_categroy.Where(x => x.cat_fk_adid == sid).ToList();
            ViewBag.list = new SelectList(li, "cat_id", "cat_name");

            return View();
        }


        [HttpPost]

        public ActionResult Addquestions(TBL_QUESTIONS q)
        {
            int sid = Convert.ToInt32(Session["ad_id"]);
            List<tbl_categroy> li = db.tbl_categroy.Where(x => x.cat_fk_adid == sid).ToList();
            ViewBag.list = new SelectList(li, "cat_id", "cat_name");

            TBL_QUESTIONS QA = new TBL_QUESTIONS();
            QA.Q_TEXT = q.Q_TEXT;
            QA.OPA = q.OPA;
            QA.OPB = q.OPB;
            QA.OPC = q.OPC;
            QA.OPD = q.OPD;
            QA.COP = q.COP;
            QA.q_fk_catid = q.q_fk_catid;



            db.TBL_QUESTIONS.Add(QA);
            db.SaveChanges();
            TempData["msg"] = "Question added successfully....";
            TempData.Keep();
            return RedirectToAction("Addquestion");

        }



        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}