using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Models
{
    public class Constitution
    {
        public string Preamble { get; set;}
        IReadOnlyList<Article> Articles => _articles.AsReadOnly();
        private List<Article> _articles;
        public Constitution()
        {
            _articles = new();
            Preamble = string.Empty;
        }
        public void AddArticle(Article article)
        {
            if(Articles.Count == 0)
            {
                article.Number = 1;
                _articles.Add(article);
                return;
            }
            //Article number cannot be below 1
            if (article.Number < 1) article.Number = 1;
            int lastArticleNumber = Articles[Articles.Count - 1].Number ?? 0;
            //Case for when number is bigger than current amountCheck numbering. If numbering is bigger than existing or null
            if (article.Number > Articles[Articles.Count - 1].Number || article.Number == null) 
            {
                article.Number = Articles[Articles.Count-1].Number + 1;
                _articles.Add(article);
            }
            //Case for when number is in between current numbers
            else
            {
                int position = _articles.FindIndex(a => a.Number >= article.Number);
                //move all the other article numbers up
                for (int i = _articles.Count - 1; i >= position - 1; i--)
                {
                    _articles[i].Number++;
                }
                _articles.Insert(position, article);
            }
        }
    }
    public class Article
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public int? Number { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public Article(string title, string body, int number, DateTime expirationDate)
        {
            Title = title;
            Body = body;   
            Number = number;
            ExpirationDate = expirationDate;
        }
    }
}
