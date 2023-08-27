
using Microsoft.EntityFrameworkCore;
using prj_Traveldate_Core.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
//appsetting
builder.Services.AddDbContext<TraveldateContext>(
    options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("TraveldateConnection"))
    );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();

app.UseRouting();

app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=HomePage}/{action=HomePage}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Forum}/{action=ForumList}/{id?}");

app.Run();
