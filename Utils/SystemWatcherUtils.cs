using System;
using System.IO;
using System.Text.RegularExpressions;
using ImageMagick;
using Microsoft.AspNetCore.Html;
using System.Text.Json;
using Swrith.Classes;
using System.ServiceModel.Syndication;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Swrith.Utils
{
    internal static class SystemWatcherUtils
    {
        public static void SetupImageWatcher()
        {
            Startup.ImageWatcher = new FileSystemWatcher();
            Startup.ImageWatcher.Path = Path.Combine(Startup.ContentRoot, "wwwroot", "images", "raw");
            Startup.ImageWatcher.NotifyFilter = NotifyFilters.FileName;
            Startup.ImageWatcher.Filter = "*.jpg";
            Startup.ImageWatcher.Created += OnCreatePng;
            Startup.ImageWatcher.EnableRaisingEvents = true;
        }

        private static void OnCreatePng(object source, FileSystemEventArgs e)
        {
            Console.WriteLine($"File: {e.FullPath} {e.ChangeType}");
            CompressImage(e.FullPath);
            File.Delete(e.FullPath);
            Console.WriteLine($"File: {e.Name} compressed and resized successfully.");
        }

        public static void SetupArticleWatcher()
        {
            Startup.ArticleWatcher = new FileSystemWatcher();
            Startup.ArticleWatcher.Path = Path.Combine(Startup.ContentRoot, "articles");
            Startup.ArticleWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size;
            Startup.ArticleWatcher.Filter = "*.md";
            Startup.ArticleWatcher.Changed += OnChangedArticle;
            Startup.ArticleWatcher.EnableRaisingEvents = true;
        }

        private static void OnChangedArticle(object source, FileSystemEventArgs e)
        {
            Console.WriteLine($"File: {e.FullPath} {e.ChangeType}");
            IncludeArticleInJson(e.FullPath);
        }

        private static void CompressImage(string filePath)
        {
            FileInfo imageToCompress = new FileInfo(filePath);
            using (var magickImage = new MagickImage(imageToCompress))
            {
                ImageOptimizer optimizer = new ImageOptimizer();
                optimizer.LosslessCompress(filePath);
                magickImage.Format = MagickFormat.Jpg;
                magickImage.Quality = 80;
                magickImage.Strip();
                magickImage.Write(filePath.Replace("raw", "compressed"));
            }
            ResizeImage(filePath.Replace("raw", "compressed"));
        }

        private static void ResizeImage(string filePath)
        {
            using (var image = new MagickImage(new FileInfo(filePath)))
            {
                var size = new MagickGeometry(50, 50);
                size.IgnoreAspectRatio = true;
                image.Resize(size);
                image.Write(filePath.Insert(filePath.LastIndexOf('.'), $"50x50"));
            }
        }

        // New but not yet fully functionnal in .net core 3.0
        // private static void IncludeArticleInJson(string articlePath)
        // {
        //     if (System.IO.File.ReadAllText(articlePath).Length == 0)
        //         return;
        //     FileInfo file = new FileInfo(articlePath);
        //     string articleContent = Regex.Replace(GetTextFromMd(articlePath), "<[^>]*>", String.Empty);
        //     string nameNoExtension = file.Name.Substring(0, file.Name.LastIndexOf('.'));

        //     Post postToAdd = new Post(
        //         nameNoExtension,
        //         (char.ToUpper(nameNoExtension[0]) + nameNoExtension.Substring(1)).Replace('-', ' '),
        //         articleContent.Substring(0, 230).Replace("&quot;", "\""),
        //         Path.Combine("articles", file.Name),
        //         Path.Combine(Path.DirectorySeparatorChar.ToString(), "images", "compressed", nameNoExtension + "50x50.jpg"),
        //         DateTime.Now.ToShortDateString(),
        //         articleContent.Substring(articleContent.LastIndexOf("Tags:") + 5,
        //             articleContent.LastIndexOf('.') - articleContent.LastIndexOf("Tags:") - 5)
        //     );
        //     var jsonRoot = PostUtils.GetJsonRoot();
        //     var existingPost = PostUtils.LoadPost(jsonRoot, nameNoExtension);
        //     var updatedPosts = jsonRoot.ToObject<List<Post>>();
        //     if (existingPost != null)
        //     {
        //         postToAdd.PublishedDate = existingPost.PublishedDate;
        //         PostUtils.TotalPosts[PostUtils.TotalPosts.IndexOf(PostUtils.TotalPosts.First(post => post.Slug.Equals(postToAdd.Slug)))] = postToAdd;
        //         updatedPosts[updatedPosts.IndexOf(updatedPosts.First(post => post.Slug.Equals(postToAdd.Slug)))] = postToAdd;
        //         if (PostUtils.PostsContent.ContainsKey(postToAdd.Slug))
        //             PostUtils.PostsContent[postToAdd.Slug] = SystemWatcherUtils.GetHtmlFromMd(Path.Combine(Startup.ContentRoot, postToAdd.MdPath));
        //     }
        //     else
        //     {
        //         updatedPosts.Insert(0, postToAdd);
        //         PostUtils.TotalPosts.Insert(0, postToAdd);
        //     }
        //     var options = new JsonSerializerOptions
        //     {
        //         WriteIndented = true
        //     };
        //     System.IO.File.WriteAllText(PostUtils.JsonPath, JsonSerializer.Serialize(jsonRoot, options));
        //     System.Console.WriteLine($"JSON Updated for the new article.");
        //     AppendToRss(articlePath, nameNoExtension);
        // }
        private static void IncludeArticleInJson(string articlePath)
        {
            if(System.IO.File.ReadAllText(articlePath).Length == 0)
                return;
            JObject json;
            FileInfo file = new FileInfo(articlePath);
            string articleContent = Regex.Replace(GetTextFromMd(articlePath), "<[^>]*>", String.Empty);
            string nameNoExtension = file.Name.Substring(0, file.Name.LastIndexOf('.'));

            using (StreamReader streamReader = System.IO.File.OpenText($@"{Startup.ContentRoot}/posts.json"))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                json = JObject.Load(jsonReader);
                Post postToAdd = new Post(
                    nameNoExtension,
                    (char.ToUpper(nameNoExtension[0]) + nameNoExtension.Substring(1)).Replace('-', ' '),
                    articleContent.Substring(0, 230).Replace("&quot;", "\""),
                    Path.Combine("articles", file.Name),
                    Path.Combine(Path.DirectorySeparatorChar.ToString(), "images", "compressed", nameNoExtension + "50x50.jpg"),
                    DateTime.Now.ToShortDateString(),
                    articleContent.Substring(articleContent.LastIndexOf("Tags:") + 5,
                        articleContent.LastIndexOf('.') - articleContent.LastIndexOf("Tags:") - 5)
                );
                JToken existingPostAsToken = json["posts"].Children<JObject>().FirstOrDefault(post => ((string)post["slug"]).Equals(nameNoExtension));
                if(existingPostAsToken != null)
                {
                    Post existingPost = existingPostAsToken.ToObject<Post>();
                    postToAdd.PublishedDate = existingPost.PublishedDate;
                    PostUtils.TotalPosts[PostUtils.TotalPosts.IndexOf(PostUtils.TotalPosts.First(post => post.Slug.Equals(postToAdd.Slug)))] = postToAdd;
                    ((JArray)(json["posts"]))[((JArray)(json["posts"])).IndexOf(existingPostAsToken)] = JToken.FromObject(postToAdd);
                    if(PostUtils.PostsContent.ContainsKey(postToAdd.Slug))
                        PostUtils.PostsContent[postToAdd.Slug] = SystemWatcherUtils.GetHtmlFromMd(Path.Combine(Startup.ContentRoot, postToAdd.MdPath));
                }
                else
                {
                    ((JArray)(json["posts"])).Insert(0, JToken.FromObject(postToAdd));
                    PostUtils.TotalPosts.Insert(0, postToAdd);
                }
            }
            System.IO.File.WriteAllText(PostUtils.JsonPath, JsonConvert.SerializeObject(json, Newtonsoft.Json.Formatting.Indented));
            System.Console.WriteLine($"JSON Updated for the new article.");
            AppendToRss(articlePath, nameNoExtension);
        }

        public static void AppendToRss(string filePath, string fileName)
        {
            string rssPath = Path.Combine(Startup.ContentRoot, "wwwroot", "rss.xml");
            XmlReader rssReader = XmlReader.Create(rssPath);
            SyndicationFeed feed = SyndicationFeed.Load(rssReader);
            rssReader.Close();

            SyndicationItem item = new SyndicationItem(char.ToUpper(fileName[0]) + fileName.Substring(1), "", new Uri($"https://swrith.com/read/{fileName}"), fileName, DateTime.Now);

            XmlDocument doc = new XmlDocument();
            XmlElement content = doc.CreateElement("content", "encoded", "http://purl.org/rss/1.0/modules/content/");
            content.InnerText = GetHtmlFromMd(filePath).Value;
            item.ElementExtensions.Add(content);
            List<SyndicationItem> items;
            if (feed.Items != null)
                items = new List<SyndicationItem>(feed.Items);
            else
                items = new List<SyndicationItem>();
            SyndicationItem oldItem = items.FirstOrDefault(olditem => olditem.Id == item.Id);
            if (oldItem != null)
            {
                item.PublishDate = oldItem.PublishDate;
                items[items.IndexOf(oldItem)] = item;
                System.Console.WriteLine($"{fileName} updated in the RSS Feed.");
            }
            else
            {
                items.Add(item);
                System.Console.WriteLine($"{fileName} added to the RSS Feed.");
            }
            feed.LastUpdatedTime = DateTime.Now;
            feed.Items = items;

            XmlWriter rssWriter = XmlWriter.Create(rssPath);
            Rss20FeedFormatter rssFormatter = new Rss20FeedFormatter(feed);
            rssFormatter.WriteTo(rssWriter);
            rssWriter.Close();
        }

        public static HtmlString GetHtmlFromMd(string articlePath)
        {
            string content = System.IO.File.ReadAllText(articlePath);
            return Markdown.ParseHtmlString(content.Remove(content.LastIndexOf("Tags:")));
        }
        public static string GetTextFromMd(string articlePath) => Markdown.Parse(System.IO.File.ReadAllText(articlePath));
    }
}