using PluginDemocracy.Models.Attributes;
using PluginDemocracy.Translations;
namespace PluginDemocracy.Models
{
    public class Article
    {
        public int Id { get; set; }
        [UIInput]
        public string? Title { get; set; }
        [UIInput(type : UIInputType.TextArea)]
        public string? Body { get; set; }
        [UIInput(type : UIInputType.Integer /*, descriptionKey: ResourceKeys.*/)]
        public int? Number { get; set; }
        public DateTime? CreationDate { get; }
        public DateTime? ExpirationDate { get; set; }
        public readonly BaseDictamen? OriginDictamen;
        protected Article() {}
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
