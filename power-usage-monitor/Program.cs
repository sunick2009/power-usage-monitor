using Highsoft.Web.Mvc.Charts;
using Microsoft.EntityFrameworkCore;
using power_usage_monitor.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
//加入資料收集背景服務
builder.Services.AddHostedService<power_usage_monitor.Controllers.DataCollecterService>();
//加入給資料庫的DI
builder.Services.AddDbContext<power_usage_monitorContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultDatabase") ?? throw new InvalidOperationException("Connection string 'DefaultDatabase' not found.")));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
