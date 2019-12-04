using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Html;
using Swrith.Classes;

namespace Swrith.Utils
{
    public static class PostUtils
    {
        public const byte DISPLAYED_POSTS = 6;
        public static IList<Post> TotalPosts { get; set; } = new List<Post>();
        public static Dictionary<string, HtmlString> PostsContent { get; set; } = new Dictionary<string, HtmlString>();
        public static string LastAddedSlug { get; set; }
        public static string JsonPath { get { return Path.Combine(Startup.ContentRoot, "posts.json"); } }

        public static JsonElement GetJsonRoot()
        {
            var jsonBytes = System.IO.File.ReadAllBytes(JsonPath);
            var json = JsonDocument.Parse(jsonBytes);
            return json.RootElement;
        }
        public static Post LoadPost(JsonElement rootElement, string slug)
        {
            if(rootElement.TryGetProperty("posts", out JsonElement posts))
                return posts
                        .EnumerateArray()
                        .FirstOrDefault(child => child.GetProperty("slug").ToString() == slug)
                        .ToObject<Post>();
            return null;
        }

        public static string GetMdPathFromSlug(JsonElement rootElement, string slug) => 
            rootElement.GetProperty("posts")
                        .EnumerateArray()
                        .FirstOrDefault(child => child.GetProperty("slug").ToString() == slug)
                        .GetProperty("md_path")
                        .ToString();


        public static void LoadArticles()
        {
            var jsonBytes = System.IO.File.ReadAllBytes(JsonPath);
            var json = JsonDocument.Parse(jsonBytes);
            var root = json.RootElement;
            if(root.TryGetProperty("posts", out JsonElement jsonPosts))
                TotalPosts = jsonPosts.ToObject<List<Post>>();
        }

        public static void LoadPostsContent()
        {
            var jsonRoot = GetJsonRoot();
            for(ushort index = 0; index < DISPLAYED_POSTS; index++)
            {
                if(TotalPosts.Count() - 1 < index)
                    break;
                var currentSlug = TotalPosts[index].Slug;
                PostsContent.Add(currentSlug, SystemWatcherUtils.GetHtmlFromMd
                (
                    Path.Combine(Startup.ContentRoot, GetMdPathFromSlug(jsonRoot, currentSlug)))
                );
                LastAddedSlug = currentSlug;
            }
        }
    }
}
