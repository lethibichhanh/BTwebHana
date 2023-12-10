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
    public class ChuDeController : Controller
    {
        dbSachOnlineDataContext db = new dbSachOnlineDataContext();


        public ActionResult Index(int? page)
        {

            int iPageNum = (page ?? 1);
            int iPageSize = 10;
            return View(db.Categories.ToList().OrderBy(n => n.CategoryID).ToPagedList(iPageNum, iPageSize));
        }
        [HttpGet]
        public ActionResult Create()
        {
            
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Category category, FormCollection f)
        {
            
            if (f["sCategoryName"] == null)
            {
                //Nội dung thông báo yêu cầu chọn ảnh bìa
                ViewBag.Thongbao = "Hãy nhập tên chủ đề";
                
                return View();
            }
            else
            {
                if (ModelState.IsValid)
                {


                    category.CategoryName = f["sCategoryName"];
                    db.Categories.InsertOnSubmit(category);
                    db.SubmitChanges();
                    //Về lại trang Quản lý sách
                    return RedirectToAction("Index");
                }
                return View();
            }
        }
        public ActionResult Details(int id)
        {
            var sach = db.Categories.SingleOrDefault(n => n.CategoryID == id);
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sach);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var chude = db.Categories.SingleOrDefault(n => n.CategoryID == id);
            if (chude == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(chude);
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirm(int id, FormCollection f)
        {
            var chude = db.Categories.SingleOrDefault(n => n.CategoryID == id);
            if (chude == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            //Xóa sách
            db.Categories.DeleteOnSubmit(chude);
            db.SubmitChanges();

            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var sach = db.Categories.SingleOrDefault(n => n.CategoryID == id);
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            //Hiển thị danh sách chủ đề và nhà xuất bản đồng thời chọn chủ đề và nhà xuất bản của cuốn hiện tại
            ViewBag.CategoryID = new SelectList(db.Categories.ToList().OrderBy(n => n.CategoryName), "CategoryID", "CategoryName", sach.CategoryID);
            return View(sach);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(FormCollection f)
        {
            var sach = db.Categories.SingleOrDefault(n => n.CategoryID == int.Parse(f["iCategoryID"]));
           
            if (ModelState.IsValid)
            {
                

                //Lưu sách vào cơ sở dữ liệu
                sach.CategoryName = f["sCategoryName"];
                

                db.SubmitChanges();
                //Về lại trang Quản lý sách
                return RedirectToAction("Index");
            }
            return View(sach);
        }

    }
}