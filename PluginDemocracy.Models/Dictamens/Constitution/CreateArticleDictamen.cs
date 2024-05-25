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
        public override string? Title { get => ResourceKeys.CreateArticleDictamenTitle ; set { } } 
        public override string? Description { get => ResourceKeys.CreateArticleDictamenDescription; set { } } 
        #region InputsNeededByThisDictamenInParticular
        //I need to know the details of the law to be known. What all is part of a law? 
        //I need to know if it is going to have a specific number? (if so, push all others)
        //How am I going to handle the inputs? Get them manually in here or import them from Article.cs? What is the best way to do it? 
        #endregion
        public override void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
