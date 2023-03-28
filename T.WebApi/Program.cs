using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using T.Library.Model.Users;
using T.WebApi.Database.ConfigurationDatabase;
using T.WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Service extension
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddServices();
builder.Services.AddAuth(builder.Configuration);
builder.Services.AddSwagger(builder.Configuration);
builder.Services.AddIdentityConfig();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
