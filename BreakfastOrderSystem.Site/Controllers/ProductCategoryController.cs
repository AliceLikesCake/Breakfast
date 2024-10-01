using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BreakfastOrderSystem.Site.Models.EFModels;
using System.Data.Entity;
using BreakfastOrderSystem.Site.Models.ViewModels;
using System.IO;
using BreakfastOrderSystem.Site.Models.Services;
using BreakfastOrderSystem.Site.Models.Dtos;
namespace BreakfastOrderSystem.Site.Controllers
{
    public class ProductCategoryController : Controller
    {
        // GET: 秀出商品分類
        public ActionResult Index()
        {
            var db = new AppDbContext();
            var data = db.ProductCategories
            .AsNoTracking()
       
            
            .Select(p => new ProductCategoryVm
            {
                Id = p.Id,
                Name = p.Name,
                Image= p.Image,

                DisplayOrder = p.DisplayOrder,
            })
            .OrderBy(p => p.DisplayOrder ?? int.MaxValue)
            .ToList();//逼迫它在TOLIST時就查詢

            return View(data);
        }

        private readonly ProductCategoryService _productCategoryService;


        public ProductCategoryController()
        {
            _productCategoryService = new ProductCategoryService();
        }

       //新增商品分類
        [HttpPost]
        public JsonResult Create(ProductCategoryVm model, HttpPostedFileBase Image)
        {
            if (ModelState.IsValid)
            {
                // 構建 DTO 並使用 AutoMapper 進行映射
                var productCategoryDto = new ProductCategoryDto
                {
                    Name = model.Name,
                    DisplayOrder = model.DisplayOrder
                };

                // 圖片處理（如果需要保存到磁盤或雲端）
                if (Image != null && Image.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(Image.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/images/"), fileName);
                    Image.SaveAs(path); // 保存圖片到本地
                    productCategoryDto.Image = "/Content/images/" + fileName; // 設置圖片路徑
                }

                try
                {
                    // 調用服務層的 CreateProductCategory 方法
                    _productCategoryService.CreateProductCategory(productCategoryDto);

                    // 返回成功訊息
                    return Json(new { success = true, message = "類別已成功創建" });
                }
                catch (ArgumentException ex)
                {
                    // 返回錯誤訊息
                    return Json(new { success = false, message = "類別已存在" });
                }
            }

            // 返回驗證錯誤
            return Json(new { success = false, message = "表單驗證失敗" });
        }

        //編輯時依id尋找資料
        [HttpGet]
        public JsonResult Edit(int id)
        {
            var db = new AppDbContext();
            var productCategory = db.ProductCategories
                .Where(c => c.Id == id)
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.DisplayOrder,
                    c.Image
                })
                .FirstOrDefault();

            if (productCategory != null)
            {
                return Json(new { success = true, data = productCategory }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "找不到該商品類別" }, JsonRequestBehavior.AllowGet);
            }
        }

        // 更新商品類別
        [HttpPost]
        public ActionResult Update(ProductCategoryVm model, HttpPostedFileBase Image)
        {
            if (ModelState.IsValid)
            {
                // 使用 AutoMapper 將 ViewModel 轉換為 DTO
                var productCategoryDto = MvcApplication._mapper.Map<ProductCategoryDto>(model);

                if (model.DeleteImage)
                {
                    // 如果標記為刪除圖片，將 DTO 中的圖片設置為 null
                    productCategoryDto.Image = null;
                }
                else if (Image != null && Image.ContentLength > 0)
                {
                    // 如果有新圖片上傳，將路徑設置到 DTO 中
                    var fileName = Path.GetFileName(Image.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/images/"), fileName);
                    Image.SaveAs(path);
                    productCategoryDto.Image = "/Content/images/" + fileName; // 新圖片的路徑
                }

                try
                {
                    // 調用服務層進行更新操作
                    _productCategoryService.UpdateProductCategory(productCategoryDto);

                    // 返回成功訊息
                    return Json(new { success = true, message = "類別已成功更新" });
                }
                catch (ArgumentException ex)
                {
                    // 返回錯誤訊息
                    return Json(new { success = false, message = ex.Message });
                }
            }

            // 返回表單驗證失敗的錯誤訊息
            return Json(new { success = false, message = "表單驗證失敗" });
        }

        [HttpDelete]
        public JsonResult Delete(int id)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var productCategory = db.ProductCategories.Find(id);
                    if (productCategory == null)
                    {
                        return Json(new { success = false, message = "找不到該商品類別" });
                    }

                    db.ProductCategories.Remove(productCategory);
                    db.SaveChanges();

                    return Json(new { success = true, message = "類別已成功刪除" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "刪除過程中發生錯誤：" + ex.Message });
            }
        }


    }
}
