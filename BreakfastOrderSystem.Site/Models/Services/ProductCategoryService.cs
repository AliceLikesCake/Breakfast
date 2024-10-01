using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BreakfastOrderSystem.Site.Models.Dtos;
using BreakfastOrderSystem.Site.Models.EFModels;
using BreakfastOrderSystem.Site.Models.Repositories;

namespace BreakfastOrderSystem.Site.Models.Services
{
    public class ProductCategoryService
    {

        private ProductCategoryRepositories _repo;

        public ProductCategoryService()
        {
            _repo = new ProductCategoryRepositories();
        }
        public void CreateProductCategory(ProductCategoryDto dto)
        {
            // precondition checks
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (_repo.IsExist(dto.Name)) throw new ArgumentException("類別名字已存在");

            _repo.CreateProductCategory(dto);
        }

        public void UpdateProductCategory(ProductCategoryDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            // 檢查分類名稱是否已存在
            var categoryInDb = _repo.GetByName(dto.Name);
            if (categoryInDb != null && categoryInDb.Id != dto.Id)
                throw new ArgumentException("類別名字已存在");

            // 調用 Repository 更新分類
            _repo.UpdateProductCategory(dto);
        }

    }
}