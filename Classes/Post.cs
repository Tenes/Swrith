using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Swrith.Classes
{
    public class Post
    {
        [JsonProperty("slug")]
        [JsonPropertyName("slug")]
        public string Slug { get; set; }
        
        [JsonProperty("title")]
        [JsonPropertyName("title")]
        public string Title { get; set; }
        
        [JsonProperty("preview")]
        [JsonPropertyName("preview")]
        public string Preview { get; set; }
        
        [JsonProperty("md_path")]
        [JsonPropertyName("md_path")]
        public string MdPath { get; set; }
        
        [JsonProperty("img_path")]
        [JsonPropertyName("img_path")]
        public string ImgPath { get; set; }
        
        [JsonProperty("published_date")]
        [JsonPropertyName("published_date")]
        public string PublishedDate { get; set; }

        [JsonProperty("categories")]
        [JsonPropertyName("categories")]
        public string Categories { get; set; }

        public Post(){}
        public Post(string slug, string title, string preview, string mdPath, string imgPath, string publishedDate, string categories)
        {
            this.Slug = slug;
            this.Title = title;
            this.Preview = preview;
            this.MdPath = mdPath;
            this.ImgPath = imgPath;
            this.PublishedDate = publishedDate;
            this.Categories = categories;
        }
    }
}