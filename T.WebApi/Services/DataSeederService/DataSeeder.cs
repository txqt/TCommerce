using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using T.Library.Model;
using T.Library.Model.Common;
using T.Library.Model.Enum;
using T.Library.Model.Response;
using T.Library.Model.Security;
using T.Library.Model.Users;
using T.WebApi.Database.ConfigurationDatabase;
using T.WebApi.Extensions;
using T.WebApi.Services.CategoryServices;
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
        private readonly Func<string, Task<ServiceResponse<Product>>> _getProductByName;
        private readonly Func<string, Task<ServiceResponse<Category>>> _getCategoryByName;

        public DataSeeder(DatabaseContext context, RoleManager<Role> roleManager, UserManager<User> userManager, IProductService productSerivce, ICategoryService categorySerivce, IProductCategoryService productCategorySerivce, IProductAttributeService productAttributeService, IProductAttributeMappingService productAttributeMappingService, IProductAttributeValueService productAttributeValueService)
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
        }

        public async Task Initialize()
        {
            await SeedCategoriesAsync();
            await SeedProductAttributeAsync();
            await SeedProductsAsync();
            //SeedUser();
            if (await SeedRoles() && SeedPermission())
            {
                SeedPermissionRolesMapping();
            }

        }
        private async Task SeedCategoriesAsync()
        {
            if (!await _context.Category.AnyAsync())
            {
                Type seedType = typeof(CategoryName);
                FieldInfo[] fields = seedType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

                foreach (FieldInfo field in fields)
                {
                    string fieldName = field.Name;
                    object fieldValue = field.GetValue(null);

                    Category category = new Category
                    {
                        Name = fieldValue.ToString(),
                        CreatedOnUtc = DateTime.UtcNow
                    };

                    await _categorySerivce.CreateOrEditAsync(category);
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
                    var categoryId = (await _categorySerivce.GetCategoryByNameAsync(item.CategoryName.ToString())).Data.Id;
                    var productAttributeId = (await _productAttributeService.GetProductAttributeByName(item.ProductAttributeName.ToString())).Data.Id;

                    foreach (var product in item.Products)
                    {
                        await _productService.CreateProduct(product);

                        var productId = (await _productService.GetByNameAsync(product.Name)).Data.Id;

                        var productCategoryMapping = new ProductCategory()
                        {
                            CategoryId = categoryId,
                            ProductId = productId
                        };
                        await _productCategorySerivce.CreateOrEditAsync(productCategoryMapping);

                        var productAttributeMapping = new ProductAttributeMapping()
                        {
                            ProductId = productId,
                            ProductAttributeId = productAttributeId
                        };
                        await _productAttributeMappingService.CreateOrEditProductAttributeMappingAsync(productAttributeMapping);

                        var productAttributeMappingId = (await _productAttributeMappingService.GetProductAttributeMappingByProductIdAsync(productId)).Data.Where(x=>x.ProductAttributeId == productAttributeId).FirstOrDefault().Id;

                        foreach(var pavName in item.ProductAttributeValues)
                        {
                            var productAttributeValue = new ProductAttributeValue()
                            {
                                ProductAttributeMappingId = productAttributeMappingId,
                                Name = pavName,
                            };
                            await _productAttributeValueService.AddOrUpdateProductAttributeValue(productAttributeValue);
                        }
                    }
                }
            }
        }
        private async Task SeedProductAttributeAsync()
        {
            if (!await _context.ProductAttribute.AnyAsync())
            {
                Type seedType = typeof(ProductAttributeName);
                FieldInfo[] fields = seedType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

                foreach (FieldInfo field in fields)
                {
                    string fieldName = field.Name;
                    object fieldValue = field.GetValue(null);

                    ProductAttribute productAttribute = new ProductAttribute
                    {
                        Name = fieldValue.ToString(),
                    };

                    await _productAttributeService.CreateProductAttributeAsync(productAttribute);
                }
            };
            
        }
        private bool SeedPermission()
        {
            if (!_context.PermissionRecords.Any())
            {
                return false;
            }
            var permission_list = new List<PermissionRecord>() 
            {
            new PermissionRecord() {
                  Name = "Access admin area",
                     SystemName = "AccessAdminPanel",
                     Category = "Standard"
               },
               new PermissionRecord() {
                  Name = "Admin area: Manage Products",
                     SystemName = "ManageProducts",
                     Category = "Manager"
               },
               new PermissionRecord() {
                  Name = "Admin area: Manage Categories",
                     SystemName = "ManageCategories",
                     Category = "Manager"
               },
               new PermissionRecord() {
                  Name = "Admin area: Manage Attributes",
                     SystemName = "ManageAttributes",
                     Category = "Manager"
               },
               new PermissionRecord() {
                  Name = "Admin area: Manage Customers",
                     SystemName = "ManageCustomers",
                     Category = "Manager"
               }
         };
            try
            {
                _context.PermissionRecords.AddRange(permission_list);
                _context.SaveChanges();
                return true;
            }
            catch 
            {
                return false;
            }
        }
        private async Task SeedPermissionRolesMapping()
        {
            if (!_context.PermissionRecordUserRoleMappings.Any())
            {
                return;
            }
            var permission_roles_mapping = new List<PermissionRecordUserRoleMapping>() {
            new PermissionRecordUserRoleMapping() {
                  PermissionRecordId = 1,
                     UserRoleId = await GetRoleId(_roleManager, RoleName.Admin.ToString())
               },
               new PermissionRecordUserRoleMapping() {
                  PermissionRecordId = 2,
                     UserRoleId = await GetRoleId(_roleManager, RoleName.Admin.ToString())
               },
               new PermissionRecordUserRoleMapping() {
                  PermissionRecordId = 3,
                     UserRoleId = await GetRoleId(_roleManager, RoleName.Admin.ToString())
               },
               new PermissionRecordUserRoleMapping() {
                  PermissionRecordId = 4,
                     UserRoleId = await GetRoleId(_roleManager, RoleName.Admin.ToString())
               },
               new PermissionRecordUserRoleMapping() {
                  PermissionRecordId = 5,
                     UserRoleId = await GetRoleId(_roleManager, RoleName.Admin.ToString())
               }
         };
            _context.PermissionRecordUserRoleMappings.AddRange(permission_roles_mapping);
            _context.SaveChanges();
        }
        private async Task<bool> SeedRoles()
        {
            if (_context.Roles.Any())
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
        private async void SeedUser()
        {
            if (_context.Users.Any())
            {
                return;
            }
            var user2 = await _userManager.FindByEmailAsync("hovanthanh12102002@gmail.com");
            if (user2 == null)
            {
                user2 = new User()
                {
                    Id = Guid.NewGuid(),

                    FirstName = "Văn Thành",
                    LastName = "Hồ",
                    Email = "hovanthanh12102002@gmail.com",
                    NormalizedEmail = "hovanthanh12102002@gmail.com",
                    PhoneNumber = "032232131",
                    UserName = "thanhhv",
                    NormalizedUserName = "THANHHV",
                    CreatedDate = AppExtensions.GetDateTimeNow(),
                    EmailConfirmed = true // không cần xác thực email nữa , 
                };
                await _userManager.CreateAsync(user2, "123321");
                await _userManager.AddToRoleAsync(user2, RoleName.Admin);
            }

            var user3 = await _userManager.FindByEmailAsync("hovanthanh@gmail.com");
            if (user3 == null)
            {
                user3 = new User()
                {
                    Id = Guid.NewGuid(),

                    FirstName = "Văn Thành",
                    LastName = "Hồ",
                    Email = "hovanthanh@gmail.com",
                    NormalizedEmail = "hovanthanh@gmail.com",
                    PhoneNumber = "032232131",
                    UserName = "thanhhv2",
                    NormalizedUserName = "THANHHV2",
                    CreatedDate = AppExtensions.GetDateTimeNow(),
                    EmailConfirmed = true // không cần xác thực email nữa , 
                };
                await _userManager.CreateAsync(user3, "123321");
                await _userManager.AddToRoleAsync(user3, RoleName.Customer);
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