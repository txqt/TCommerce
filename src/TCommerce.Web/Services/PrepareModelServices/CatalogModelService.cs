using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using TCommerce.Web.Models.Catalog;
using TCommerce.Core.Models.Catalogs;
using Microsoft.Extensions.DependencyModel;
using TCommerce.Core.Models.ViewsModel;
using TCommerce.Core.Interface;
using TCommerce.Core.Helpers;
using TCommerce.Core.Extensions;
using MailKit.Search;
using TCommerce.Web.Areas.Admin.Services.PrepareAdminModel;
using TCommerce.Services.PrepareModelServices.PrepareAdminModel;

namespace TCommerce.Web.PrepareModelServices
{
    public interface ICatalogModelService
    {
        Task<CategoryModel> PrepareCategoryModelAsync(Category category, CatalogProductsCommand command, bool ignoreFeaturedProducts = false);
        Task<CatalogProductsModel> PrepareCategoryProductsModelAsync(Category category, CatalogProductsCommand command);
        Task<CategoryNavigationModel> PrepareCategoryNavigationModelAsync(int currentCategoryId);
        Task<ManufacturerModel> PrepareManufacturerModelAsync(Manufacturer manufacturer, CatalogProductsCommand command, bool ignoreFeaturedProducts = false);
        Task<CatalogProductsModel> PrepareManufacturerProductsModelAsync(Manufacturer manufacturer, CatalogProductsCommand command);
        Task<ManufacturerNavigationModel> PrepareManufacturerNavigationModelAsync(int currentManufacturerId);
        Task<SearchModel> PrepareSearchModelAsync(SearchModel model, CatalogProductsCommand command);
        Task<CatalogProductsModel> PrepareSearchProductsModelAsync(SearchModel searchModel, CatalogProductsCommand command);
        Task<SearchBoxModel> PrepareSearchBoxModelAsync();
    }
    public class CatalogModelService : ICatalogModelService
    {
        private readonly IMapper _mapper;
        private readonly IUrlRecordService _urlRecordService;
        private readonly ICategoryService _categoryService;
        private readonly IPictureService _pictureService;
        private readonly IProductService _productService;
        private readonly IProductModelService _productModelFactory;
        private readonly IManufacturerService _manufacturerService;
        private static readonly char[] _separator = { ',', ' ' };
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBaseAdminModelService _baseAdminModelService;
        private readonly ISettingService _settingService;
        private readonly CatalogSettings _catalogSettings;

        public CatalogModelService(IMapper mapper, IUrlRecordService urlRecordService, ICategoryService categoryService, IPictureService pictureService, IProductService productService, IProductModelService productModelFactory, IManufacturerService manufacturerService, IHttpContextAccessor httpContextAccessor, IBaseAdminModelService baseAdminModelService, ISettingService settingService, CatalogSettings catalogSettings)
        {
            _mapper = mapper;
            _urlRecordService = urlRecordService;
            _categoryService = categoryService;
            _pictureService = pictureService;
            _productService = productService;
            _productModelFactory = productModelFactory;
            _manufacturerService = manufacturerService;
            _httpContextAccessor = httpContextAccessor;
            _baseAdminModelService = baseAdminModelService;
            _settingService = settingService;
            _catalogSettings = catalogSettings;
        }

        #region Category
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
            var allCategories = await _categoryService.GetAllCategoryAsync();
            var subCategories = allCategories.Where(x => x.ParentCategoryId == category.Id).ToList();

            var subCategoryModels = new List<CategoryModel.SubCategoryModel>();

            foreach (var curCategory in subCategories)
            {
                var subCatModel = new CategoryModel.SubCategoryModel
                {
                    Id = curCategory.Id,
                    Name = curCategory.Name,
                    SeName = await _urlRecordService.GetSeNameAsync(curCategory),
                    Description = curCategory.Description
                };

                var picture = await _pictureService.GetPictureByIdAsync(curCategory.PictureId);

                var pictureModel = new PictureModel();

                if (picture is not null && !string.IsNullOrEmpty(picture.UrlPath))
                {
                    pictureModel.ImageUrl = picture.UrlPath;
                }

                subCatModel.PictureModel = pictureModel;

                subCategoryModels.Add(subCatModel);
            }

            model.SubCategories = subCategoryModels;



            //featured products
            if (!ignoreFeaturedProducts)
            {
                var featuredProducts = await _productService.GetCategoryFeaturedProductsAsync(category.Id);
                if (featuredProducts != null)
                {
                    model.FeaturedProducts = new List<Models.ProductBoxModel>();
                    foreach (var curProduct in featuredProducts)
                    {
                        model.FeaturedProducts.Add(await _productModelFactory.PrepareProductBoxModel(curProduct, null));
                    }
                }


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
            var pagedList = await _productService.SearchProductsAsync(new ProductParameters()
            {

                PageNumber = command.PageNumber,
                PageSize = command.PageSize,
                CategoryIds = categoryIds,
                ExcludeFeaturedProducts = false,
                PriceMin = selectedPriceRange?.From,
                PriceMax = selectedPriceRange?.To,
                OrderBy = orderString
            });

            var products = new PagingResponse<Product>()
            {
                Items = pagedList,
                MetaData = pagedList.MetaData
            };

            await PrepareCatalogProductsAsync(model, products);

            return model;
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
        #endregion

        #region Manufacturer
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

        public virtual async Task<ManufacturerModel> PrepareManufacturerModelAsync(Manufacturer manufacturer, CatalogProductsCommand command, bool ignoreFeaturedProducts = false)
        {
            ArgumentNullException.ThrowIfNull(manufacturer);

            ArgumentNullException.ThrowIfNull(command);

            var model = new ManufacturerModel
            {
                Id = manufacturer.Id,
                Name = manufacturer.Name,
                Description = manufacturer.Description,
                MetaKeywords = manufacturer.MetaKeywords,
                MetaDescription = manufacturer.MetaDescription,
                MetaTitle = manufacturer.MetaTitle,
                SeName = await _urlRecordService.GetSeNameAsync(manufacturer),
                CatalogProductsModel = await PrepareManufacturerProductsModelAsync(manufacturer, command)
            };

            //featured products
            if (!ignoreFeaturedProducts)
            {
                var featuredProducts = await _productService.GetManufacturerFeaturedProductsAsync(manufacturer.Id);
                if (featuredProducts != null)
                    model.FeaturedProducts = (await Task.WhenAll(featuredProducts.Select(async curProduct =>
                    {
                        return await _productModelFactory.PrepareProductBoxModel(curProduct, null);
                    }))).ToList();
            }

            return model;
        }
        public virtual async Task<CatalogProductsModel> PrepareManufacturerProductsModelAsync(Manufacturer manufacturer, CatalogProductsCommand command)
        {
            ArgumentNullException.ThrowIfNull(manufacturer);

            ArgumentNullException.ThrowIfNull(command);

            var model = new CatalogProductsModel
            {
                UseAjaxLoading = true
            };

            var manufacturerIds = new List<int> { manufacturer.Id };

            //sorting
            PrepareSortingOptions(model, command);
            //view mode
            PrepareViewModes(model, command);
            //page size
            await PreparePageSizeOptionsAsync(model, command, manufacturer.AllowCustomersToSelectPageSize,
                manufacturer.PageSizeOptions, manufacturer.PageSize);

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
            var pagedList = await _productService.SearchProductsAsync(new ProductParameters()
            {

                PageNumber = command.PageNumber,
                PageSize = command.PageSize,
                ExcludeFeaturedProducts = false,
                PriceMin = selectedPriceRange?.From,
                PriceMax = selectedPriceRange?.To,
                ManufacturerIds = command.ManufacturerIds,
                OrderBy = orderString
            });

            var products = new PagingResponse<Product>()
            {
                Items = pagedList,
                MetaData = pagedList.MetaData
            };

            await PrepareCatalogProductsAsync(model, products);

            return model;
        }

        public async Task<ManufacturerNavigationModel> PrepareManufacturerNavigationModelAsync(int currentManufacturerId)
        {

            var currentManufacturer = await _manufacturerService.GetManufacturerByIdAsync(currentManufacturerId);

            var manufacturers = await _manufacturerService.GetAllManufacturerAsync();
            var model = new ManufacturerNavigationModel
            {
                TotalManufacturers = manufacturers.Count
            };

            foreach (var manufacturer in manufacturers)
            {
                var modelMan = new ManufacturerBriefInfoModel
                {
                    Id = manufacturer.Id,
                    Name = manufacturer.Name,
                    SeName = await _urlRecordService.GetSeNameAsync(manufacturer),
                    IsActive = currentManufacturer != null && currentManufacturer.Id == manufacturer.Id,
                };
                model.Manufacturers.Add(modelMan);
            }

            return model;
        }
        #endregion

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

        protected virtual async Task PrepareCatalogProductsAsync(CatalogProductsModel model, PagingResponse<Product> products)
        {
            if (!string.IsNullOrEmpty(model.WarningMessage))
                return;

            if (!products.Items.Any())
                model.NoResultMessage = "No result";
            else
            {
                model.Products = new List<Models.ProductBoxModel>();
                foreach (var curProduct in products.Items)
                {
                    model.Products.Add(await _productModelFactory.PrepareProductBoxModel(curProduct, null));
                }

                model.PagingMetaData = products.MetaData;
            }
        }

        public async Task<CatalogProductsModel> PrepareSearchProductsModelAsync(SearchModel searchModel, CatalogProductsCommand command)
        {
            ArgumentNullException.ThrowIfNull(command);

            var model = new CatalogProductsModel
            {
                UseAjaxLoading = true
            };

            //sorting
            PrepareSortingOptions(model, command);
            //view mode
            PrepareViewModes(model, command);
            //page size
            await PreparePageSizeOptionsAsync(model, command, _catalogSettings.SearchPageAllowCustomersToSelectPageSize,
                _catalogSettings.SearchPagePageSizeOptions, _catalogSettings.SearchPageProductsPerPage);

            var searchTerms = searchModel.q == null
                ? string.Empty
                : searchModel.q.Trim();

            PagingResponse<Product> products = new PagingResponse<Product>();

            var isSearchTermSpecified = _httpContextAccessor.HttpContext.Request.Query.ContainsKey("q");
            if (isSearchTermSpecified)
            {

                if (searchTerms.Length < 3)
                {
                    model.WarningMessage =
                        string.Format("Độ dài tối thiểu : ", _catalogSettings.ProductSearchTermMinimumLength);
                }
                else
                {
                    var categoryIds = new List<int>();
                    var manufacturerId = 0;
                    var searchInDescriptions = false;
                    if (searchModel.advs)
                    {
                        //advanced search
                        var categoryId = searchModel.cid;
                        if (categoryId > 0)
                        {
                            categoryIds.Add(categoryId);
                            if (searchModel.isc)
                            {
                                var allCategories = await _categoryService.GetAllCategoryAsync();
                                var subCategories = allCategories.Where(x => x.ParentCategoryId == categoryId).Select(x => x.Id).ToList();
                                //include subcategories
                                categoryIds.AddRange(subCategories);
                            }
                        }

                        manufacturerId = searchModel.mid;

                        searchInDescriptions = searchModel.sid;
                    }

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
                    var pagedList = await _productService.SearchProductsAsync(new ProductParameters()
                    {

                        PageNumber = command.PageNumber,
                        PageSize = command.PageSize,
                        ExcludeFeaturedProducts = false,
                        ManufacturerIds = command.ManufacturerIds,
                        OrderBy = orderString,
                        SearchText = searchTerms
                    });

                    products = new PagingResponse<Product>()
                    {
                        Items = pagedList,
                        MetaData = pagedList.MetaData
                    };
                }
            }

            await PrepareCatalogProductsAsync(model, products);

            return model;
        }

        public async Task<SearchModel> PrepareSearchModelAsync(SearchModel model, CatalogProductsCommand command)
        {
            ArgumentNullException.ThrowIfNull(model);

            ArgumentNullException.ThrowIfNull(command);

            await _baseAdminModelService.PrepareSelectListCategoryAsync(model.AvailableCategories);

            await _baseAdminModelService.PrepareSelectListManufactureAsync(model.AvailableManufacturers);

            model.CatalogProductsModel = await PrepareSearchProductsModelAsync(model, command);

            return model;
        }

        public Task<SearchBoxModel> PrepareSearchBoxModelAsync()
        {
            var model = new SearchBoxModel
            {
                AutoCompleteEnabled = _catalogSettings.ProductSearchAutoCompleteEnabled,
                ShowProductImagesInSearchAutoComplete = _catalogSettings.ShowProductImagesInSearchAutoComplete,
                SearchTermMinimumLength = _catalogSettings.ProductSearchTermMinimumLength,
                ShowSearchBox = _catalogSettings.ProductSearchEnabled
            };

            return Task.FromResult(model);
        }
    }
}
