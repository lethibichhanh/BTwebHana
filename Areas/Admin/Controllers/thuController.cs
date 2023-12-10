using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SachOnline.Models;
using PagedList;
using PagedList.Mvc;
using System.IO;

namespace SachOnline.Areas.Admin.Controllers
{
    public class thuController : Controller
    {

        // GET: Admin/ChuDe
        dbSachOnlineDataContext db = new dbSachOnlineDataContext();
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult DsChuDe()
        {
            try
            {
                var dsCD = (from cd in db.Categories
                            select new
                            {
                                CategoryID = cd.CategoryID,
                                CategoryName = cd.CategoryName
                            }).ToList();
                return Json(new { code = 200, dsCD = dsCD, msg = "Lấy danh sách chủ đề thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { code = 500, msg = "Lấy danh sách chủ đề thất bại " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult Detail(int CategoryID)
        {
            try
            {
                var cd = (from s in db.Categories where (s.CategoryID == CategoryID)
                            select new
                            {
                                CategoryID = s.CategoryID,
                                CategoryName = s.CategoryName
                            }).SingleOrDefault();
                return Json(new { code = 200, cd = cd, msg = "Lấy danh sách chủ đề thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch( Exception ex)
            {
                return Json(new { code = 500, msg = "Lấy danh sách chủ đề thất bại " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        
    }
}