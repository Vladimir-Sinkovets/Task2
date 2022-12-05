using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Task2.Data;
using Task2.Services.ExcelFileManagers.Options;
using Task2.Services.ExcelFileManagers;

namespace Task2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var builder = WebApplication.CreateBuilder(args);




            // Add services to the container.

            string connection = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<ApplicationDbContext>(opt =>
            {
                opt.UseSqlServer(connection);
            });

            builder.Services.Configure<FilesSaveSettings>(builder.Configuration.GetSection("FilesSaveSettings"));

            builder.Services.AddControllersWithViews();

            builder.Services.AddTransient<IExcelFileManager, ExcelFileManager>();

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

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}