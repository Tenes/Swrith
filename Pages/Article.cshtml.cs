using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Swrith.Classes;
using Swrith.Utils;

namespace Swrith.Pages
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
            if (PostUtils.PostsContent.ContainsKey(slug))
            {
                PostTitle = (char.ToUpper(slug[0]) + slug.Substring(1)).Replace('-', ' ');
                ArticleContent = PostUtils.PostsContent[slug];
            }
            else
            {
                Post post = PostUtils.LoadPost(PostUtils.GetJsonRoot(), slug);
                ArticleContent = SystemWatcherUtils.GetHtmlFromMd(Path.Combine(Startup.ContentRoot, post.MdPath));
                PostTitle = post.Title;
                if (PostUtils.PostsContent.Count == 5)
                    PostUtils.PostsContent.Remove(PostUtils.LastAddedSlug);
                PostUtils.PostsContent.Add(slug, ArticleContent);
            }
        }
    }
}