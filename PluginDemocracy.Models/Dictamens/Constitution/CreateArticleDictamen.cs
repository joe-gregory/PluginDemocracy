using PluginDemocracy.Translations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Models.Dictamens.Constitution
{
    public class CreateArticleDictamen : BaseDictamen
    {
        public override string? TitleKey { get => ResourceKeys.CreateArticleDictamenTitle ; set { } } 
        public override string? DescriptionKey { get => ResourceKeys.CreateArticleDictamenDescription; set { } }
        #region InputsNeededByThisDictamenInParticular
        public Article Article { get; set; } = new();
        #endregion
        public override void Execute()
        {
            if (Community == null) throw new Exception("This Dictamen's Community is not set");
            Community.Constitution.AddArticle(Article);
        }
    }
}
