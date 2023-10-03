namespace PluginDemocracy.Models.Tests
{
    /// <summary>
    /// A dictamen with no Execute code for testing purposes
    /// </summary>
    public class EmptyDictamen : BaseDictamen
    {
        public override MultilingualString Title { get; set; }
        public override MultilingualString Description { get; set; }

        public EmptyDictamen() : base()
        {
            Title = new();
            Description = new();
        }
        public override void Execute()
        {
           
        }
    }
}
