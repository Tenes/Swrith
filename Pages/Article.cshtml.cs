using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Roll_Driven_Stories.Classes;
using Roll_Driven_Stories.Extensions;
using Newtonsoft.Json.Linq;

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
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                var json = JObject.Load(jsonReader);
                Post = json["posts"].Children<JObject>()
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
