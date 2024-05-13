using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using T.Library.Model;
using T.Library.Model.Catalogs;
using T.Library.Model.Interface;
using T.Web.Models;
using T.Web.Models.Catalog;
using T.Web.Services.ManufacturerServices;
using T.Web.Services.PictureServices;
using T.Web.Services.ProductService;
using T.Web.Services.UrlRecordService;

namespace T.Web.Services.PrepareModelServices
{
    public interface ICatalogModelService
    {
        Task<CategoryModel> PrepareCategoryModelAsync(Category category, CatalogProductsCommand command, bool ignoreFeaturedProducts = false);
        Task<CatalogProductsModel> PrepareCategoryProductsModelAsync(Category category, CatalogProductsCommand command);
        Task<CategoryNavigationModel> PrepareCategoryNavigationModelAsync(int currentCategoryId);
    }
    public class CatalogModelService : ICatalogModelService
    {
        private readonly IMapper _mapper;
        private readonly IUrlRecordService _urlRecordService;
        private readonly ICategoryServiceCommon _categoryService;
        private readonly IPictureService _pictureService;
        private readonly IProductService _productService;
        private readonly IProductModelService _productModelFactory;
        private readonly IManufacturerService _manufacturerService;
        private static readonly char[] _separator = { ',', ' ' };

        public CatalogModelService(IMapper mapper, IUrlRecordService urlRecordService, ICategoryServiceCommon categoryService, IPictureService pictureService, IProductService productService, IProductModelService productModelFactory, IManufacturerService manufacturerService)
        {
            _mapper = mapper;
            _urlRecordService = urlRecordService;
            _categoryService = categoryService;
            _pictureService = pictureService;
            _productService = productService;
            _productModelFactory = productModelFactory;
            _manufacturerService = manufacturerService;
        }

        public virtual async Task<CategoryModel> PrepareCategoryModelAsync(Category category, CatalogProductsCommand command, bool ignoreFeaturedProducts = false)
        {
            ArgumentNullException.ThrowIfNull(category);

            ArgumentNullException.ThrowIfNull(command);

            var model = new CategoryModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                MetaKeywords = category.MetaKeywords,
                MetaDescription = category.MetaDescription,
                MetaTitle = category.MetaTitle,
                SeName = await _urlRecordService.GetSeNameAsync(category),
                CatalogProductsModel = await PrepareCategoryProductsModelAsync(category, command)
            };

            //subcategories
            model.SubCategories = (await Task.WhenAll((await _categoryService.GetAllCategoryAsync()).Where(x => x.ParentCategoryId == category.Id)
            .Select(async curCategory =>
            {
                var subCatModel = new CategoryModel.SubCategoryModel
                {
                    Id = curCategory.Id,
                    Name = curCategory.Name,
                    SeName = await _urlRecordService.GetSeNameAsync(curCategory),
                    Description = curCategory.Description
                };

                async Task<Library.Model.ViewsModel.PictureModel> GetPicture()
                {
                    var picture = await _pictureService.GetPictureByIdAsync(curCategory.PictureId);

                    var pictureModel = new Library.Model.ViewsModel.PictureModel();

                    if (picture is not null && !string.IsNullOrEmpty(picture.UrlPath))
                        pictureModel.ImageUrl = picture.UrlPath;

                    return pictureModel;
                }

                subCatModel.PictureModel = await GetPicture();

                return subCatModel;
            }))).ToList();


            //featured products
            if (!ignoreFeaturedProducts)
            {
                var featuredProducts = await _productService.GetCategoryFeaturedProductsAsync(category.Id);
                if (featuredProducts != null)
                    model.FeaturedProducts = (await Task.WhenAll(featuredProducts.Select(async curProduct =>
                    {
                        return await _productModelFactory.PrepareProductBoxModel(curProduct, null);
                    }))).ToList();
            }

            return model;
        }
        public virtual async Task<CatalogProductsModel> PrepareCategoryProductsModelAsync(Category category, CatalogProductsCommand command)
        {
            ArgumentNullException.ThrowIfNull(category);

            ArgumentNullException.ThrowIfNull(command);

            var model = new CatalogProductsModel
            {
                UseAjaxLoading = true
            };

            //sorting
            PrepareSortingOptions(model, command);

            PrepareViewModes(model, command);
            //page size
            await PreparePageSizeOptionsAsync(model, command, category.AllowCustomersToSelectPageSize,
                category.PageSizeOptions, category.PageSize);

            var categoryIds = new List<int> { category.Id };

            ////include subcategories
            //if (_catalogSettings.ShowProductsFromSubcategories)
            //    categoryIds.AddRange(await _categoryService.GetChildCategoryIdsAsync(category.Id, currentStore.Id));

            //price range
            PriceRangeModel selectedPriceRange = null;
            

            var orderString = string.Empty;
            switch (command.OrderBy)
            {
                case (int)ProductSortingEnum.Position:
                    orderString = "DisplayOrder asc";
                    break;
                case (int)ProductSortingEnum.PriceDesc:
                    orderString = "Price desc";
                    break;
                case (int)ProductSortingEnum.PriceAsc:
                    orderString = "Price asc";
                    break;
                case (int)ProductSortingEnum.NameDesc:
                    orderString = "Name desc";
                    break;
                case (int)ProductSortingEnum.CreatedOn:
                    orderString = "CreatedOnUtc desc";
                    break;
                case (int)ProductSortingEnum.NameAsc:
                default:
                    orderString = "Name asc";
                    break;
            }

            //products
            var products = await _productService.GetAll(new ProductParameters()
            {

                PageNumber = command.PageNumber,
                PageSize = command.PageSize,
                CategoryIds = categoryIds,
                ExcludeFeaturedProducts = false,
                PriceMin = selectedPriceRange?.From,
                PriceMax = selectedPriceRange?.To,
                ManufacturerIds = command.ManufacturerIds,
                OrderBy = orderString
            });

            await PrepareCatalogProductsAsync(model, products);

            return model;
        }

        public virtual void PrepareViewModes(CatalogProductsModel model, CatalogProductsCommand command)
        {
            model.AllowProductViewModeChanging = true;

            var viewMode = !string.IsNullOrEmpty(command.ViewMode)
                ? command.ViewMode
                : "3-cols";
            model.ViewMode = viewMode;
            if (model.AllowProductViewModeChanging)
            {
                //list
                model.AvailableViewModes.Add(new SelectListItem
                {
                    Text = "List",
                    Value = "list",
                    Selected = viewMode == "list"
                });
                //2-cols
                model.AvailableViewModes.Add(new SelectListItem
                {
                    Text = "2 Cols",
                    Value = "2-cols",
                    Selected = viewMode == "2-cols"
                });
                //3-cols
                model.AvailableViewModes.Add(new SelectListItem
                {
                    Text = "3 Cols",
                    Value = "3-cols",
                    Selected = viewMode == "3-cols"
                });
                //4-cols
                model.AvailableViewModes.Add(new SelectListItem
                {
                    Text = "4 Cols",
                    Value = "4-cols",
                    Selected = viewMode == "4-cols"
                });
            }
        }

        public virtual Task PreparePageSizeOptionsAsync(CatalogProductsModel model, CatalogProductsCommand command,
        bool allowCustomersToSelectPageSize, string pageSizeOptions, int fixedPageSize)
        {
            if (command.PageNumber <= 0)
                command.PageNumber = 1;

            model.AllowCustomersToSelectPageSize = true;
            if (allowCustomersToSelectPageSize && pageSizeOptions != null)
            {
                var pageSizes = pageSizeOptions.Split(_separator, StringSplitOptions.RemoveEmptyEntries);

                if (pageSizes.Any())
                {
                    // get the first page size entry to use as the default (category page load) or if customer enters invalid value via query string
                    if (command.PageSize <= 0 || !pageSizes.Contains(command.PageSize.ToString()))
                    {
                        if (int.TryParse(pageSizes.FirstOrDefault(), out var temp))
                        {
                            if (temp > 0)
                                command.PageSize = temp;
                        }
                    }

                    foreach (var pageSize in pageSizes)
                    {
                        if (!int.TryParse(pageSize, out var temp))
                            continue;

                        if (temp <= 0)
                            continue;

                        model.PageSizeOptions.Add(new SelectListItem
                        {
                            Text = pageSize,
                            Value = pageSize,
                            Selected = pageSize.Equals(command.PageSize.ToString(), StringComparison.InvariantCultureIgnoreCase)
                        });
                    }

                    if (model.PageSizeOptions.Any())
                    {
                        model.PageSizeOptions = model.PageSizeOptions.OrderBy(x => int.Parse(x.Value)).ToList();
                        model.AllowCustomersToSelectPageSize = true;

                        if (command.PageSize <= 0)
                            command.PageSize = int.Parse(model.PageSizeOptions.First().Value);
                    }
                }
            }
            else
            {
                //customer is not allowed to select a page size
                command.PageSize = fixedPageSize;
            }

            //ensure pge size is specified
            if (command.PageSize <= 0)
            {
                command.PageSize = fixedPageSize;
            }

            return Task.CompletedTask;
        }


        protected virtual ManufacturerFilterModel PrepareManufacturerFilterModel(IList<int> selectedManufacturers, IList<Manufacturer> availableManufacturers)
        {
            var model = new ManufacturerFilterModel();

            if (availableManufacturers?.Any() == true)
            {
                model.Enabled = true;

                foreach (var manufacturer in availableManufacturers)
                {
                    model.Manufacturers.Add(new SelectListItem
                    {
                        Value = manufacturer.Id.ToString(),
                        Text = manufacturer.Name,
                        Selected = selectedManufacturers?
                            .Any(manufacturerId => manufacturerId == manufacturer.Id) == true
                    });
                }
            }

            return model;
        }

        protected virtual async Task PrepareCatalogProductsAsync(CatalogProductsModel model, PagingResponse<Product> products)
        {
            if (!string.IsNullOrEmpty(model.WarningMessage))
                return;

            if (!products.Items.Any())
                model.NoResultMessage = "No result";
            else
            {
                model.Products = (await Task.WhenAll(products.Items.Select(async curProduct =>
                {
                    return await _productModelFactory.PrepareProductBoxModel(curProduct, null);
                }))).ToList();

                model.PagingMetaData = products.MetaData;
            }
        }

        public virtual void PrepareSortingOptions(CatalogProductsModel model, CatalogProductsCommand command)
        {
            var activeSortingOptionsIds = Enum.GetValues(typeof(ProductSortingEnum)).Cast<int>().ToList().Select(id => new { Id = id });

            //set the default option
            model.OrderBy = command.OrderBy;
            command.OrderBy = activeSortingOptionsIds.FirstOrDefault()?.Id ?? (int)ProductSortingEnum.Position;

            model.AllowProductSorting = true;
            command.OrderBy = model.OrderBy ?? command.OrderBy;

            //prepare available model sorting options
            foreach (var option in activeSortingOptionsIds)
            {
                model.AvailableSortOptions.Add(new SelectListItem
                {
                    Text = ((ProductSortingEnum)option.Id).ToString(),
                    Value = option.Id.ToString(),
                    Selected = option.Id == command.OrderBy
                });
            }
        }

        public async Task<CategoryNavigationModel> PrepareCategoryNavigationModelAsync(int currentCategoryId)
        {
            var activeCategoryId = 0;
            if (currentCategoryId > 0)
            {
                activeCategoryId = currentCategoryId;
            }

            var model = new CategoryNavigationModel()
            {
                CurrentCategoryId = activeCategoryId,
                Categories = await PrepareCategorySimpleModel()
            };

            return model;
        }

        private async Task<List<CategorySimpleModel>> PrepareCategorySimpleModel(int rootCategoryId = 0)
        {
            var categories = await _categoryService.GetAllCategoryAsync();
            var filteredCategories = categories.Where(c => c.ParentCategoryId == rootCategoryId).ToList();

            var result = new List<CategorySimpleModel>();

            foreach (var category in filteredCategories)
            {
                var categoryModel = new CategorySimpleModel()
                {
                    Id = category.Id,
                    Name = category.Name,
                    SeName = await _urlRecordService.GetSeNameAsync(category),
                    NumberOfProducts = (await _categoryService.GetProductCategoriesByCategoryIdAsync(category.Id)).Count,
                    IncludeInTopMenu = category.IncludeInTopMenu,
                    SubCategories = await PrepareCategorySimpleModel(category.Id)
                };

                categoryModel.HaveSubCategories = categoryModel.SubCategories.Count > 0 &
                                              categoryModel.SubCategories.Any(x => x.IncludeInTopMenu);

                result.Add(categoryModel);
            }

            return result;
        }
    }
}
