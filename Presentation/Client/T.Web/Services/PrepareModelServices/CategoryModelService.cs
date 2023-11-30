using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using T.Library.Model.Catalogs;
using T.Library.Model.Interface;
using T.Web.Areas.Admin.Models;

namespace T.Web.Services.PrepareModelServices
{
    public interface ICategoryModelService
    {
        Task<CategoryModel> PrepareCategoryModelAsync(CategoryModel model, Category category);
    }
    public class CategoryModelService : ICategoryModelService
    {
        private readonly IMapper _mapper;
        private readonly ICategoryService _categoryService;

        public CategoryModelService(IMapper mapper, ICategoryService categoryService)
        {
            _mapper = mapper;
            _categoryService = categoryService;
        }

        public async Task<CategoryModel> PrepareCategoryModelAsync(CategoryModel model, Category category)
        {
            if (category is not null)
            {
                model ??= new CategoryModel()
                {
                    Id = category.Id
                };
                _mapper.Map(category, model);
            }

            var listcategory = (await _categoryService.GetAllCategoryAsync());
            listcategory.Insert(0, new Category()
            {
                Name = "Không có danh mục cha",
                Id = -1
            });
            model.AvailableCategories = (listcategory).Select(productAttribute => new SelectListItem
            {
                Text = productAttribute.Name,
                Value = productAttribute.Id.ToString()
            }).ToList();

            return model;
        }
    }
}
