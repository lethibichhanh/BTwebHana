using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SachOnline.Areas;
using SachOnline.Models;
//using SachOnline.Areas;



namespace SachOnline.Areas.Admin.Controllers
{

    public class HomeController : Controller
    {
        dbSachOnlineDataContext db = new dbSachOnlineDataContext();
        // GET: Admin/Home
        public ActionResult Index()
        {

            return View();
        }
        
        [HttpGet]
        public ActionResult Login()
        {
            
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection f)
        {
            
                // gán giá trị mà người dùng đã nhập cho biến
                var sTenDN = f["UserName"];
                var sMatKhau = f["Password"];
                // gán giá trị cho đối tượng được tạo mới (ad)
                ADMIN ad = db.ADMINs.SingleOrDefault(n => n.TenDN == sTenDN && n.MatKhau == sMatKhau);
                if (ad != null)
                {
                    Session["Admin"] = ad;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.ThongBao = "Tên đăng nhập hoặc mật khẩu không đúng";
                }
                return View();
            
        }
    }
}