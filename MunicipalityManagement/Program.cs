using Microsoft.EntityFrameworkCore;
using MunicipalityManagement.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(); // This sets up MVC
builder.Services.AddDbContext<MunicipalityDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // To allow for CSS, JS, images

app.UseRouting();                    

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Citizens}/{action=Index}/{id?}");

app.Run();
