namespace PluginDemocracy.Models.Tests
{
    /// <summary>
    /// A dictamen with no Execute code for testing purposes
    /// </summary>
    public class EmptyDictamen : BaseDictamen
    {
        public override string? Title { get => string.Empty ; set { } }
        public override string? Description { get => string.Empty; set { } }

        public EmptyDictamen() : base()
        {

        }
        
        public override void Execute()
        {
           
        }
    }
}
