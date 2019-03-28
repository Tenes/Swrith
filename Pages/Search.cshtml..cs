using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Roll_Driven_Stories.Classes;

namespace Roll_Driven_Stories.Pages
{
    public class SearchModel : PageModel
    {
        [BindProperty]
        public IList<Post> Posts { get; set; }
        public void OnGet(string search)
        {
            if(String.IsNullOrEmpty(search))
                LoadEmptyArticle();
            else    
                LoadArticlesBySearch(search.ToLower());
        }

        private void LoadArticlesBySearch(string search)
        {
            using (StreamReader streamReader = System.IO.File.OpenText($@"{Startup.ContentRoot}/posts.json"))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                var json = JObject.Load(jsonReader);
                var postArray = (JArray)json["posts"];
                Posts = new JArray(postArray.Where(post => ((string)post["title"]).ToLower()
                    .Contains(search) ||
                    ((string)post["preview"]).ToLower().Contains(search)))
                    .ToObject<IList<Post>>();
            }
        }

        private void LoadEmptyArticle()
        {
            Posts = new List<Post>();
        }
    }
}
