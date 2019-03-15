using Newtonsoft.Json;

namespace Roll_Driven_Stories.Classes
{
    public class Post
    {
        [JsonProperty("slug")]
        public string Slug { get; set; }
        
        [JsonProperty("title")]
        public string Title { get; set; }
        
        [JsonProperty("summary")]
        public string Summary { get; set; }
        
        [JsonProperty("md_path")]
        public string MdPath { get; set; }
        
        [JsonProperty("img_path")]
        public string ImgPath { get; set; }
        
        [JsonProperty("published_date")]
        public string PublishedDate { get; set; }

        [JsonProperty("categories")]
        public string Categories { get; set; }
    }
}