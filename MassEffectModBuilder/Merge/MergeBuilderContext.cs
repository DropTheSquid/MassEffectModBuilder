namespace MassEffectModBuilder.Merge
{
    public class MergeBuilderContext
    {
        internal MergeBuilderContext(MergeBuilder builder, ModBuilderContext modBuilderContext) {
            ModBuilderContext = modBuilderContext;
            Builder = builder;
        }
        public ModBuilderContext ModBuilderContext { get; }
        public MergeBuilder Builder { get; }
    }
}
