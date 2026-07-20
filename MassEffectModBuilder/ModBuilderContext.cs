using LegendaryExplorerCore.Packages;

namespace MassEffectModBuilder
{

    //public class ModBuilderContext<T>(ModBuilder builder, string modOutputBasePath) : ModBuilderContext(builder, modOutputBasePath) where T : new()
    //{
    //    public T CustomContext { get; set; } = new();
    //}

    public class ModBuilderContext(ModBuilder builder, string modOutputBasePath, string modLibraryBase)
    {
        protected readonly ModBuilder Builder = builder;

        public MEGame Game => Builder.Game;

        public ModDesc ModDesc => Builder.ModDesc;

        public string ModLibraryBase => modLibraryBase;

        public string ModOutputBasePath => modOutputBasePath;

        public string MergeModsFolder => Path.Combine(modOutputBasePath, "MergeMods");
    }
}
