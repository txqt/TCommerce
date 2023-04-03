using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text.Json.Serialization;
using T.Library.Model;
using T.Library.Model.Users;
using T.WebApi.Attribute;
using T.WebApi.Database.ConfigurationDatabase;
using T.WebApi.Extensions;
using T.WebApi.Services;

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
builder.Services.AddRedis(builder.Configuration);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//// Sử dụng JwtBlacklistMiddleware

//app.Use(async (context, next) =>
//{
//    var middleware = app.Services.GetRequiredService<JwtBlacklistMiddleware>();


//    await middleware.InvokeAsync(context);
//});
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<TokenManagerMiddleware>();
app.MapControllers();

app.Run();
