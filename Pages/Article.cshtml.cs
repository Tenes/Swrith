using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Roll_Driven_Stories.Classes;
using Roll_Driven_Stories.Extensions;

namespace Roll_Driven_Stories.Pages
{
    public class ArticleModel : PageModel
    {
        [BindProperty]
        public Post Post { get; set; }
        [BindProperty]
        public Microsoft.AspNetCore.Html.HtmlString ArticleContent {get; set;}
        public void OnGet(string slug)
        {
            LoadArticle(slug);
        }

        private void LoadArticle(string slug)
        {
            using (StreamReader streamReader = System.IO.File.OpenText($@"{Startup.ContentRoot}/posts.json"))
            using (var jsonReader = new Newtonsoft.Json.JsonTextReader(streamReader))
            {
                var json = Newtonsoft.Json.Linq.JObject.Load(jsonReader);
                Post = json["posts"].Children<Newtonsoft.Json.Linq.JObject>()
                        .FirstOrDefault(child => (string)child["slug"] == slug).ToObject<Post>();
            }
            LoadArticleContent();
        }
        private void LoadArticleContent()
        {
            ArticleContent = Markdown.ParseHtmlString(System.IO.File.ReadAllText($@"{Startup.ContentRoot}/{Post.MdPath}"));
        }
    }
}
