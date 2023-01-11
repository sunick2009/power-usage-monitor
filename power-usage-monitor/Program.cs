using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NETCore.MailKit.Core;
using power_usage_monitor.Models;
using System.Configuration;
using IEmailService = power_usage_monitor.Models.IEmailService;
using power_usage_monitor.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
//加入e-mail發送服務
builder.Services.AddTransient<IEmailService, MailRequest>();
//加入資料收集背景服務
builder.Services.AddHostedService<power_usage_monitor.Controllers.DataCollecterService>();
//加入資料定期分析背景服務
builder.Services.AddTransient<IHostedService, power_usage_monitor.Controllers.DataAnalyzerService>();
//加入給資料庫的DI
builder.Services.AddDbContext<power_usage_monitorContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultDatabase") ?? 
    throw new InvalidOperationException("Connection string 'DefaultDatabase' not found.")));
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
