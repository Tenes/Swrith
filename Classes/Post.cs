using Newtonsoft.Json;

namespace Roll_Driven_Stories.Classes
{
    public class Post
    {
        [JsonProperty("slug")]
        public string Slug { get; set; }
        
        [JsonProperty("title")]
        public string Title { get; set; }
        
        [JsonProperty("preview")]
        public string Preview { get; set; }
        
        [JsonProperty("md_path")]
        public string MdPath { get; set; }
        
        [JsonProperty("img_path")]
        public string ImgPath { get; set; }
        
        [JsonProperty("published_date")]
        public string PublishedDate { get; set; }

        [JsonProperty("categories")]
        public string Categories { get; set; }

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