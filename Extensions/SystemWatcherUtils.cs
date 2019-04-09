using System;
using System.IO;
using System.Text.RegularExpressions;
using ImageMagick;
using Microsoft.AspNetCore.Html;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Dice_Driven_Stories.Classes;
using System.ServiceModel.Syndication;
using System.Collections.Generic;
using System.Xml;

namespace Dice_Driven_Stories.Extensions
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
            Startup.ArticleWatcher.NotifyFilter = NotifyFilters.FileName;
            Startup.ArticleWatcher.Filter = "*.md";
            Startup.ArticleWatcher.Created += OnCreateArticle;
            Startup.ArticleWatcher.EnableRaisingEvents = true;
        }

        private static void OnCreateArticle(object source, FileSystemEventArgs e)
        {
            Console.WriteLine($"File: {e.FullPath} {e.ChangeType}");
            IncludeArticleInJson(e.FullPath);
            Console.WriteLine($"{e.Name} incorporated into posts.json successfully and added to RAM.");
        }
        
        private static void CompressImage(string filePath)
        {
            FileInfo snakewareLogo = new FileInfo(filePath);
            using (var magickImage = new MagickImage(snakewareLogo))
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

        private static void IncludeArticleInJson(string articlePath)
        {
            JObject json;
            using (StreamReader streamReader = System.IO.File.OpenText($@"{Startup.ContentRoot}/posts.json"))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                FileInfo file = new FileInfo(articlePath);
                string articleContent = Regex.Replace(GetTextFromMd(articlePath), "<[^>]*>", String.Empty);
                string nameNoExtension = file.Name.Substring(0, file.Name.LastIndexOf('.'));
                json = JObject.Load(jsonReader);
                Post postToAdd = new Post(
                    nameNoExtension,
                    char.ToUpper(nameNoExtension[0]) + nameNoExtension.Substring(1),
                    articleContent.Substring(0, 230),
                    Path.Combine("articles", file.Name),
                    Path.Combine(@"\", "images", "compressed", nameNoExtension + "50x50.jpg"),
                    DateTime.Now.ToShortDateString(),
                    articleContent.Substring(articleContent.LastIndexOf("Tags:") + 5,
                        articleContent.LastIndexOf('.') - articleContent.LastIndexOf("Tags:") - 5)
                );
                ((JArray)(json["posts"])).Insert(0, JToken.FromObject(postToAdd));
                Startup.TotalPosts.Insert(0, postToAdd);
                AppendToRss(articlePath, nameNoExtension);
            }
            System.IO.File.WriteAllText($@"{Startup.ContentRoot}/posts.json", JsonConvert.SerializeObject(json, Newtonsoft.Json.Formatting.Indented));
        }

        public static void AppendToRss(string filePath, string fileName)
        {
            XmlReader rssReader = XmlReader.Create($"{Startup.ContentRoot}/rss.xml");
            SyndicationFeed feed = SyndicationFeed.Load(rssReader);
            rssReader.Close();

            SyndicationItem item = new SyndicationItem(char.ToUpper(fileName[0]) + fileName.Substring(1), "", new Uri($"https://dicedrivenstories.com/read/{fileName}"), fileName, DateTime.Now);

            XmlDocument doc = new XmlDocument();
            XmlElement content = doc.CreateElement("content", "encoded", "http://purl.org/rss/1.0/modules/content/");
            content.InnerText = GetHtmlFromMd(filePath).Value;
            item.ElementExtensions.Add(content);
            List<SyndicationItem> items = new List<SyndicationItem>(feed.Items);
            items.Add(item);
            feed.LastUpdatedTime = DateTime.Now;
            feed.Items = items;

            XmlWriter rssWriter = XmlWriter.Create($"{Startup.ContentRoot}/rss.xml");
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