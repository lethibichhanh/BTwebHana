using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SachOnline.Models
{
    public class GioHang
    {
        dbSachOnlineDataContext db = new dbSachOnlineDataContext();

        public int iMaSach { get; set; }
        public string sTenSach { get; set; }
        public string sAnhBia { get; set; }

        public double dDongia { get; set; }

        public int iSoLuong { get; set; }
        public double dThanhTien
        {
            get { return iSoLuong * dDongia; }
        }

        //Khởi tạo giỏ hàng với mã sách truyền vào với số lượng mặc định là 1
        public GioHang(int ms)
        {
            iMaSach = ms;
            Book s = db.Books.SingleOrDefault(n => n.BookID == iMaSach);
            sTenSach = s.Title;
            sAnhBia = s.Images;
            dDongia = double.Parse(s.Price.ToString());
            iSoLuong = 1;
        }

    }
}