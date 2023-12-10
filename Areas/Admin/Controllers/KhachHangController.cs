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
    public class KhachHangController : Controller
    {
        // GET: Admin/KhachHang
        dbSachOnlineDataContext db = new dbSachOnlineDataContext();
        public ActionResult Index(int? page)
        {
           
            int iPageNum = (page ?? 1);
            int iPageSize = 10;
            return View(db.KHACHHANGs.ToList().OrderBy(n => n.MaKH).ToPagedList(iPageNum, iPageSize));
        }
        [HttpGet]
        public ActionResult Create()
        {
            
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(KHACHHANG khachhang, FormCollection f)
        {
            
            if (f["sHoten"] == null)
            {
                //Nội dung thông báo yêu cầu chọn ảnh bìa
                ViewBag.Thongbao = "Nhập họ tên khách hàng";
                
                return View();
            }
            else if (f["sTaikhoan"] == null)
            {
                //Nội dung thông báo yêu cầu chọn ảnh bìa
                ViewBag.Thongbao = "Nhập tài khoản khách hàng";

                return View();
            }
            else if (f["sMatkhau"] == null)
            {
                //Nội dung thông báo yêu cầu chọn ảnh bìa
                ViewBag.Thongbao = "Nhập mật khẩu khách hàng";

                return View();
            }
            else if (f["sEmail"] == null)
            {
                //Nội dung thông báo yêu cầu chọn ảnh bìa
                ViewBag.Thongbao = "Nhập email khách hàng";

                return View();
            }
            else if (f["sDiachiKH"] == null)
            {
                //Nội dung thông báo yêu cầu chọn ảnh bìa
                ViewBag.Thongbao = "Nhập địa chỉ khách hàng";

                return View();
            }
            else if (f["sDienThoaiKH"] == null)
            {
                //Nội dung thông báo yêu cầu chọn ảnh bìa
                ViewBag.Thongbao = "Nhập điện thoại khách hàng";

                return View();
            }
            else if (f["sNgaysinh"] == null)
            {
                //Nội dung thông báo yêu cầu chọn ảnh bìa
                ViewBag.Thongbao = "Nhập ngày sinh khách hàng";

                return View();
            }
            else
            {
                if (ModelState.IsValid)
                {
                   
                   
                    //Lưu Sach vào csdl

                    khachhang.HoTen = f["sHoTen"];
                    khachhang.Taikhoan = f["sTaikhoan"];
                    khachhang.Matkhau = f["sMatkhau"];
                   
                    khachhang.Email = f["sEmail"];
                    khachhang.DiachiKH = f["sDiachiKH"];
                    khachhang.DienThoaiKH = f["sDienThoaiKH"];
                    khachhang.Ngaysinh = Convert.ToDateTime(f["sNgaysinh"]);
                    db.KHACHHANGs.InsertOnSubmit(khachhang);
                    db.SubmitChanges();
                    //Về lại trang Quản lý sách
                    return RedirectToAction("Index");
                }
                return View();
            }
        }
        public ActionResult Details(int id)
        {
            var sach = db.KHACHHANGs.SingleOrDefault(n => n.MaKH == id);
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sach);
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var sach = db.KHACHHANGs.SingleOrDefault(n => n.MaKH == id);
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
           
            return View(sach);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(FormCollection f, HttpPostedFileBase fFileUpload)
        {
            var sach = db.KHACHHANGs.SingleOrDefault(n => n.MaKH == int.Parse(f["sMaKH"]));
           
            if (ModelState.IsValid)
            {


                //Lưu sách vào cơ sở dữ liệu
                sach.HoTen = f["sHoTen"];
                sach.Taikhoan = f["sTaikhoan"];
                sach.Matkhau = f["sMatkhau"];

                sach.Email = f["sEmail"];
                sach.DiachiKH = f["sDiachiKH"];
                sach.DienThoaiKH = f["sDienThoaiKH"];
                sach.Ngaysinh = Convert.ToDateTime(f["sNgaysinh"]);


                db.SubmitChanges();
                //Về lại trang Quản lý sách
                return RedirectToAction("Index");
            }
            return View(sach);
        }
        [HttpGet]
        public ActionResult Delete(int id)
        {
            var sach = db.KHACHHANGs.SingleOrDefault(n => n.MaKH == id);
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sach);
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirm(int id, FormCollection f)
        {
            var sach = db.KHACHHANGs.SingleOrDefault(n => n.MaKH == id);
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            
            //Xóa sách
            db.KHACHHANGs.DeleteOnSubmit(sach);
            db.SubmitChanges();

            return RedirectToAction("Index");
        }
    }
}