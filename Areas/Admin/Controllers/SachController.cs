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
    

    public class SachController : Controller
    {
        
        // GET: Admin/Sach

        dbSachOnlineDataContext db = new dbSachOnlineDataContext();

        
        public ActionResult Index(int ? page)
        {
            
            int iPageNum = (page ?? 1);
            int iPageSize = 7;
            return View(db.Books.ToList().OrderBy(n => n.BookID).ToPagedList(iPageNum,iPageSize));
        }
        [HttpGet]
        public ActionResult Create() 
        {
            //Lấy ds từ các table ChuDe, NhaXuatBan, Hiển thị tên, khi chọn sẽ lấy mã
            ViewBag.CategoryID = new SelectList(db.Categories.ToList().OrderBy(n => n.CategoryName), "CategoryID", "CategoryName");
            ViewBag.NhaXuatBanID = new SelectList(db.NhaXuatBans.ToList().OrderBy(n => n.NhaXuatBanName), "NhaXuatBanID", "NhaXuatBanName");
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Book sach, FormCollection f, HttpPostedFileBase fFileUpload)
        {
            //Ðưa dữ liệu vào DropDown
            ViewBag.CategoryID = new SelectList(db.Categories.ToList().OrderBy(n => n.CategoryName), "CategoryID", "CategoryName");
            ViewBag.NhaXuatBanID = new SelectList(db.NhaXuatBans.ToList().OrderBy(n => n.NhaXuatBanName), "NhaXuatBanID", "NhaXuatBanName");
            if (fFileUpload == null)
            {
                //Nội dung thông báo yêu cầu chọn ảnh bìa
                ViewBag.Thongbao = "Hãy chọn ảnh bìa";
                //Lưu thông tin để khi load lại trang do yêu cầu chọn ảnh bìa sẽ hiển thị các thông tin này lên trang 
                ViewBag.TenSach = f["sTenSach"];
                ViewBag.Description = f["sDescription"];
                ViewBag.ViewCount = int.Parse(f["iViewCount"]);
                ViewBag.Price = decimal.Parse(f["mPrice"]);
                ViewBag.CategoryID = new SelectList(db.Categories.ToList().OrderBy(n => n.CategoryName), "CategoryID", "CategoryName", int.Parse(f["CategoryID"]));
                ViewBag.NhaXuatBanID = new SelectList(db.NhaXuatBans.ToList().OrderBy(n => n.NhaXuatBanName), "NhaXuatBanID", "NhaXuatBanName", int.Parse(f["NhaXuatBanID"]));
                return View();
            }
            else
            {
                if (ModelState.IsValid)
                {
                //Lấy tên file(Khai báo thư viện: System.IO)
                  var sFileName = Path.GetFileName(fFileUpload.FileName);
 
                 //Lấy đường dẫn lưu file
                 var path = Path.Combine(Server.MapPath("~/Images"), sFileName);
                 //Kiểm tra ảnh bìa đã tồn taji chưa để lưu lên thư mục
                 if (!System.IO.File.Exists(path))
                    {
                        fFileUpload.SaveAs(path);
                    }
                    //Lưu Sach vào csdl

                    sach.Title = f["sTenSach"];
                    sach.Description = f["sDescription"];
                    sach.Images = sFileName;
                    sach.Published = Convert.ToDateTime(f["dPublished"]);
                    sach.ViewCount = int.Parse(f["iViewCount"]);
                    sach.Price = decimal.Parse(f["mPrice"]);
                    sach.CategoryID = int.Parse(f["CategoryID"]);
                    sach.NhaXuatBanID = int.Parse(f["NhaXuatBanID"]);
                    db.Books.InsertOnSubmit(sach);
                    db.SubmitChanges();
                    //Về lại trang Quản lý mỹ phẩm
                    return RedirectToAction("Index");
                }
                return View();
            }
        }

        public ActionResult Details(int id)
        {
            var sach = db.Books.SingleOrDefault(n => n.BookID == id);
            if(sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sach);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var sach = db.Books.SingleOrDefault(n => n.BookID == id);
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
            var sach = db.Books.SingleOrDefault(n => n.BookID == id);
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            var ctdh = db.CHITIETDATHANGs.Where(ct => ct.BookID == id);
            if (ctdh.Count() > 0)
            {
                //Nội dung sẽ hiển thị khi sách cần xóa đã có trong table CHITIETDATHANG
                ViewBag.ThongBao = "Sách này đang có trong bảng Chi tiết đặt hàng <br>" + "Nếu muốn xóa thì phải xóa hết mã sách này trong bảng chi tiết đặt hàng";
                return View(sach);
            }
            //Xóa hết thông tin của cuốn sách trong table VietSach trước khi xóa sách này
            var vietsach = db.VIETSACHes.Where(vs => vs.BookID == id).ToList();
            if (vietsach != null)
            {
                db.VIETSACHes.DeleteAllOnSubmit(vietsach);
                db.SubmitChanges();
            }
            //Xóa sácmỹ phẩm
            db.Books.DeleteOnSubmit(sach);
            db.SubmitChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var sach = db.Books.SingleOrDefault(n => n.BookID == id);
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            //Hiển thị danh sách chủ đề và nhà xuất bản đồng thời chọn chủ đề và nhà xuất bản của cuốn hiện tại
            ViewBag.CategoryID = new SelectList(db.Categories.ToList().OrderBy(n => n.CategoryName), "CategoryID", "CategoryName", sach.CategoryID);
            ViewBag.NhaXuatBanID = new SelectList(db.NhaXuatBans.ToList().OrderBy(n => n.NhaXuatBanName), "NhaXuatBanID", "NhaXuatBanName", sach.NhaXuatBanID);
            return View(sach);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(FormCollection f, HttpPostedFileBase fFileUpload)
        {
            var sach = db.Books.SingleOrDefault(n => n.BookID == int.Parse(f["iBookID"]));
            ViewBag.CategoryID = new SelectList(db.Categories.ToList().OrderBy(n => n.CategoryName), "CategoryID", "CategoryName",sach.CategoryID);
            ViewBag.NhaXuatBanID = new SelectList(db.NhaXuatBans.ToList().OrderBy(n => n.NhaXuatBanName), "NhaXuatBanID", "NhaXuatBanName",sach.NhaXuatBanID);
            if (ModelState.IsValid)
            {
                if (fFileUpload != null)//Kiểm tra để xác nhận cho thay đổi ảnh bìa
                {
                    //Lấy tên file (Khai báo thư viện :System>IO)
                    var sFileName = Path.GetFileName(fFileUpload.FileName);
                    //Lấy đường dẫn lưu file
                    var path = Path.Combine(Server.MapPath("~/Images"), sFileName);
                    //Kiểm tra File đã tồn tại hay chưa
                    if (!System.IO.File.Exists(path))
                    {
                        fFileUpload.SaveAs(path);
                    }
                    sach.Images = sFileName;
                }

                //Lưu sách vào cơ sở dữ liệu
                sach.Title = f["sTitle"];
                sach.Description = f["sDescription"];

                sach.Published = Convert.ToDateTime(f["dPublished"]);
                sach.ViewCount = int.Parse(f["iViewCount"]);
                sach.Price = decimal.Parse(f["mPrice"]);
                sach.CategoryID = int.Parse(f["CategoryID"]);
                sach.NhaXuatBanID = int.Parse(f["NhaXuatBanID"]);

                db.SubmitChanges();
                //Về lại trang Quản lý sách
                return RedirectToAction("Index");
            }
            return View(sach);
        }
    }
}