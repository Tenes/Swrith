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
        public Post Post { get; set; }
        [BindProperty]
        public Microsoft.AspNetCore.Html.HtmlString ArticleContent { get; set; }
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
            ArticleContent = SystemWatcherUtils.GetHtmlFromMd($@"{Startup.ContentRoot}/{Post.MdPath}");
        }
    }
}