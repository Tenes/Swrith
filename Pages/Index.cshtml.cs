using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Roll_Driven_Stories.Classes;

namespace Roll_Driven_Stories.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public IList<Post> Posts { get; set; }
        public int count = 0;
        public void OnGet()
        {
            LoadArticles();
            count += 1;
        }

        private void LoadArticles()
        {
            using (StreamReader streamReader = System.IO.File.OpenText($@"{Startup.ContentRoot}/posts.json"))
            using (var jsonReader = new Newtonsoft.Json.JsonTextReader(streamReader))
            {
                var json = Newtonsoft.Json.Linq.JObject.Load(jsonReader);
                var postArray = (Newtonsoft.Json.Linq.JArray)json["posts"];
                Posts = postArray.ToObject<IList<Post>>();
            }
        }
    }
}
