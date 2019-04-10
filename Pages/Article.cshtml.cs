using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Dice_Driven_Stories.Classes;
using Dice_Driven_Stories.Extensions;
using Newtonsoft.Json.Linq;

namespace Dice_Driven_Stories.Pages
{
    public class ArticleModel : PageModel
    {
        [BindProperty]
        public string PostTitle { get; set; }
        [BindProperty]
        public Microsoft.AspNetCore.Html.HtmlString ArticleContent { get; set; }
        public void OnGet(string slug)
        {
            LoadArticle(slug);
        }

        private void LoadArticle(string slug)
        {
            if(Startup.PostsContent.ContainsKey(slug))
            {
                PostTitle = (char.ToUpper(slug[0]) + slug.Substring(1)).Replace('-', ' ');
                ArticleContent = Startup.PostsContent[slug];
            }
            else
            {
                Post post;
                using (StreamReader streamReader = System.IO.File.OpenText(Path.Combine(Startup.ContentRoot, "posts.json")))
                using (var jsonReader = new JsonTextReader(streamReader))
                {
                    var json = JObject.Load(jsonReader);
                    post = json["posts"].Children<JObject>()
                            .FirstOrDefault(child => (string)child["slug"] == slug).ToObject<Post>();
                }
                ArticleContent = SystemWatcherUtils.GetHtmlFromMd(Path.Combine(Startup.ContentRoot, post.MdPath));
                PostTitle = post.Title;
                if(Startup.PostsContent.Count == 5)
                    Startup.PostsContent.Remove(Startup.LastAddedSlug);
                Startup.PostsContent.Add(slug, ArticleContent);
            }
        }
    }
}