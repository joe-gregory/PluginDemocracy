namespace PluginDemocracy.Models
{
    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public int? Number { get; set; }
        public DateTime CreationDate { get; }
        public DateTime? ExpirationDate { get; set; }
        public readonly BaseDictamen OriginDictamen;
        public Article(BaseDictamen dictamen, string title, string body, int? number, DateTime? expirationDate)
        {
            OriginDictamen = dictamen;
            Title = title;
            Body = body;
            Number = number;
            CreationDate = DateTime.UtcNow;
            ExpirationDate = expirationDate;
        }
    }
}
