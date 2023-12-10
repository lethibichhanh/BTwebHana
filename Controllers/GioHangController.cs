using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc ;
using System.Security.Cryptography;
using System.Text;
using static System.Console;
using SachOnline.Models;


namespace SachOnline.Controllers
{
    public class GioHangController : Controller
    {
        dbSachOnlineDataContext db = new dbSachOnlineDataContext();
        // GET: GioHang
        //GET: DatHang

        public List<GioHang> LayGioHang()
        {
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang == null)
            {
                //Khởi tạo giỏ hàng(giỏ hàng chưa tồn tại)
                lstGioHang = new List<GioHang>();
                Session["GioHang"] = lstGioHang;

            }
            return lstGioHang;
        }
        //Thêm sản phẩm vào giỏ
        public ActionResult ThemGioHang(int ms, string url)
        {
            //Lấy giỏ hàng hiện tại
            List<GioHang> lstGioHang = LayGioHang();
            //Kiểm tra nếu sản phẩm chưa có trong giỏ hàng thì thêm vào, nếu có thì tăng số lượng
            GioHang sp = lstGioHang.Find(n => n.iMaSach == ms);
            if (sp == null)
            {
                sp = new GioHang(ms);
                lstGioHang.Add(sp);

            }
            else
            {
                sp.iSoLuong++;
            }

            return Redirect(url);
        }

        //Tính tổng số lượng
        // private int TongSoLuong()
        // {
        //     int iTongSoLuong = 0;
        //     List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
        //     if (lstGioHang != null)
        //      {
        //         iTongSoLuong = lstGioHang.Sum(n => n.iSoLuong);
        //    }
        //      return iTongSoLuong;
        //  }

        //Tính tổng tiền
        // private double TongTien()
        // {
        //    double dTongTien = 0;
        //   List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
        //   if (lstGioHang != null)
        //   {
        //      dTongTien = lstGioHang.Sum(n => n.dThanhTien);
        // }
        //  return dTongTien;
        // }

        //Action trả về view GioHang
        // public ActionResult GioHang()
        //  {
        //Lấy giỏ hàng hiện tại
        //     List<GioHang> lstGioHang = LayGioHang();

        //     if (lstGioHang.Count==0)
        //     {
        //        return RedirectToAction("Index", "SachOnline");
        //   }
        //   ViewBag.TongSoLuong = TongSoLuong();
        //   ViewBag.TongTien = TongTien();
        //   return View(lstGioHang);
        // }
        //Tính tổng số lượng
        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                iTongSoLuong = lstGioHang.Sum(n => n.iSoLuong);
            }
            return iTongSoLuong;
        }

        //Tính tổng tiền
        private double TongTien()
        {
            double dTongTien = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                dTongTien = lstGioHang.Sum(n => n.dThanhTien);
            }
            return dTongTien;
        }

        //Action trả về view GioHang
        public ActionResult GioHang()
        {
            //Lấy giỏ hàng hiện tại
            List<GioHang> lstGioHang = LayGioHang();

            if (lstGioHang.Count == 0)
            {
                return RedirectToAction("Index", "SachOnline");
            }
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(lstGioHang);
        }

        public ActionResult GioHangPartial()
        {
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            //return PartialView("GioHangPartial");
            //List<GioHang> lstGioHang = LayGioHang();
            return PartialView("GioHangPartial");
        }

        //Xóa sản phẩm khỏi giỏ hàng
        public ActionResult XoaSPKhoiGioHang(int iMaSach)
        {
            List<GioHang> lstGioHang = LayGioHang();
            //Kiểm tra Sách đã có trong Session["GioHang"]
            GioHang sp = lstGioHang.SingleOrDefault(n => n.iMaSach == iMaSach);
            //Xóa sp khỏi giỏ hàng
            if (sp != null) {
                lstGioHang.RemoveAll(n => n.iMaSach == iMaSach);
                if (lstGioHang.Count == 0)
                {
                    return RedirectToAction("Index", "SachOnlie");
                }
            }
            return RedirectToAction("GioHang");
        }

        //Cập nhật giỏ hàng

        public ActionResult CapNhatGioHang(int iMaSach, FormCollection f)

        {

            List<GioHang> lstGioHang = LayGioHang();

            GioHang sp = lstGioHang.SingleOrDefault(n => n.iMaSach == iMaSach);

            // Nếu tồn tại thì cho sửa số lượng if (sp != null)

            sp.iSoLuong = int.Parse(f["txtSoLuong"].ToString());

            return RedirectToAction("GioHang");
        }

        //Xóa giỏ hàng

        public ActionResult XoaGioHang()

        {

            List<GioHang> lstGioHang = LayGioHang();

            lstGioHang.Clear();

            return RedirectToAction("Index", "SachOnline");
        }

        [HttpGet]

        public ActionResult DatHang()
        {
            //Kiểm tra đăng nhập chưa 
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")

            { return Redirect("~/User/Dangnhap?id=2");

            }
            //Kiểm tra có hàng trong giỏ chưa 
            if (Session["GioHang"] == null)

            { return RedirectToAction("Index", "SachOnline"); }

            //Lấy hàng từ Session

            List<GioHang> lstGioHang = LayGioHang();
            ViewBag.TongSoLuong = TongSoLuong();

            ViewBag.TongTien = TongTien();
            return View(lstGioHang);
        }

        [HttpPost]
        public ActionResult DatHang(FormCollection f)
        {
            //Thêm đơn hàng
            DONDATHANG ddh = new DONDATHANG();
            KHACHHANG kh = (KHACHHANG)Session["Taikhoan"];
            List<GioHang> lstGioHang = LayGioHang();
            ddh.MaKH = kh.MaKH;
            ddh.Ngaydat = DateTime.Now;
            var NgayGiao = string.Format("{0:MM//dd/yyyy}", f["Ngaygiao"]);
            ddh.Ngaygiao = DateTime.Parse(NgayGiao);
            ddh.Tinhtranggiaohang = 1;
            ddh.Dathanhtoan = false;
            db.DONDATHANGs.InsertOnSubmit(ddh);
            db.SubmitChanges();

            //Thêm chi tiết đơn hàng
            foreach (var item in lstGioHang)
            {
                CHITIETDATHANG ctdh = new CHITIETDATHANG();
                ctdh.MaDonHang = ddh.MaDonHang;
                ctdh.BookID = item.iMaSach;
                ctdh.Soluong = item.iSoLuong;
                ctdh.Dongia = (decimal)item.dDongia;
                db.CHITIETDATHANGs.InsertOnSubmit(ctdh);
            }
            db.SubmitChanges();
            Session["GioHang"] = null;
            return RedirectToAction("XacNhanDonHang", "GioHang");
        }

        //Thêm phương thức xác nhận đơn hàng
        public ActionResult XacNhanDonHang()
        {
            return View();
        }
       
        
    }
   }












