using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using T.Library.Model;
using T.Library.Model.Common;
using T.Library.Model.Interface;
using T.Library.Model.Response;
using T.Library.Model.Roles.RoleName;
using T.Library.Model.Security;
using T.Library.Model.Users;
using T.WebApi.Database.ConfigurationDatabase;
using T.WebApi.Services.PermissionRecordUserRoleMappingServices;
using T.WebApi.Services.ProductServices;

namespace T.WebApi.Services.DataSeederService
{
    public class DataSeeder
    {
        private readonly DatabaseContext _context;
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly IProductService _productService;
        private readonly ICategoryService _categorySerivce;
        private readonly IProductCategoryService _productCategorySerivce;
        private readonly IProductAttributeService _productAttributeService;
        //private readonly IProductAttributeMappingService _productAttributeMappingService;
        //private readonly IProductAttributeValueService _productAttributeValueService;
        private readonly ISecurityService _permissionRecordService;
        private readonly IPermissionRecordUserRoleMappingService _permissionRecordUserRoleMappingService;
        private readonly Func<string, Task<ServiceResponse<Product>>> _getProductByName;
        private readonly Func<string, Task<ServiceResponse<Category>>> _getCategoryByName;

        public DataSeeder(DatabaseContext context, RoleManager<Role> roleManager, UserManager<User> userManager, IProductService productSerivce, ICategoryService categorySerivce, IProductCategoryService productCategorySerivce, IProductAttributeService productAttributeService, /*IProductAttributeMappingService productAttributeMappingService,*/ /*IProductAttributeValueService productAttributeValueService,*/ ISecurityService permissionRecordService, IPermissionRecordUserRoleMappingService permissionRecordUserRoleMappingService)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _productService = productSerivce;
            _categorySerivce = categorySerivce;
            _getProductByName = _productService.GetByNameAsync;
            _getCategoryByName = _categorySerivce.GetCategoryByNameAsync;
            _productCategorySerivce = productCategorySerivce;
            _productAttributeService = productAttributeService;
            //_productAttributeMappingService = productAttributeMappingService;
            //_productAttributeValueService = productAttributeValueService;
            _permissionRecordService = permissionRecordService;
            _permissionRecordUserRoleMappingService = permissionRecordUserRoleMappingService;
        }

        public async Task Initialize(bool sampleData = false)
        {
            if (await SeedRoles() && await SeedPermission())
            {
                await SeedPermissionRolesMapping();
                if (sampleData)
                {
                    await SeedCategoriesAsync();
                    await SeedProductAttributeAsync();
                    await SeedProductsAsync();
                    await SeedUserAsync();
                }
            }
        }
        private async Task SeedCategoriesAsync()
        {
            if (!await _context.Category.AnyAsync())
            {
                foreach(var item in CategoriesDataSeed.Instance.GetAll())
                {
                    await _categorySerivce.CreateCategoryAsync(item);
                }
            }
        }
        private async Task SeedProductsAsync()
        {
            if (!await _context.Product.AnyAsync())
            {
                ProductDataSeed listProductSeed = new ProductDataSeed();

                foreach(var item in listProductSeed.GetAll())
                {
                    
                    foreach (var product in item.Products)
                    {

                        await _productService.CreateProductAsync(product);

                        var productId = (await _productService.GetByNameAsync(product.Name)).Data.Id;

                        foreach(var category in item.Categories)
                        {
                            var categoryId = (await _categorySerivce.GetCategoryByNameAsync(category.Name.ToString())).Data.Id;

                            var productCategoryMapping = new ProductCategory()
                            {
                                CategoryId = categoryId,
                                ProductId = productId
                            };
                            await _productCategorySerivce.CreateProductCategoryAsync(productCategoryMapping);
                        }
                        
                        foreach(var pa in item.ProductAttributes)
                        {
                            var productAttributeId = (await _productAttributeService.GetProductAttributeByName(pa.Name.ToString())).Data.Id;

                            var productAttributeMapping = new ProductAttributeMapping()
                            {
                                ProductId = productId,
                                ProductAttributeId = productAttributeId
                            };
                            await _productAttributeService.CreateProductAttributeMappingAsync(productAttributeMapping);

                            var productAttributeMappingId = (await _productAttributeService.GetProductAttributesMappingByProductIdAsync(productId)).Data.Where(x => x.ProductAttributeId == productAttributeId).FirstOrDefault().Id;

                            foreach (var pav in item.ProductAttributeValues)
                            {
                                var productAttributeValue = new ProductAttributeValue()
                                {
                                    ProductAttributeMappingId = productAttributeMappingId,
                                    Name = pav.Name,
                                };
                                await _productAttributeService.CreateProductAttributeValueAsync(productAttributeValue);
                            }
                        }
                    }
                }
            }
        }
        private async Task SeedProductAttributeAsync()
        {
            if (!await _context.ProductAttribute.AnyAsync())
            {
                foreach (var item in ProductAttributesDataSeed.Instance.GetAll())
                {
                    await _productAttributeService.CreateProductAttributeAsync(item);
                }
            };
            
        }
        private async Task<bool> SeedPermission()
        {
            if (await _context.PermissionRecords.AnyAsync())
            {
                return false;
            }

            var permission_list = await _permissionRecordService.GetAllPermissionRecordAsync();

            try
            {
                await _context.PermissionRecords.AddRangeAsync(permission_list);
                await _context.SaveChangesAsync();
                return true;
            }
            catch 
            {
                return false;
            }
        }
        private async Task SeedPermissionRolesMapping()
        {
            if (await _context.PermissionRecordUserRoleMappings.AnyAsync())
            {
                return;
            }

            var list = new List<RolePermissionMappingSeedModel>()
            {
                new RolePermissionMappingSeedModel()
                {
                    Roles = new List<Role>()
                    {
                        new Role(RoleName.Employee)
                    },
                    PermissionRecords = new List<PermissionRecord>()
                    {
                        DefaultPermission.AccessAdminPanel,
                        DefaultPermission.ManageProducts,
                        DefaultPermission.ManageCategories,
                        DefaultPermission.ManageAttributes,
                    }
                },
                new RolePermissionMappingSeedModel()
                {
                    Roles = new List<Role>()
                    {
                        new Role(RoleName.Admin)
                    },
                    PermissionRecords = await _permissionRecordService.GetAllPermissionRecordAsync()
                }
            };

            foreach (var item in list)
            {
                foreach (var role in item.Roles)
                {
                    foreach (var permission in item.PermissionRecords)
                    {
                        var rolePermissionMapping = new PermissionRecordUserRoleMapping()
                        {
                            PermissionRecordId = (await _permissionRecordService.GetPermissionRecordBySystemNameAsync(permission.SystemName)).Data.Id,
                            RoleId = await GetRoleId(_roleManager, role.Name)
                        };
                        await _permissionRecordUserRoleMappingService.CreateOrEditAsync(rolePermissionMapping);
                    }
                }
            }
        }
        private async Task<bool> SeedRoles()
        {
            if (await _context.Roles.AnyAsync())
            {
                return false;
            }

            try
            {
                var rolenames = typeof(RoleName).GetFields();
                foreach (var item in rolenames)
                {
                    string name = item.GetRawConstantValue().ToString();
                    var ffound = await _roleManager.FindByNameAsync(name);
                    if (ffound == null)
                    {
                        await _roleManager.CreateAsync(new Role(name));
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        private async Task SeedUserAsync()
        {
            if (await _context.Users.AnyAsync())
            {
                return;
            }

            foreach (var item in UserRoleMappingDataSeed.Instance.GetAll())
            {
                foreach(var user in item.Users)
                {
                    await _userManager.CreateAsync(user, "123321");
                    var createdUser = await _userManager.FindByNameAsync(user.UserName);
                    foreach(var role in item.Roles)
                    {
                        await _userManager.AddToRoleAsync(createdUser, role.Name);
                    }
                }
            }
        }
        public async Task<Guid> GetRoleId(RoleManager<Role> roleManager, string roleName)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role != null)
            {
                return role.Id;
            }
            return Guid.Empty;
        }
    }
}