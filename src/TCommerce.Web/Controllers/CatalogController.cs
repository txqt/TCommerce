﻿using Microsoft.AspNetCore.Mvc;
using TCommerce.Core.Interface;
using TCommerce.Web.Models.Catalog;
using TCommerce.Web.PrepareModelServices;

namespace TCommerce.Web.Controllers
{
    public class CatalogController : BaseController
    {
        private readonly ICatalogModelService _prepareCatalogModel;
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;

        public CatalogController(ICatalogModelService prepareCategoryModel, ICategoryService categoryService, IManufacturerService manufacturerService)
        {
            _prepareCatalogModel = prepareCategoryModel;
            _categoryService = categoryService;
            _manufacturerService = manufacturerService;
        }

        public IActionResult Index()
        {
            return View();
        }
        #region Category
        public virtual async Task<IActionResult> Category(int id, CatalogProductsCommand command)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);

            return View(await _prepareCatalogModel.PrepareCategoryModelAsync(category, command));
        }
        public virtual async Task<IActionResult> GetCategoryProducts(int categoryId, CatalogProductsCommand command)
        {
            var category = await _categoryService.GetCategoryByIdAsync(categoryId);

            if (category is null || category.Deleted)
                return InvokeHttp404();

            var model = await _prepareCatalogModel.PrepareCategoryProductsModelAsync(category, command);

            return PartialView("_CatalogProducts", model);
        }
        #endregion

        #region Manufacturer
        public virtual async Task<IActionResult> Manufacturer(int id, CatalogProductsCommand command)
        {
            var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(id);

            return View(await _prepareCatalogModel.PrepareManufacturerModelAsync(manufacturer, command));
        }
        public virtual async Task<IActionResult> GetManufacturerProducts(int manufacturerId, CatalogProductsCommand command)
        {
            var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(manufacturerId);

            if (manufacturer is null || manufacturer.Deleted)
                return InvokeHttp404();

            var model = await _prepareCatalogModel.PrepareManufacturerProductsModelAsync(manufacturer, command);

            return PartialView("_CatalogProducts", model);
        }
        #endregion
        public virtual async Task<IActionResult> Search(SearchModel model, CatalogProductsCommand command)
        {
            if (model == null)
                model = new SearchModel();

            model = await _prepareCatalogModel.PrepareSearchModelAsync(model, command);

            if (model.CatalogProductsModel.WarningMessage is not null)
            {
                SetStatusMessage(model.CatalogProductsModel.WarningMessage);
            }

            return View(model);
        }

        public async Task<IActionResult> SearchProducts(SearchModel searchModel, CatalogProductsCommand command)
        {
            if (searchModel == null)
                searchModel = new SearchModel();

            var model = await _prepareCatalogModel.PrepareSearchProductsModelAsync(searchModel, command);

            if(model.WarningMessage is not null)
            {
                SetStatusMessage(model.WarningMessage);
            }

            return PartialView("_CatalogProducts", model);
        }
    }
}