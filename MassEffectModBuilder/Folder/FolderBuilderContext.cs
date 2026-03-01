using LegendaryExplorerCore.Packages;

namespace MassEffectModBuilder.Folder
{
    public class FolderBuilderContext
    {
        public FolderBuilderContext(FolderBuilder builder, ModBuilderContext modContext)
        {
            Builder = builder;
            ModContext = modContext;
        }

        protected FolderBuilder Builder;

        protected ModBuilderContext ModContext;

        public MEGame Game => ModContext.Game;
    }
}
