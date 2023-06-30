using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace T.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MetaKeywords = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MetaDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MetaTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: false),
                    PictureId = table.Column<int>(type: "int", nullable: false),
                    ShowOnHomepage = table.Column<bool>(type: "bit", nullable: false),
                    IncludeInTopMenu = table.Column<bool>(type: "bit", nullable: false),
                    Published = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PriceRangeFiltering = table.Column<bool>(type: "bit", nullable: false),
                    PriceFrom = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PriceTo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ManuallyPriceRange = table.Column<bool>(type: "bit", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Picture",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MimeType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SeoFilename = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AltAttribute = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TitleAttribute = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsNew = table.Column<bool>(type: "bit", nullable: false),
                    UrlPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Picture", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VisibleIndividually = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    ShortDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdminComment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShowOnHomepage = table.Column<bool>(type: "bit", nullable: false),
                    MetaKeywords = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MetaDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MetaTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AllowUserReviews = table.Column<bool>(type: "bit", nullable: false),
                    Sku = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsShipEnabled = table.Column<bool>(type: "bit", nullable: false),
                    IsFreeShipping = table.Column<bool>(type: "bit", nullable: false),
                    StockQuantity = table.Column<int>(type: "int", nullable: false),
                    OrderMinimumQuantity = table.Column<int>(type: "int", nullable: false),
                    OrderMaximumQuantity = table.Column<int>(type: "int", nullable: false),
                    NotReturnable = table.Column<bool>(type: "bit", nullable: false),
                    DisableBuyButton = table.Column<bool>(type: "bit", nullable: false),
                    DisableWishlistButton = table.Column<bool>(type: "bit", nullable: false),
                    AvailableForPreOrder = table.Column<bool>(type: "bit", nullable: false),
                    PreOrderAvailabilityStartDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OldPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MarkAsNew = table.Column<bool>(type: "bit", nullable: false),
                    MarkAsNewStartDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MarkAsNewEndDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Weight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Length = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Width = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Height = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AvailableStartDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AvailableEndDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    Published = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductAttribute",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAttribute", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShoppingCartItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShoppingCartTypeId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    UserEnteredPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    RentalStartDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RentalEndDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ShoppingCartType = table.Column<int>(type: "int", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingCartItem", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Dob = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AvatarPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequirePasswordChange = table.Column<bool>(type: "bit", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Product_ProductCategory_Mapping",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    IsFeaturedProduct = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product_ProductCategory_Mapping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Product_ProductCategory_Mapping_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Product_ProductCategory_Mapping_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Product_ProductPicture_Mapping",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    PictureId = table.Column<int>(type: "int", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product_ProductPicture_Mapping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Product_ProductPicture_Mapping_Picture_PictureId",
                        column: x => x.PictureId,
                        principalTable: "Picture",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Product_ProductPicture_Mapping_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductReview",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReplyText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerNotifiedOfReply = table.Column<bool>(type: "bit", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    HelpfulYesTotal = table.Column<int>(type: "int", nullable: false),
                    HelpfulNoTotal = table.Column<int>(type: "int", nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductReview", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductReview_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Product_ProductAttribute_Mapping",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ProductAttributeId = table.Column<int>(type: "int", nullable: false),
                    TextPrompt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    AttributeControlTypeId = table.Column<int>(type: "int", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    ValidationMinLength = table.Column<int>(type: "int", nullable: true),
                    ValidationMaxLength = table.Column<int>(type: "int", nullable: true),
                    ValidationFileAllowedExtensions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValidationFileMaximumSize = table.Column<int>(type: "int", nullable: true),
                    DefaultValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConditionAttributeSelected = table.Column<int>(type: "int", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product_ProductAttribute_Mapping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Product_ProductAttribute_Mapping_ProductAttribute_ProductAttributeId",
                        column: x => x.ProductAttributeId,
                        principalTable: "ProductAttribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Product_ProductAttribute_Mapping_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShoppingCartItemAttributeValue",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShoppingCartItemId = table.Column<int>(type: "int", nullable: false),
                    ProductAttributeId = table.Column<int>(type: "int", nullable: false),
                    ProductAttributeValueId = table.Column<int>(type: "int", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingCartItemAttributeValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShoppingCartItemAttributeValue_ShoppingCartItem_ShoppingCartItemId",
                        column: x => x.ShoppingCartItemId,
                        principalTable: "ShoppingCartItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UserLogins_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_UserTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductReviewHelpfulness",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductReviewId = table.Column<int>(type: "int", nullable: false),
                    WasHelpful = table.Column<bool>(type: "bit", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductReviewHelpfulness", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductReviewHelpfulness_ProductReview_ProductReviewId",
                        column: x => x.ProductReviewId,
                        principalTable: "ProductReview",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductAttributeValue",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductAttributeMappingId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    ColorSquaresRgb = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PriceAdjustment = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PriceAdjustmentUsePercentage = table.Column<bool>(type: "bit", nullable: false),
                    WeightAdjustment = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CustomerEntersQty = table.Column<bool>(type: "bit", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    IsPreSelected = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    PictureId = table.Column<int>(type: "int", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAttributeValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductAttributeValue_Product_ProductAttribute_Mapping_ProductAttributeMappingId",
                        column: x => x.ProductAttributeMappingId,
                        principalTable: "Product_ProductAttribute_Mapping",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Category",
                columns: new[] { "Id", "CreatedOnUtc", "Deleted", "Description", "DisplayOrder", "IncludeInTopMenu", "ManuallyPriceRange", "MetaDescription", "MetaKeywords", "MetaTitle", "Name", "ParentCategoryId", "PictureId", "PriceFrom", "PriceRangeFiltering", "PriceTo", "Published", "ShowOnHomepage", "UpdatedOnUtc" },
                values: new object[,]
                {
                    { 1, new DateTime(2023, 6, 29, 21, 37, 23, 284, DateTimeKind.Utc).AddTicks(3446), false, null, 0, false, false, null, null, null, "Thời trang", 0, 0, 0m, false, 0m, false, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, new DateTime(2023, 6, 29, 21, 37, 23, 284, DateTimeKind.Utc).AddTicks(3451), false, null, 0, false, false, null, null, null, "Điện tử", 0, 0, 0m, false, 0m, false, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, new DateTime(2023, 6, 29, 21, 37, 23, 284, DateTimeKind.Utc).AddTicks(3452), false, null, 0, false, false, null, null, null, "Điện gia dụng", 0, 0, 0m, false, 0m, false, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, new DateTime(2023, 6, 29, 21, 37, 23, 284, DateTimeKind.Utc).AddTicks(3453), false, null, 0, false, false, null, null, null, "Âm thanh", 0, 0, 0m, false, 0m, false, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, new DateTime(2023, 6, 29, 21, 37, 23, 284, DateTimeKind.Utc).AddTicks(3454), false, null, 0, false, false, null, null, null, "Phụ kiện", 0, 0, 0m, false, 0m, false, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Product",
                columns: new[] { "Id", "AdminComment", "AllowUserReviews", "AvailableEndDateTimeUtc", "AvailableForPreOrder", "AvailableStartDateTimeUtc", "CreatedOnUtc", "Deleted", "DisableBuyButton", "DisableWishlistButton", "DisplayOrder", "FullDescription", "Height", "IsFreeShipping", "IsShipEnabled", "Length", "MarkAsNew", "MarkAsNewEndDateTimeUtc", "MarkAsNewStartDateTimeUtc", "MetaDescription", "MetaKeywords", "MetaTitle", "Name", "NotReturnable", "OldPrice", "OrderMaximumQuantity", "OrderMinimumQuantity", "PreOrderAvailabilityStartDateTimeUtc", "Price", "Published", "ShortDescription", "ShowOnHomepage", "Sku", "StockQuantity", "UpdatedOnUtc", "VisibleIndividually", "Weight", "Width" },
                values: new object[,]
                {
                    { 1, null, false, null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, false, false, 0, "Full Description", 0m, false, false, 0m, false, null, null, null, null, null, "Áo thun nam", false, 0m, 0, 0, null, 200000m, false, "Áo thun nam hàng hiệu", false, null, 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 0m, 0m },
                    { 2, null, false, null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, false, false, 0, "Full Description", 0m, false, false, 0m, false, null, null, null, null, null, "Áo khoác nữ", false, 0m, 0, 0, null, 500000m, false, "Áo khoác dành cho nữ thời trang", false, null, 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 0m, 0m },
                    { 3, null, false, null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, false, false, 0, "Full Description", 0m, false, false, 0m, false, null, null, null, null, null, "Quần jean nam", false, 0m, 0, 0, null, 400000m, false, "Quần jean nam hàng hiệu", false, null, 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 0m, 0m },
                    { 4, null, false, null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, false, false, 0, "Full Description", 0m, false, false, 0m, false, null, null, null, null, null, "Váy đầm dự tiệc", false, 0m, 0, 0, null, 1000000m, false, "Váy đầm dự tiệc sang trọng", false, null, 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 0m, 0m },
                    { 5, null, false, null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, false, false, 0, "Full Description", 0m, false, false, 0m, false, null, null, null, null, null, "Giày thể thao nam", false, 0m, 0, 0, null, 800000m, false, "Giày thể thao nam Adidas", false, null, 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 0m, 0m },
                    { 6, null, false, null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, false, false, 0, "Full Description", 0m, false, false, 0m, false, null, null, null, null, null, "Giày cao gót nữ", false, 0m, 0, 0, null, 700000m, false, "Giày cao gót nữ đẹp", false, null, 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 0m, 0m },
                    { 7, null, false, null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, false, false, 0, "Full Description", 0m, false, false, 0m, false, null, null, null, null, null, "Túi xách nữ", false, 0m, 0, 0, null, 600000m, false, "Túi xách nữ hàng hiệu", false, null, 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 0m, 0m },
                    { 8, null, false, null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, false, false, 0, "Full Description", 0m, false, false, 0m, false, null, null, null, null, null, "Mũ len nam", false, 0m, 0, 0, null, 100000m, false, "Mũ len nam giữ ấm", false, null, 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 0m, 0m },
                    { 9, null, false, null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, false, false, 0, "Full Description", 0m, false, false, 0m, false, null, null, null, null, null, "Khẩu trang y tế", false, 0m, 0, 0, null, 5000m, false, "Khẩu trang y tế 3 lớp", false, null, 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 0m, 0m },
                    { 10, null, false, null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, false, false, 0, "Full Description", 0m, false, false, 0m, false, null, null, null, null, null, "Balo laptop", false, 0m, 0, 0, null, 900000m, false, "Balo laptop chống sốc", false, null, 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 0m, 0m },
                    { 11, null, false, null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, false, false, 0, "Full Description", 0m, false, false, 0m, false, null, null, null, null, null, "Tủ lạnh", false, 0m, 0, 0, null, 15000000m, false, "Tủ lạnh side by side LG", false, null, 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 0m, 0m },
                    { 12, null, false, null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, false, false, 0, "Full Description", 0m, false, false, 0m, false, null, null, null, null, null, "Máy giặt", false, 0m, 0, 0, null, 10000000m, false, "Máy giặt Samsung", false, null, 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 0m, 0m },
                    { 13, null, false, null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, false, false, 0, "Full Description", 0m, false, false, 0m, false, null, null, null, null, null, "Điều hòa", false, 0m, 0, 0, null, 8000000m, false, "Điều hòa Panasonic Inverter", false, null, 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 0m, 0m },
                    { 14, null, false, null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, false, false, 0, "Full Description", 0m, false, false, 0m, false, null, null, null, null, null, "Tivi OLED", false, 0m, 0, 0, null, 30000000m, false, "Tivi OLED Sony 65 inch", false, null, 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 0m, 0m },
                    { 15, null, false, null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, false, false, 0, "Full Description", 0m, false, false, 0m, false, null, null, null, null, null, "Máy ảnh DSLR", false, 0m, 0, 0, null, 15000000m, false, "Máy ảnh Canon EOS 90D", false, null, 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 0m, 0m },
                    { 16, null, false, null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, false, false, 0, "Full Description", 0m, false, false, 0m, false, null, null, null, null, null, "Loa bluetooth", false, 0m, 0, 0, null, 2000000m, false, "Loa bluetooth JBL Flip 5", false, null, 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 0m, 0m },
                    { 17, null, false, null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, false, false, 0, "Full Description", 0m, false, false, 0m, false, null, null, null, null, null, "Tai nghe true wireless", false, 0m, 0, 0, null, 5000000m, false, "Tai nghe true wireless Apple AirPods Pro", false, null, 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 0m, 0m },
                    { 18, null, false, null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, false, false, 0, "Full Description", 0m, false, false, 0m, false, null, null, null, null, null, "Chuột gaming", false, 0m, 0, 0, null, 1000000m, false, "Chuột gaming", false, null, 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 0m, 0m }
                });

            migrationBuilder.InsertData(
                table: "ProductAttribute",
                columns: new[] { "Id", "Deleted", "Description", "Name" },
                values: new object[] { 1, false, "Thuộc tính màu sắc của sản phẩm", "Màu sắc" });

            migrationBuilder.InsertData(
                table: "Product_ProductAttribute_Mapping",
                columns: new[] { "Id", "AttributeControlTypeId", "ConditionAttributeSelected", "DefaultValue", "Deleted", "DisplayOrder", "IsRequired", "ProductAttributeId", "ProductId", "TextPrompt", "ValidationFileAllowedExtensions", "ValidationFileMaximumSize", "ValidationMaxLength", "ValidationMinLength" },
                values: new object[,]
                {
                    { 1, 0, 0, null, false, 0, false, 1, 1, null, null, null, null, null },
                    { 2, 0, 0, null, false, 0, false, 1, 2, null, null, null, null, null },
                    { 3, 0, 0, null, false, 0, false, 1, 3, null, null, null, null, null },
                    { 4, 0, 0, null, false, 0, false, 1, 4, null, null, null, null, null },
                    { 5, 0, 0, null, false, 0, false, 1, 5, null, null, null, null, null },
                    { 6, 0, 0, null, false, 0, false, 1, 6, null, null, null, null, null },
                    { 7, 0, 0, null, false, 0, false, 1, 7, null, null, null, null, null },
                    { 8, 0, 0, null, false, 0, false, 1, 8, null, null, null, null, null },
                    { 9, 0, 0, null, false, 0, false, 1, 9, null, null, null, null, null },
                    { 10, 0, 0, null, false, 0, false, 1, 10, null, null, null, null, null },
                    { 11, 0, 0, null, false, 0, false, 1, 11, null, null, null, null, null },
                    { 12, 0, 0, null, false, 0, false, 1, 12, null, null, null, null, null },
                    { 13, 0, 0, null, false, 0, false, 1, 13, null, null, null, null, null },
                    { 14, 0, 0, null, false, 0, false, 1, 14, null, null, null, null, null },
                    { 15, 0, 0, null, false, 0, false, 1, 15, null, null, null, null, null },
                    { 16, 0, 0, null, false, 0, false, 1, 16, null, null, null, null, null },
                    { 17, 0, 0, null, false, 0, false, 1, 17, null, null, null, null, null },
                    { 18, 0, 0, null, false, 0, false, 1, 18, null, null, null, null, null }
                });

            migrationBuilder.InsertData(
                table: "Product_ProductCategory_Mapping",
                columns: new[] { "Id", "CategoryId", "Deleted", "DisplayOrder", "IsFeaturedProduct", "ProductId" },
                values: new object[,]
                {
                    { 1, 1, false, 0, false, 1 },
                    { 2, 1, false, 0, false, 2 },
                    { 3, 1, false, 0, false, 3 },
                    { 4, 1, false, 0, false, 4 },
                    { 5, 1, false, 0, false, 5 },
                    { 6, 1, false, 0, false, 6 },
                    { 7, 1, false, 0, false, 7 },
                    { 8, 1, false, 0, false, 8 },
                    { 9, 1, false, 0, false, 9 },
                    { 10, 1, false, 0, false, 10 },
                    { 11, 2, false, 0, false, 11 },
                    { 12, 3, false, 0, false, 12 },
                    { 13, 3, false, 0, false, 13 },
                    { 14, 2, false, 0, false, 14 },
                    { 15, 2, false, 0, false, 15 },
                    { 16, 4, false, 0, false, 16 },
                    { 17, 4, false, 0, false, 17 },
                    { 18, 2, false, 0, false, 18 }
                });

            migrationBuilder.InsertData(
                table: "ProductAttributeValue",
                columns: new[] { "Id", "ColorSquaresRgb", "Cost", "CustomerEntersQty", "Deleted", "DisplayOrder", "IsPreSelected", "Name", "PictureId", "PriceAdjustment", "PriceAdjustmentUsePercentage", "ProductAttributeMappingId", "Quantity", "WeightAdjustment" },
                values: new object[,]
                {
                    { 1, null, 0m, false, false, 0, false, "Đỏ", 0, 0m, false, 1, 0, 0m },
                    { 2, null, 0m, false, false, 0, false, "Xanh", 0, 0m, false, 1, 0, 0m },
                    { 3, null, 0m, false, false, 0, false, "Vàng", 0, 0m, false, 1, 0, 0m },
                    { 4, null, 0m, false, false, 0, false, "Tím", 0, 0m, false, 2, 0, 0m },
                    { 5, null, 0m, false, false, 0, false, "Đen", 0, 0m, false, 2, 0, 0m },
                    { 6, null, 0m, false, false, 0, false, "Vàng", 0, 0m, false, 2, 0, 0m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Product_ProductAttribute_Mapping_ProductAttributeId",
                table: "Product_ProductAttribute_Mapping",
                column: "ProductAttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_ProductAttribute_Mapping_ProductId",
                table: "Product_ProductAttribute_Mapping",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_ProductCategory_Mapping_CategoryId",
                table: "Product_ProductCategory_Mapping",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_ProductCategory_Mapping_ProductId",
                table: "Product_ProductCategory_Mapping",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_ProductPicture_Mapping_PictureId",
                table: "Product_ProductPicture_Mapping",
                column: "PictureId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_ProductPicture_Mapping_ProductId",
                table: "Product_ProductPicture_Mapping",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributeValue_ProductAttributeMappingId",
                table: "ProductAttributeValue",
                column: "ProductAttributeMappingId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReview_ProductId",
                table: "ProductReview",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReviewHelpfulness_ProductReviewId",
                table: "ProductReviewHelpfulness",
                column: "ProductReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_RoleId",
                table: "RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Roles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCartItemAttributeValue_ShoppingCartItemId",
                table: "ShoppingCartItemAttributeValue",
                column: "ShoppingCartItemId");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId",
                table: "UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_UserId",
                table: "UserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Users",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Product_ProductCategory_Mapping");

            migrationBuilder.DropTable(
                name: "Product_ProductPicture_Mapping");

            migrationBuilder.DropTable(
                name: "ProductAttributeValue");

            migrationBuilder.DropTable(
                name: "ProductReviewHelpfulness");

            migrationBuilder.DropTable(
                name: "RoleClaims");

            migrationBuilder.DropTable(
                name: "ShoppingCartItemAttributeValue");

            migrationBuilder.DropTable(
                name: "UserClaims");

            migrationBuilder.DropTable(
                name: "UserLogins");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UserTokens");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "Picture");

            migrationBuilder.DropTable(
                name: "Product_ProductAttribute_Mapping");

            migrationBuilder.DropTable(
                name: "ProductReview");

            migrationBuilder.DropTable(
                name: "ShoppingCartItem");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "ProductAttribute");

            migrationBuilder.DropTable(
                name: "Product");
        }
    }
}
