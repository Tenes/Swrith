using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ImageMagick;

namespace Roll_Driven_Stories
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public static string ContentRoot { get; set; }
        public static FileSystemWatcher Watcher { get; set; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            ContentRoot = configuration.GetValue<string>(WebHostDefaults.ContentRootKey);
            SetupSystemWatcher();
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            ContentRoot = env.ContentRootPath;
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc();
        }


        private void SetupSystemWatcher()
        {
            Watcher = new FileSystemWatcher();
            Watcher.Path = Path.Combine(ContentRoot, "wwwroot", "images", "raw");
            Watcher.NotifyFilter = NotifyFilters.FileName;
            Watcher.Filter = "*.jpg";
            Watcher.Created += OnCreate;
            Watcher.EnableRaisingEvents = true;
        }

        private static void OnCreate(object source, FileSystemEventArgs e)
        {
            Console.WriteLine($"File: {e.FullPath} {e.ChangeType}");
            CompressImage(e.FullPath);
            File.Delete(e.FullPath);
        }

        private static void CompressImage(string filePath)
        {
            FileInfo snakewareLogo = new FileInfo(filePath);
            using (var magickImage = new MagickImage(snakewareLogo))
            {
                ImageOptimizer optimizer = new ImageOptimizer();
                optimizer.LosslessCompress(filePath);
                magickImage.Format = MagickFormat.Jpg;
                magickImage.Quality = 80;
                magickImage.Strip();
                magickImage.Write(filePath.Replace("raw", "compressed"));
            }
            ResizeImage(filePath.Replace("raw", "compressed"));
        }

        private static void ResizeImage(string filePath)
        {
            using (var image = new MagickImage(new FileInfo(filePath)))
            {
                var size = new MagickGeometry(50, 50);
                size.IgnoreAspectRatio = true;
                image.Resize(size);
                image.Write(filePath.Insert(filePath.LastIndexOf('.'), $"50x50"));
            }
        }
    }
}
