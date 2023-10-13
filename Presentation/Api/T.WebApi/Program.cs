using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using T.WebApi.Extensions;
using T.WebApi.Middleware;
using T.WebApi.Services.DataSeederService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(opt =>
{
    
})
    .AddJsonOptions(o => o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Service extension
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddServices();
builder.Services.AddAuth(builder.Configuration);
builder.Services.AddSwagger(builder.Configuration);
builder.Services.AddIdentityConfig();
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();
builder.Services.AddCustomOptions(builder.Configuration);
builder.Services.AddAutoMapper(typeof(Program).Assembly);

//add this line for custom validate model
builder.Services.Configure<ApiBehaviorOptions>(options
    => options.SuppressModelStateInvalidFilter = true);

builder.Services.AddCors(policy =>
{
    policy.AddPolicy("CorsPolicy", opt => opt
    .AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod()
    .WithExposedHeaders("X-Pagination"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Resolve DataSeeder và gọi phương thức Initialize để seed dữ liệu
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dataSeeder = services.GetRequiredService<DataSeeder>();
    await dataSeeder.Initialize();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("CorsPolicy");
//app.UseMiddleware<TokenManagerMiddleware>();
app.ConfigureCustomExceptionMiddleware();

app.MapControllers();

app.Run();
