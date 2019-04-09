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
using Dice_Driven_Stories.Extensions;
using Dice_Driven_Stories.Classes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ServiceModel.Syndication;
using System.Xml;

namespace Dice_Driven_Stories
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public static string ContentRoot { get; set; }
        public static FileSystemWatcher ImageWatcher { get; set; }
        public static FileSystemWatcher ArticleWatcher { get; set; }
        public static IList<Post> TotalPosts { get; set; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            ContentRoot = configuration.GetValue<string>(WebHostDefaults.ContentRootKey);
            SystemWatcherUtils.SetupImageWatcher();
            SystemWatcherUtils.SetupArticleWatcher();
            LoadArticles();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
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
            }
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc();
        }

        public static void LoadArticles()
        {
            using (StreamReader streamReader = System.IO.File.OpenText($@"{ContentRoot}/posts.json"))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                var json = JObject.Load(jsonReader);
                TotalPosts = JArray.FromObject(json["posts"]).ToObject<IList<Post>>();
            }
        }
    }
}
