using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SachOnline.Models;
using PagedList;
using PagedList.Mvc;

namespace SachOnline.Controllers
{
    public class SachOnlineController : Controller
    {
        // GET: SachOnline
        //Tao đối tượng chứa csdl
        dbSachOnlineDataContext data = new dbSachOnlineDataContext();

        //Lấy ra sách cập nhật gần đây

            private List<Book> LaySachMoi(int count)
        {
            return data.Books.OrderByDescending(a => a.Published).Take(count).ToList();
        }
        public ActionResult Index( int ?page)
        {
            
            //return View(listSachMoi);
            
            //Tạo biến quy định số sản phẩm trên mỗi trang
            int iSize = 3;
            //Tạo biến số trang
            int iPageNum = (page ?? 1);
            var listSachMoi = LaySachMoi(9);
            return View(listSachMoi.ToPagedList(iPageNum, iSize));
        }

        public ActionResult ChuDePartial()
        {
            var listChuDe = from CategoryID in data.Categories select CategoryID;
            return PartialView(listChuDe);
        }
        
        public ActionResult _NhaXuatBanPartial()
        {

            var listNXB = from NhaXuatBanID in data.NhaXuatBans select NhaXuatBanID;
            return PartialView(listNXB);
        }
        public ActionResult SachTheoChuDe(int? iMaCD, int ? page)
        {
            // var sach = from s in data.Books where s.CategoryID == id select s;
            //return View(sach);
            ViewBag.CategoryID = iMaCD;
            //Tạo biến quy định số sản phẩm trên mỗi trang
            int iSize = 3;
            //Tạo biến số trang
            int iPageNum = (page ?? 1);
            var sach = from s in data.Books where s.CategoryID == iMaCD select s;
            return View(sach.ToPagedList(iPageNum, iSize));
        }
       

        public ActionResult SachTheoNhaXuatBan(int? iMaNXB, int ? page)
        {
            //var sach = from s in data.Books where s.NhaXuatBanID == id select s;
            //return View(sach);
            ViewBag.NhaXuatBanID = iMaNXB;
            //Tạo biến quy định số sản phẩm trên mỗi trang
            int iSize = 3;
            //Tạo biến số trang
            int iPageNum = (page ?? 1);
            var sach = from s in data.Books where s.NhaXuatBanID == iMaNXB select s;
            return View(sach.ToPagedList(iPageNum, iSize));

        }
        private List<Book> LaySachBanNhieu(int count)
        {
            return data.Books.OrderByDescending(a => a.ViewCount).Take(count).ToList();
        }
        public ActionResult SachBanNhieuPartial()
        {
            var listSachBanNhieu = LaySachBanNhieu(4);
            return View(listSachBanNhieu);
        }
        public ActionResult ChiTietSach(int id)
        {
            var sach = from s in data.Books where s.BookID == id select s;
            return View(sach.Single());
        }
        public ActionResult LoginLogout()
        {
            return PartialView("LoginLogout");
        }

        public ActionResult Timkiem(string searchString, int? page)
        {
            int iSize = 8;
            //Tạo biến số trang
            int iPageNum = (page ?? 1);


            var links = from l in data.Books
                        select l;

            if (!String.IsNullOrEmpty(searchString))
            {
                links = links.Where(s => (s.Title.Contains(searchString)) || (s.Category.CategoryName.Contains(searchString)) || (s.NhaXuatBan.NhaXuatBanName.Contains(searchString)));
            }
            return View(links.ToPagedList(iPageNum, iSize));

        }


    }
}