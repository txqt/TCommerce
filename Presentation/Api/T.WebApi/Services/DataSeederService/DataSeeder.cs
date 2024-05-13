using Microsoft.AspNetCore.Identity;
using System.Reflection;
using T.Library.Model;
using T.Library.Model.Catalogs;
using T.Library.Model.Interface;
using T.Library.Model.Roles.RoleName;
using T.Library.Model.Security;
using T.Library.Model.Users;
using T.WebApi.Services.ProductServices;
using T.WebApi.Services.DataSeederService;
using AutoMapper;
using T.Library.Model.ViewsModel;
using T.WebApi.Services.CategoryServices;
using T.WebApi.Database;

namespace T.WebApi.ServicesSeederService
{
    public class DataSeeder
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly IProductService _productService;
        private readonly ICategoryService _categorySerivce;
        private readonly IProductAttributeService _productAttributeService;
        private readonly ISecurityService _securityService;
        private readonly IManufacturerServicesCommon _manufacturerServicesCommon;
        private readonly IMapper _mapper;
        private readonly DatabaseContext _context;

        public DataSeeder(RoleManager<Role> roleManager,
            UserManager<User> userManager,
            IProductService productSerivce,
            ICategoryService categorySerivce,
            IProductCategoryService productCategorySerivce,
            IProductAttributeService productAttributeService,
            ISecurityService permissionRecordService,
            IManufacturerServicesCommon manufacturerServicesCommon,
            IMapper mapper,
            DatabaseContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _productService = productSerivce;
            _categorySerivce = categorySerivce;
            _productAttributeService = productAttributeService;
            _securityService = permissionRecordService;
            this._manufacturerServicesCommon = manufacturerServicesCommon;
            _mapper = mapper;
            _context = context;
        }

        public async Task Initialize(string AdminEmail, string AdminPassword, bool sampleData = false)
        {
            if (await SeedRoles() && await SeedPermission())
            {
                await SeedPermissionRolesMapping();
                if (sampleData)
                {
                    await SeedCategoriesAsync();
                    await SeedManufacturerAsync();
                    await SeedProductAttributeAsync();
                    await SeedProductsAsync();
                    await SeedUserAsync();
                }
                var user = new User()
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Admin",
                    LastName = "Admin",
                    Email = AdminEmail,
                    NormalizedEmail = AdminEmail,
                    PhoneNumber = "0322321312",
                    UserName = AdminEmail,
                    NormalizedUserName = AdminEmail.ToUpper(),
                    CreatedDate = DateTime.UtcNow,
                    EmailConfirmed = true
                };
                var result = await _userManager.CreateAsync(user, AdminPassword);
                if (result.Succeeded)
                {
                    try
                    {
                        await _userManager.AddToRoleAsync(user, RoleName.Admin);
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

                //var countriesJson = System.IO.File.ReadAllText(Path.Combine("jsondata", "countries.json"));
                //var statesJson = System.IO.File.ReadAllText(Path.Combine("jsondata", "states.json"));
                //var citiesJson = System.IO.File.ReadAllText(Path.Combine("jsondata", "cities.json"));

                //try
                //{
                //    var countryObjects = JsonConvert.DeserializeObject<List<Country>>(countriesJson);
                //    var stateObjects = JsonConvert.DeserializeObject<List<State>>(statesJson);
                //    var cityObjects = JsonConvert.DeserializeObject<List<City>>(citiesJson);

                //    using var transaction = _context.Database.BeginTransaction();

                //    await _context.EnableIdentityInsert<Country>();
                //    await _countryService.BulkCreateCountryAsync(countryObjects);
                //    await _context.DisableIdentityInsert<Country>();

                //    await _context.EnableIdentityInsert<State>();
                //    await _stateService.BulkCreateStateAsync(stateObjects);
                //    await _context.DisableIdentityInsert<State>();

                //    await _context.EnableIdentityInsert<City>();
                //    await _cityService.BulkCreateCityAsync(cityObjects);
                //    await _context.DisableIdentityInsert<City>();

                //    transaction.Commit();
                //}
                //catch(Exception e)
                //{
                //    Console.WriteLine(e.Message);
                //}


            }
        }
        private async Task SeedCategoriesAsync()
        {
            foreach (var item in CategoriesDataSeed.Instance.GetAll())
            {
                var model = _mapper.Map<CategoryModel>(item);
                await _categorySerivce.CreateCategoryAsync(model);
            }

        }

        private async Task SeedManufacturerAsync()
        {
            foreach (var item in ManufacturerDataSeed.Instance.GetAll())
            {
                await _manufacturerServicesCommon.CreateManufacturerAsync(item);
            }

        }
        private async Task SeedProductsAsync()
        {

            ProductDataSeed listProductSeed = new ProductDataSeed();

            foreach (var item in listProductSeed.GetAll())
            {

                foreach (var product in item.Products!)
                {
                    var model = _mapper.Map<ProductModel>(product);

                    await _productService.CreateProductAsync(model);

                    var productId = (await _productService.GetByNameAsync(product.Name))!.Id;

                    foreach (var category in item.Categories!)
                    {
                        var categoryId = (await _categorySerivce.GetCategoryByNameAsync(category.Name!.ToString()))!.Id;

                        var productCategoryMapping = new ProductCategory()
                        {
                            CategoryId = categoryId,
                            ProductId = productId
                        };
                        await _categorySerivce.CreateProductCategoryAsync(productCategoryMapping);
                    }

                    foreach (var pa in item.ProductAttributes!)
                    {
                        var productAttributeId = (await _productAttributeService.GetProductAttributeByName(pa.Name!.ToString()))!.Id;

                        var productAttributeMapping = new ProductAttributeMapping()
                        {
                            ProductId = productId,
                            ProductAttributeId = productAttributeId
                        };
                        await _productAttributeService.CreateProductAttributeMappingAsync(productAttributeMapping);

                        var productAttributeMappingId = (await _productAttributeService.GetProductAttributesMappingByProductIdAsync(productId)).Where(x => x.ProductAttributeId == productAttributeId).FirstOrDefault()!.Id;

                        foreach (var pav in item.ProductAttributeValues!)
                        {
                            var productAttributeValue = new ProductAttributeValue()
                            {
                                ProductAttributeMappingId = productAttributeMappingId,
                                Name = pav.Name,
                            };
                            await _productAttributeService.CreateProductAttributeValueAsync(productAttributeValue);
                        }
                    }

                    foreach(var pm in item.Manufacturers!)
                    {
                        var manfacturerId = (await _manufacturerServicesCommon.GetManufacturerByNameAsync(pm.Name!))!.Id;

                        var productManufacturer = new ProductManufacturer()
                        {
                            ProductId = productId,
                            ManufacturerId = manfacturerId
                        };
                        await _manufacturerServicesCommon.CreateProductManufacturerAsync(productManufacturer);

                        var productManfacturer = (await _manufacturerServicesCommon.GetProductManufacturersByManufacturerIdAsync(manfacturerId))
                                                    .Where(x => x.ProductId == productId).FirstOrDefault();
                        ArgumentNullException.ThrowIfNull(productManfacturer);
                    }
                }
            }

        }
        private async Task SeedProductAttributeAsync()
        {

            foreach (var item in ProductAttributesDataSeed.Instance.GetAll())
            {
                await _productAttributeService.CreateProductAttributeAsync(item);
            }
        }
        private async Task<bool> SeedPermission()
        {
            FieldInfo[] fields = typeof(DefaultPermission).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);

            List<PermissionRecord> permission_list = new List<PermissionRecord>();

            foreach (FieldInfo field in fields)
            {
                if (field.FieldType == typeof(PermissionRecord))
                {
                    // Lấy giá trị của trường và thêm vào danh sách
                    PermissionRecord permission = (PermissionRecord)field.GetValue(null)!;
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
                        PermissionRecords = await _securityService.GetAllPermissionRecordAsync()
                    }
                };

            foreach (var item in list)
            {
                foreach (var role in item.Roles!)
                {
                    foreach (var permission in item.PermissionRecords!)
                    {
                        var rolePermissionMapping = new PermissionRecordUserRoleMapping()
                        {
                            PermissionRecordId = (await _securityService.GetPermissionRecordBySystemNameAsync(permission.SystemName)).Id,
                            RoleId = await GetRoleId(_roleManager, role.Name!)
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
                foreach (var item in rolenames)
                {
                    string name = item.GetRawConstantValue()!.ToString()!;
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


            foreach (var item in UserRoleMappingDataSeed.Instance.GetAll())
            {
                foreach (var user in item.Users)
                {
                    //await _userManager.CreateAsync(user, "123321");
                    //var createdUser = await _userManager.Users.AsNoTracking().FirstOrDefaultAsync(x=>x.UserName == user.UserName);
                    //foreach (var role in item.Roles)
                    //{
                    //    try
                    //    {
                    //        await _userManager.AddToRoleAsync(createdUser, role.Name);
                    //    }
                    //    catch(Exception ex)
                    //    {
                    //        Console.WriteLine(ex.Message);
                    //        return;
                    //    }
                    //}
                    var result = await _userManager.CreateAsync(user, "123321");
                    if (result.Succeeded)
                    {
                        foreach (var role in item.Roles)
                        {
                            try
                            {
                                await _userManager.AddToRoleAsync(user, role.Name!);
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