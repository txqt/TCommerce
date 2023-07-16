using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using T.Library.Model;
using T.Library.Model.Common;
using T.Library.Model.Response;
using T.Library.Model.Roles.RoleName;
using T.Library.Model.Security;
using T.Library.Model.Users;
using T.WebApi.Database.ConfigurationDatabase;
using T.WebApi.Extensions;
using T.WebApi.Services.CategoryServices;
using T.WebApi.Services.PermissionRecordServices;
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
        private readonly IProductAttributeMappingService _productAttributeMappingService;
        private readonly IProductAttributeValueService _productAttributeValueService;
        private readonly IPermissionRecordService _permissionRecordService;
        private readonly IPermissionRecordUserRoleMappingService _permissionRecordUserRoleMappingService;
        private readonly Func<string, Task<ServiceResponse<Product>>> _getProductByName;
        private readonly Func<string, Task<ServiceResponse<Category>>> _getCategoryByName;

        public DataSeeder(DatabaseContext context, RoleManager<Role> roleManager, UserManager<User> userManager, IProductService productSerivce, ICategoryService categorySerivce, IProductCategoryService productCategorySerivce, IProductAttributeService productAttributeService, IProductAttributeMappingService productAttributeMappingService, IProductAttributeValueService productAttributeValueService, IPermissionRecordService permissionRecordService, IPermissionRecordUserRoleMappingService permissionRecordUserRoleMappingService)
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
            _productAttributeMappingService = productAttributeMappingService;
            _productAttributeValueService = productAttributeValueService;
            _permissionRecordService = permissionRecordService;
            _permissionRecordUserRoleMappingService = permissionRecordUserRoleMappingService;
        }

        public async Task Initialize()
        {
            await SeedCategoriesAsync();
            await SeedProductAttributeAsync();
            await SeedProductsAsync();
            
            if (await SeedRoles() && await SeedPermission())
            {
                await SeedPermissionRolesMapping();
                await SeedUserAsync();
            }

        }
        private async Task SeedCategoriesAsync()
        {
            if (!await _context.Category.AnyAsync())
            {
                foreach(var item in CategoriesDataSeed.Instance.GetAll())
                {
                    await _categorySerivce.CreateOrEditAsync(item);
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
                        await _productService.CreateProduct(product);

                        var productId = (await _productService.GetByNameAsync(product.Name)).Data.Id;

                        foreach(var category in item.Categories)
                        {
                            var categoryId = (await _categorySerivce.GetCategoryByNameAsync(category.Name.ToString())).Data.Id;

                            var productCategoryMapping = new ProductCategory()
                            {
                                CategoryId = categoryId,
                                ProductId = productId
                            };
                            await _productCategorySerivce.CreateOrEditAsync(productCategoryMapping);
                        }
                        
                        foreach(var pa in item.ProductAttributes)
                        {
                            var productAttributeId = (await _productAttributeService.GetProductAttributeByName(pa.Name.ToString())).Data.Id;

                            var productAttributeMapping = new ProductAttributeMapping()
                            {
                                ProductId = productId,
                                ProductAttributeId = productAttributeId
                            };
                            await _productAttributeMappingService.CreateOrEditProductAttributeMappingAsync(productAttributeMapping);

                            var productAttributeMappingId = (await _productAttributeMappingService.GetProductAttributeMappingByProductIdAsync(productId)).Data.Where(x => x.ProductAttributeId == productAttributeId).FirstOrDefault().Id;

                            foreach (var pav in item.ProductAttributeValues)
                            {
                                var productAttributeValue = new ProductAttributeValue()
                                {
                                    ProductAttributeMappingId = productAttributeMappingId,
                                    Name = pav.Name,
                                };
                                await _productAttributeValueService.AddOrUpdateProductAttributeValue(productAttributeValue);
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
            var permission_list = new List<PermissionRecord>() 
            {
            new PermissionRecord() {
                  Name = "Access admin area",
                     SystemName = PermissionSystemName.AccessAdminPanel,
                     Category = "Manager"
               },
               new PermissionRecord() {
                  Name = "Admin area: Manage Products",
                     SystemName = PermissionSystemName.ManageProducts,
                     Category = "Manager"
               },
               new PermissionRecord() {
                  Name = "Admin area: Manage Categories",
                     SystemName = PermissionSystemName.ManageCategories,
                     Category = "Manager"
               },
               new PermissionRecord() {
                  Name = "Admin area: Manage Attributes",
                     SystemName = PermissionSystemName.ManageAttributes,
                     Category = "Manager"
               },
               new PermissionRecord() {
                  Name = "Admin area: Manage Users",
                     SystemName = PermissionSystemName.ManageCustomers,
                     Category = "Manager"
               }
         };
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

            foreach (var item in RolePermissionMappingDataSeed.Instance.GetAll())
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
        public async Task<Guid?> GetRoleId(RoleManager<Role> roleManager, string roleName)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role != null)
            {
                return role.Id;
            }
            return null;
        }
    }
}