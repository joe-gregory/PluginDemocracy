using PluginDemocracy.Models.Attributes;
using PluginDemocracy.Translations;
namespace PluginDemocracy.Models
{
    [UIInputContainer(titleKey: ResourceKeys.ArticleUIContainerTitle, descriptionKey: ResourceKeys.ArticleUIContainerDescription)]
    public class Article
    {
        public int Id { get; set; }
        [UIInput(type:UIInputType.TextBox, labelKey: ResourceKeys.ArticleTitleUIInputLabel,descriptionKey: ResourceKeys.ArticleTitleUIInputDescription)]
        public string? Title { get; set; }
        [UIInput(type : UIInputType.TextArea, labelKey: ResourceKeys.ArticleBodyUIInputLabel, descriptionKey: ResourceKeys.ArticleBodyUIInputDescription, optional: false)]
        public string? Body { get; set; }
        [UIInput(type : UIInputType.Integer, labelKey: ResourceKeys.ArticleNumberLabel, descriptionKey: ResourceKeys.ArticleNumberDescription)]
        public int? Number { get; set; }
        public DateTime? CreationDate { get; }
        [UIInput(type : UIInputType.Date, labelKey: ResourceKeys.ArticleExpirationDateUIInputLabel, descriptionKey: ResourceKeys.ArticleExpirationDateUIInputDescription)]
        public DateTime? ExpirationDate { get; set; }
        public readonly BaseDictamen? OriginDictamen;
        public Article() {}
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
