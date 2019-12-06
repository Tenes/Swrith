using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swrith.Utils;

namespace Swrith
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public static string ContentRoot { get; set; }
        public static FileSystemWatcher ImageWatcher { get; set; }
        public static FileSystemWatcher ArticleWatcher { get; set; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            ContentRoot = configuration.GetValue<string>(WebHostDefaults.ContentRootKey);
            SystemWatcherUtils.SetupImageWatcher();
            SystemWatcherUtils.SetupArticleWatcher();
            PostUtils.LoadArticles();
            if(PostUtils.TotalPosts.Any())
                PostUtils.LoadPostsContent();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddRazorPages()
                .AddNewtonsoftJson();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            ContentRoot = env.ContentRootPath;
            app.UseRouting();
            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
