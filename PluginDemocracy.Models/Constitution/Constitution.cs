namespace PluginDemocracy.Models
{
    /// <summary>
    /// Articles in _articles should be in numbering order starting with 1 (so +1 from the list's index).
    /// </summary>
    public class Constitution
    {
        public string Preamble => _preamble;
        private string _preamble;
        public IReadOnlyList<Article> Articles
        {
            get
            {
                Update();
                return _articles.AsReadOnly();
            }
        }
        private readonly List<Article> _articles;
        public Constitution()
        {
            _articles = new();
            _preamble = string.Empty;
        }
        public void AddArticle(Article article)
        {
            if (article == null || string.IsNullOrEmpty(article.Title) || string.IsNullOrEmpty(article.Body))
            {
                throw new ArgumentException("Invalid article.");
            }

            if (!_articles.Any())
            {
                article.Number = 1;
                _articles.Add(article);
                return;
            }
            //Article number cannot be below 1
            if (article.Number <= 0)
            {
                article.Number = 1;
            }

            int lastArticleNumber = Articles[Articles.Count - 1].Number ?? 0;
            //Case for when number is bigger than current amountCheck numbering. If numbering is bigger than existing or null
            if (article.Number > Articles[Articles.Count - 1].Number || article.Number == null)
            {
                article.Number = Articles[Articles.Count - 1].Number + 1;
                _articles.Add(article);
            }
            //Case for when number is in between current numbers
            else
            {
                int position = _articles.FindIndex(a => a.Number >= article.Number);
                //move all the other article numbers up
                _articles.Insert(position, article);
                RenumberArticles();
            }
        }
        public void RemoveArticle(Article article)
        {
            //Remove Article
            if (_articles.Contains(article))
            {
                _articles.Remove(article);
                RenumberArticles();
            }
            else throw new ArgumentException("Article does not exist in constitution");
            //Renumber Articles
        }
        public void AmendArticle(Article newArticle, Article oldArticle)
        {
            if (newArticle == null || oldArticle == null || !_articles.Contains(oldArticle)) throw new ArgumentException("Invalid input.");
            newArticle.Number ??= oldArticle.Number;
            RemoveArticle(oldArticle);
            AddArticle(newArticle);
        }
        public void RenumberArticles()
        {
            for (int i = 0; i < _articles.Count; i++)
            {
                _articles[i].Number = i + 1;
            }
        }
        public void Update()
        {
            _articles.RemoveAll(articles => articles.ExpirationDate <= DateTime.UtcNow);
            RenumberArticles();
        }
        public void UpdatePreamble(string newPreamble)
        {
            _preamble = newPreamble;
        }
    }
}
