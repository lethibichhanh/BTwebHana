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
    public class DonHangController : Controller
    {
        dbSachOnlineDataContext db = new dbSachOnlineDataContext();
        // GET: Admin/DonHang
        public ActionResult Index(int? page)
        {
            int iPageNum = page ?? 1;
            int iPageSize = 7;

            var ketqua = (from ctdh in db.CHITIETDATHANGs
                          join ctdhdh in db.DONDATHANGs on ctdh.MaDonHang equals ctdhdh.MaDonHang
                          join kh in db.KHACHHANGs on ctdhdh.MaKH equals kh.MaKH
                          join ms in db.Books on ctdh.BookID equals ms.BookID
                          select new donhang
                          {
                              MaDonHang = ctdh.MaDonHang,
                              MaKH = ctdhdh.MaKH,
                              HoTen = kh.HoTen,
                              
                              Ngaydat = ctdhdh.Ngaydat,
                              Ngaygiao = ctdhdh.Ngaygiao,
                              Soluong = ctdh.Soluong,
                              BookID = ms.BookID,
                              Title = ms.Title,
                              Dongia = ctdh.Dongia
                          }).OrderBy(d => d.MaDonHang).ToPagedList(iPageNum, iPageSize);

            return View(ketqua);
        }


        public ActionResult Kimquyen()
        {
            var ketqua = (from ctdh in db.CHITIETDATHANGs
                          join ctdhdh in db.DONDATHANGs
                          on ctdh.MaDonHang equals ctdhdh.MaDonHang
                          join kh in db.KHACHHANGs
                          on ctdhdh.MaKH equals kh.MaKH
                          select new donhang
                          {
                              MaDonHang = ctdh.MaDonHang,
                              MaKH = ctdhdh.MaKH,
                              HoTen = kh.HoTen,
                              Ngaydat = ctdhdh.Ngaydat,
                              Ngaygiao = ctdhdh.Ngaygiao,
                              Soluong = ctdh.Soluong,
                              Dongia = ctdh.Dongia
                          }).ToList();


            return View(ketqua);

        }
        public ActionResult Details(int id)
        {
            var order = db.DONDATHANGs.SingleOrDefault(n => n.MaDonHang == id);

            if (order == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            // Fetch details for the specific order
            var orderDetails = (from ctdh in db.CHITIETDATHANGs
                                join ctdhdh in db.DONDATHANGs on ctdh.MaDonHang equals ctdhdh.MaDonHang
                                join kh in db.KHACHHANGs on ctdhdh.MaKH equals kh.MaKH
                                join ms in db.Books on ctdh.BookID equals ms.BookID
                                where ctdh.MaDonHang == id
                                select new donhang
                                {
                                    MaDonHang = ctdh.MaDonHang,
                                    MaKH = ctdhdh.MaKH,
                                    HoTen = kh.HoTen,
                                    Ngaydat = ctdhdh.Ngaydat,
                                    Ngaygiao = ctdhdh.Ngaygiao,
                                    Soluong = ctdh.Soluong,
                                    BookID = ms.BookID,
                                    Title = ms.Title,
                                    Dongia = ctdh.Dongia
                                }).ToList();

            // Fetch customer name for the specific order
            var customerName = db.KHACHHANGs.Where(kh => kh.MaKH == order.MaKH).Select(kh => kh.HoTen).FirstOrDefault();

            // Pass the order, customer name, and order details to the view
            var viewModel = new OrderDetailsViewModel
            {
                Order = order,
                customerName = customerName,
                OrderDetails = orderDetails
            };

            return View(viewModel);
        }
        [HttpGet]
        public ActionResult Create()
        {
            // Assuming you want to create a new order with details based on an existing order
            // You need to provide the order ID for the existing order
            int existingOrderId = 1; // Change this to the ID of the existing order

            // Fetch the existing order
            var existingOrder = db.DONDATHANGs.SingleOrDefault(n => n.MaDonHang == existingOrderId);

            if (existingOrder == null)
            {
                // Handle the case where the existing order is not found
                Response.StatusCode = 404;
                return null;
            }

            // Fetch details for the existing order
            var existingOrderDetails = (from ctdh in db.CHITIETDATHANGs
                                         join ms in db.Books on ctdh.BookID equals ms.BookID
                                         where ctdh.MaDonHang == existingOrderId
                                         select new donhang
                                         {
                                             MaDonHang = ctdh.MaDonHang,
                                             MaKH = existingOrder.MaKH,
                                             HoTen = existingOrder.KHACHHANG.HoTen, // Use navigation property
                                             Ngaydat = existingOrder.Ngaydat,
                                             Ngaygiao = existingOrder.Ngaygiao,
                                             Soluong = ctdh.Soluong,
                                             BookID = ms.BookID,
                                             Title = ms.Title,
                                             Dongia = ctdh.Dongia
                                         }).ToList();

            // Populate properties of the new order with details
            var newOrder = new DONDATHANG
            {
                Dathanhtoan = false, // Set default value, you may change this as needed
                Tinhtranggiaohang = 0, // Set default value, you may change this as needed
                Ngaydat = DateTime.Now, // Set default value, you may change this as needed
                Ngaygiao = DateTime.Now, // Set default value, you may change this as needed
                MaKH = existingOrder.MaKH // Use the customer ID from the existing order
            };

            // Pass the existing order, customer name, and order details to the view
            var viewModel = new OrderDetailsViewModel
            {
                Order = newOrder,
                customerName = existingOrder.KHACHHANG.HoTen,
               // Use navigation property
                OrderDetails = existingOrderDetails
            };

            // Provide dropdown lists or other necessary data for the view if needed
            ViewBag.Title = new SelectList(db.Books.ToList().OrderBy(n => n.Title), "BookID", "Title");

            ViewBag.HoTen = new SelectList(db.KHACHHANGs.ToList(), "MaKH", "HoTen");

            return View(viewModel);
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(OrderDetailsViewModel viewModel)
        {
            try
            {
                // Add the new order to the database
                db.DONDATHANGs.InsertOnSubmit(viewModel.Order);
                db.SubmitChanges();

                // Process order details and add them to the database
                foreach (var detail in viewModel.OrderDetails)
                {
                    // Create a new CHITIETDATHANG object and set its properties
                    var newDetail = new CHITIETDATHANG
                    {
                        // Explicitly convert nullable int? to int, providing a default value if MaDonHang is null
                        MaDonHang = viewModel.Order.MaDonHang , // Replace 0 with your desired default value
                        BookID = (int)detail.BookID,
                        Soluong = detail.Soluong,
                        Dongia = detail.Dongia
                    };

                    // Add the new detail to the database
                    db.CHITIETDATHANGs.InsertOnSubmit(newDetail);
                }

                // Submit changes to the database
                db.SubmitChanges();

                // Redirect to the list of orders
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Handle exceptions here
                // You may want to log the exception or show an error message
                return View(viewModel); // Return the view with the same data if an error occurs
            }
        }



        //[HttpGet]
        // public ActionResult Edit(int id)
        // {
        //    var order = db.DONDATHANGs.SingleOrDefault(n => n.MaDonHang == id);


        //   if (order == null)
        //  {
        //       Response.StatusCode = 404;
        //      return null;
        //  }

        // Fetch details for the specific order
        //  var orderDetails = (from ctdh in db.CHITIETDATHANGs
        //          join ctdhdh in db.DONDATHANGs on ctdh.MaDonHang equals ctdhdh.MaDonHang
        //          join kh in db.KHACHHANGs on ctdhdh.MaKH equals kh.MaKH
        //          join ms in db.Books on ctdh.BookID equals ms.BookID
        //          where ctdh.MaDonHang == id
        //          select new donhang
        //         {
        //            MaDonHang = ctdh.MaDonHang,
        //            MaKH = ctdhdh.MaKH,
        //            HoTen = kh.HoTen,
        //            Ngaydat = ctdhdh.Ngaydat,
        //           Ngaygiao = ctdhdh.Ngaygiao,
        //           Soluong = ctdh.Soluong,
        //          BookID = ms.BookID,
        //          Title = ms.Title,
        //          Dongia = ctdh.Dongia
        //      }).ToList();

        // Fetch customer name for the specific order
        // var customerName = db.KHACHHANGs.Where(kh => kh.MaKH == order.MaKH).Select(kh => kh.HoTen).FirstOrDefault();

        // Pass the order, customer name, and order details to the view
        //  var viewModel = new OrderDetailsViewModel
        // {
        //    Order = order,
        //   customerName = customerName,
        //   OrderDetails = orderDetails
        // };

        // return View(viewModel);
        // }


        // [HttpPost]
        // [ValidateInput(false)]
        // public ActionResult Edit(OrderDetailsViewModel viewModel)
        // {
        //    try
        //   {
        // Update the existing order in the database
        //      var existingOrder = db.DONDATHANGs.SingleOrDefault(n => n.MaDonHang == viewModel.Order.MaDonHang);





        //    if (existingOrder == null)
        //     {
        //        Response.StatusCode = 404;
        //        return null;
        //   }

        // Update properties of the existing order
        //   existingOrder.Dathanhtoan = viewModel.Order.Dathanhtoan;
        //   existingOrder.Tinhtranggiaohang = viewModel.Order.Tinhtranggiaohang;
        // existingOrder.Ngaydat = viewModel.Order.Ngaydat;
        //  existingOrder.Ngaygiao = viewModel.Order.Ngaygiao;

        // Update order details
        // foreach (var detail in viewModel.OrderDetails)
        // {
        //     var existingDetail = db.CHITIETDATHANGs.SingleOrDefault(d => d.MaDonHang == viewModel.Order.MaDonHang );

        //     if (existingDetail != null)
        //    {
        // Update properties of the existing order detail
        //     existingDetail.Soluong = detail.Soluong;
        //     existingDetail.Dongia = detail.Dongia;
        // }
        //  else
        //  {
        // Handle the case where the order detail doesn't exist
        // You may choose to create a new order detail or log an error
        //  }
        // }

        // Submit changes to the database
        // db.SubmitChanges();

        // Redirect to the list of orders
        // return RedirectToAction("Index");
        // }
        // catch (Exception ex)
        // {
        // Handle exceptions here
        // You may want to log the exception or show an error message
        //  return View(viewModel); // Return the view with the same data if an error occurs
        // }
        // }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var sach = db.DONDATHANGs.SingleOrDefault(n => n.MaDonHang == id);
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            //Hiển thị danh sách chủ đề và nhà xuất bản đồng thời chọn chủ đề và nhà xuất bản của cuốn hiện tại
            ViewBag.MaKH = new SelectList(db.KHACHHANGs.ToList().OrderBy(n => n.HoTen), "MaKH", "HoTen", sach.MaKH);
            
            return View(sach);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(FormCollection f, HttpPostedFileBase fFileUpload)
        {
            var sach = db.DONDATHANGs.SingleOrDefault(n => n.MaDonHang == int.Parse(f["iMaDonHang"]));
            ViewBag.MaKH = new SelectList(db.KHACHHANGs.ToList().OrderBy(n => n.HoTen), "MaKH", "HoTen", sach.MaKH);
            
            if (ModelState.IsValid)
            {
                

                //Lưu sách vào cơ sở dữ liệu
                sach.MaDonHang = int.Parse(f["iMaDonHang"]);
                

                sach.Ngaydat = Convert.ToDateTime(f["iNgaydat"]);
                sach.Ngaygiao = Convert.ToDateTime(f["iNgaygiao"]);
               sach.Dathanhtoan =bool.Parse(f["iDathanhtoan"]);
                sach.Tinhtranggiaohang = int.Parse(f["iTinhtranggiaohang"]);
                
                db.SubmitChanges();
                //Về lại trang Quản lý mỹ phẩm
                return RedirectToAction("Index");
            }
            return View(sach);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var sach = db.DONDATHANGs.SingleOrDefault(n => n.MaDonHang == id);
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
            var sach = db.DONDATHANGs.SingleOrDefault(n => n.MaDonHang == id);
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            var ctdh = db.CHITIETDATHANGs.Where(ct => ct.MaDonHang == id);
            if (ctdh.Count() > 0)
            {
                //Nội dung sẽ hiển thị khi sách cần xóa đã có trong table CHITIETDATHANG
                ViewBag.ThongBao = "Đơn này đang có trong bảng Chi tiết đặt hàng <br>" + "Nếu muốn xóa thì phải xóa hết mã đơn hàng này trong bảng chi tiết đặt hàng";
                var vietsach = db.CHITIETDATHANGs.Where(vs => vs.MaDonHang == id).ToList();
                 if (vietsach != null)
                 {
                    db.CHITIETDATHANGs.DeleteAllOnSubmit(vietsach);
                   db.SubmitChanges();
                }
                return View(sach);
            }
            //Xóa hết thông tin của cuốn sách trong table VietSach trước khi xóa sách này
            //var vietsach = db.CHITIETDATHANGs.Where(vs => vs.MaDonHang == id).ToList();
           // if (vietsach != null)
           // {
            //    db.CHITIETDATHANGs.DeleteAllOnSubmit(vietsach);
             //   db.SubmitChanges();
            //}
            //Xóa mỹ phẩm
            db.DONDATHANGs.DeleteOnSubmit(sach);
            db.SubmitChanges();

            return RedirectToAction("Index");
        }


    }
}