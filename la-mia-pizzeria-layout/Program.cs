using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Test_MVC_2.Data;
using Microsoft.AspNetCore.Identity;
using Test_MVC_2;

namespace Test_MVC_2
{
        public class Program
        {
            public static void Main(string[] args)
            {

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<PizzaContext>();

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<PizzaContext>();

                // Add services to the container.
                builder.Services.AddControllersWithViews();

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

                app.UseAuthentication();

                app.UseAuthorization();

                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Pizza}/{action=Index}/{id?}");
                app.MapRazorPages();

            app.Run();
            }
        }
}

