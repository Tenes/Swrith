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
    public class PageIndexModel : PageModel
    {
        [BindProperty]
        public IList<Post> Posts { get; set; }
        public void OnGet(ushort pageIndex)
        {
            LoadArticlesByPage(pageIndex);
        }

        private void LoadArticlesByPage(ushort pageIndex)
        {
            using (StreamReader streamReader = System.IO.File.OpenText($@"{Startup.ContentRoot}/posts.json"))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                var json = JObject.Load(jsonReader);
                var postArray = JArray.FromObject(json["posts"].Skip((pageIndex - 1) * 6).Take(6));
                Posts = postArray
                    .ToObject<IList<Post>>();
            }
        }
    }
}
