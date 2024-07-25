using Microsoft.AspNetCore.Identity;
using System.Reflection;
using TCommerce.Services.DataSeederServices;
using AutoMapper;
using Newtonsoft.Json;
using TCommerce.Core.Models.Security;
using TCommerce.Core.Models.Users;
using TCommerce.Core.Models.RoleName;
using TCommerce.Core.Models.Common;
using TCommerce.Core.Models.ViewsModel;
using TCommerce.Core.Models.Catalogs;
using TCommerce.Data;
using Microsoft.Extensions.DependencyInjection;
using TCommerce.Data.Helpers;
using TCommerce.Services.IRepositoryServices;
using Microsoft.EntityFrameworkCore;
using TCommerce.Services.ManufacturerServices;
using TCommerce.Services.UrlRecordServices;
using TCommerce.Core.Models.Discounts;
using TCommerce.Core.Interface;

namespace TCommerce.ServicesSeederService
{
    public class DataSeeder
    {
        private readonly IServiceProvider _serviceProvider;

        public DataSeeder(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Initialize(string AdminEmail, string AdminPassword, bool sampleData = false)
        {
            await _initRequiredData(AdminEmail, AdminPassword, sampleData);
            await _initSampleData();
        }

        private async Task _initSampleData()
        {
            await SeedDiscountsAsync();
            await SeedCategoriesAsync();
            await SeedManufacturerAsync();
            await SeedProductAttributeAsync();
            await SeedProductsAsync();
            await SeedUserAsync();
        }

        private async Task _initRequiredData(string adminEmail, string adminPassword, bool sampleData = false)
        {
            if (await SeedRoles() && await SeedPermission())
            {
                await SeedPermissionRolesMapping();
                await SeedSettingAsync();
                await SeedAdminUserAsync(adminEmail, adminPassword);
            }
        }
        private async Task SeedAdminUserAsync(string adminEmail, string adminPassword)
        {
            var user = new User()
            {
                Id = Guid.NewGuid(),
                FirstName = "Admin",
                LastName = "Admin",
                Email = adminEmail,
                NormalizedEmail = adminEmail,
                PhoneNumber = "0322321312",
                UserName = adminEmail,
                NormalizedUserName = adminEmail.ToUpper(),
                CreatedDate = DateTime.UtcNow,
                EmailConfirmed = true
            };

            var userManager = _serviceProvider.GetRequiredService<UserManager<User>>();

            var result = await userManager.CreateAsync(user, adminPassword);
            if (result.Succeeded)
            {
                try
                {
                    await userManager.AddToRoleAsync(user, RoleName.Admin);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }
            }
            else
            {
                throw new Exception("Something went wrong");
            }

            // Address seed
            try
            {
                // Path to JSON file
                string jsonFilePath = Path.Combine("jsondata", "addressData.json");

                // Read JSON file content
                string jsonContent = File.ReadAllText(jsonFilePath);

                dynamic jsonData = JsonConvert.DeserializeObject(jsonContent);

                var provinceList = new List<VietNamProvince>();
                foreach (var p in jsonData.province)
                {
                    var province = new VietNamProvince()
                    {
                        Id = p.idProvince,
                        Name = p.name
                    };
                    provinceList.Add(province);
                }

                var districtList = new List<VietNamDistrict>();
                foreach (var d in jsonData.district)
                {
                    var district = new VietNamDistrict()
                    {
                        Id = d.idDistrict,
                        IdProvince = d.idProvince,
                        Name = d.name,
                    };
                    districtList.Add(district);
                }

                var communeList = new List<VietNamCommune>();
                foreach (var c in jsonData.commune)
                {
                    var commune = new VietNamCommune()
                    {
                        Id = Convert.ToInt32(c.idCommune),
                        IdDistrict = Convert.ToInt32(c.idDistrict),
                        Name = c.name,
                    };
                    communeList.Add(commune);
                }

                var _context = _serviceProvider.GetRequiredService<ApplicationDbContext>();
                var _addressService = _serviceProvider.GetRequiredService<IAddressService>();

                using var transaction = _context.Database.BeginTransaction();

                await _context.EnableIdentityInsert<VietNamProvince>();
                await _addressService.BulkCreateProvince(provinceList);
                await _context.DisableIdentityInsert<VietNamProvince>();

                await _context.EnableIdentityInsert<VietNamDistrict>();
                await _addressService.BulkCreateDistrict(districtList);
                await _context.DisableIdentityInsert<VietNamDistrict>();

                await _context.EnableIdentityInsert<VietNamCommune>();
                await _addressService.BulkCreateCommune(communeList);
                await _context.DisableIdentityInsert<VietNamCommune>();

                transaction.Commit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        private async Task SeedDiscountsAsync()
        {
            var _discountService = _serviceProvider.GetRequiredService<IRepository<Discount>>();

            foreach (var item in DiscountDataSeed.Instance.GetAll())
            {
                await _discountService.CreateAsync(item);
            }
        }

        private async Task SeedCategoriesAsync()
        {
            var _categorySerivce = _serviceProvider.GetRequiredService<ICategoryService>();
            var _urlRecordService = _serviceProvider.GetRequiredService<IUrlRecordService>();

            foreach (var item in CategoriesDataSeed.Instance.GetAll())
            {
                await _categorySerivce.CreateCategoryAsync(item);

                var seName = await _urlRecordService.ValidateSlug(item, "", item.Name, true);

                await _urlRecordService.SaveSlugAsync(item, seName);
            }
        }

        private async Task SeedManufacturerAsync()
        {
            var _manufacturerService = _serviceProvider.GetRequiredService<IManufacturerService>();
            var _urlRecordService = _serviceProvider.GetRequiredService<IUrlRecordService>();

            foreach (var item in ManufacturerDataSeed.Instance.GetAll())
            {
                await _manufacturerService.CreateManufacturerAsync(item);

                var seName = await _urlRecordService.ValidateSlug(item, "", item.Name, true);

                await _urlRecordService.SaveSlugAsync(item, seName);
            }
        }

        private async Task SeedProductsAsync()
        {
            ProductDataSeed listProductSeed = new ProductDataSeed();
            var _productRepository = _serviceProvider.GetRequiredService<IRepository<Product>>();
            var categoryService = _serviceProvider.GetRequiredService<ICategoryService>();
            var productAttributeService = _serviceProvider.GetRequiredService<IProductAttributeService>();
            var manufacturerService = _serviceProvider.GetRequiredService<IManufacturerService>();
            var _urlRecordService = _serviceProvider.GetRequiredService<IUrlRecordService>();
            var _discountProductMappingRepository = _serviceProvider.GetRequiredService<IRepository<DiscountProductMapping>>();
            var _discountRepository = _serviceProvider.GetRequiredService<IRepository<Discount>>();

            foreach (var item in listProductSeed.GetAll())
            {
                foreach (var productItem in item.Products!)
                {
                    await _productRepository.CreateAsync(productItem);

                    var product = (await _productRepository.Table.Where(x => x.Name == productItem.Name).FirstOrDefaultAsync());

                    var productId = product.Id;

                    var seName = await _urlRecordService.ValidateSlug(productItem, "", productItem.Name, true);

                    await _urlRecordService.SaveSlugAsync(productItem, seName);

                    if(item.Categories is not null)
                    {
                        foreach (var category in item.Categories!)
                        {
                            var categoryId = (await categoryService.GetCategoryByNameAsync(category.Name)).Id;

                            var productCategoryMapping = new ProductCategory()
                            {
                                CategoryId = categoryId,
                                ProductId = productId
                            };
                            await categoryService.CreateProductCategoryAsync(productCategoryMapping);
                        }
                    }

                    if(item.Discounts is not null)
                    {
                        foreach (var pa in item.ProductAttributes!)
                        {
                            var productAttributeId = (await productAttributeService.GetProductAttributeByName(pa.Name!.ToString())).Id;

                            var productAttributeMapping = new ProductAttributeMapping()
                            {
                                ProductId = productId,
                                ProductAttributeId = productAttributeId
                            };
                            await productAttributeService.CreateProductAttributeMappingAsync(productAttributeMapping);

                            var productAttributeMappingId = (await productAttributeService.GetProductAttributesMappingByProductIdAsync(productId)).Where(x => x.ProductAttributeId == productAttributeId).FirstOrDefault().Id;

                            foreach (var pav in item.ProductAttributeValues!)
                            {
                                var productAttributeValue = new ProductAttributeValue()
                                {
                                    ProductAttributeMappingId = productAttributeMappingId,
                                    Name = pav.Name,
                                };
                                await productAttributeService.CreateProductAttributeValueAsync(productAttributeValue);
                            }
                        }
                    }

                    if(item.Manufacturers is not null)
                    {
                        foreach (var pm in item.Manufacturers!)
                        {
                            var manfacturerId = (await manufacturerService.GetManufacturerByNameAsync(pm.Name!)).Id;

                            var productManufacturer = new ProductManufacturer()
                            {
                                ProductId = productId,
                                ManufacturerId = manfacturerId
                            };
                            await manufacturerService.CreateProductManufacturerAsync(productManufacturer);

                            var productManfacturer = (await manufacturerService.GetProductManufacturersByManufacturerIdAsync(manfacturerId))
                                                        .Where(x => x.ProductId == productId).FirstOrDefault();
                            ArgumentNullException.ThrowIfNull(productManfacturer);
                        }
                    }

                    if(item.Discounts is not null)
                    {
                        foreach (var pm in item.Discounts!)
                        {
                            var discountId = (await _discountRepository.Table.Where(x => x.Name == pm.Name).FirstOrDefaultAsync()).Id;

                            var discountProductMapping = new DiscountProductMapping()
                            {
                                DiscountId = discountId,
                                EntityId = productId
                            };
                            await _discountProductMappingRepository.CreateAsync(discountProductMapping);
                        }
                    }
                }
            }
        }

        private async Task SeedProductAttributeAsync()
        {
            var _productAttributeService = _serviceProvider.GetRequiredService<IProductAttributeService>();

            foreach (var item in ProductAttributesDataSeed.Instance.GetAll())
            {
                await _productAttributeService.CreateProductAttributeAsync(item);
            }
        }

        private async Task<bool> SeedPermission()
        {
            var _securityService = _serviceProvider.GetRequiredService<ISecurityService>();

            FieldInfo[] fields = typeof(DefaultPermission).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);

            List<PermissionRecord> permission_list = new List<PermissionRecord>();

            foreach (FieldInfo field in fields)
            {
                if (field.FieldType == typeof(PermissionRecord))
                {
                    PermissionRecord permission = (PermissionRecord)field.GetValue(null);
                    permission_list.Add(permission);
                }
            }

            try
            {
                foreach (var item in permission_list)
                {
                    if (!(await _securityService.CreatePermissionRecord(item)).Success)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task SeedPermissionRolesMapping()
        {
            var _securityService = _serviceProvider.GetRequiredService<ISecurityService>();

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
                        DefaultPermission.ManageDiscounts,
                    }
                },
                new RolePermissionMappingSeedModel()
                {
                    Roles = new List<Role>()
                    {
                        new Role(RoleName.Admin)
                    },
                    PermissionRecords = await _securityService.GetAllPermissionRecordAsync()
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
                            PermissionRecordId = (await _securityService.GetPermissionRecordBySystemNameAsync(permission.SystemName)).Id,
                            RoleId = await GetRoleId(role.Name)
                        };
                        await _securityService.CreatePermissionMappingAsync(rolePermissionMapping);
                    }
                }
            }
        }

        private async Task<bool> SeedRoles()
        {
            try
            {
                var rolenames = typeof(RoleName).GetFields();
                var roleManager = _serviceProvider.GetRequiredService<RoleManager<Role>>();

                foreach (var item in rolenames)
                {
                    string name = item.GetRawConstantValue().ToString();
                    var ffound = await roleManager.FindByNameAsync(name);
                    if (ffound == null)
                    {
                        await roleManager.CreateAsync(new Role(name));
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
            foreach (var item in UserRoleMappingDataSeed.Instance.GetAll())
            {
                foreach (var user in item.Users)
                {
                    var userManager = _serviceProvider.GetRequiredService<UserManager<User>>();

                    var result = await userManager.CreateAsync(user, "123321");
                    if (result.Succeeded)
                    {
                        foreach (var role in item.Roles)
                        {
                            try
                            {
                                await userManager.AddToRoleAsync(user, role.Name);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                                return;
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Something went wrong");
                    }
                }
            }
        }

        private async Task<Guid> GetRoleId(string roleName)
        {
            var roleManager = _serviceProvider.GetRequiredService<RoleManager<Role>>();
            var role = await roleManager.FindByNameAsync(roleName);
            if (role != null)
            {
                return role.Id;
            }
            return Guid.Empty;
        }

        private async Task SeedSettingAsync()
        {
            var settingService = _serviceProvider.GetRequiredService<ISettingService>();
            var catalogSetting = new CatalogSettings()
            {
                UseAjaxCatalogProductsLoading = true,
                SearchPageAllowCustomersToSelectPageSize = true,
                SearchPageProductsPerPage = 10,
                SearchPagePageSizeOptions = "10,20,50",
                ProductSearchTermMinimumLength = 3,
                ProductSearchEnabled = true,
                AllowProductViewModeChanging = true,
                DefaultViewMode = "3-cols"
            };

            await settingService.SaveSettingAsync(catalogSetting);
        }
    }
}
