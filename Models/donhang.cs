using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;



namespace SachOnline.Models
{
    public class donhang
    {
        dbSachOnlineDataContext db = new dbSachOnlineDataContext();
        public int MaDonHang { get; set; }
        public int? MaKH { get; set; }
         public string HoTen { get;  set; }
        public DateTime? Ngaydat { get; set; }
        public DateTime? Ngaygiao { get; set; }
        public bool Dathanhtoan { get; set; }
        public bool Tinhtranggiaohang { get; set; }
        public int? Soluong { get; set; }
        
        public decimal? Dongia { get; set; }
        public int? BookID { get; set; }
        public string Title { get; set; }
    }
}