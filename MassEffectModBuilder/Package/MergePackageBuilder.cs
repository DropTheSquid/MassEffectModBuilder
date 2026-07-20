using MassEffectModBuilder.Merge;

namespace MassEffectModBuilder.Package
{
    public class MergePackageBuilder : PackageBuilder<MergeBuilderContext>
    {
        public MergePackageBuilder(string packageName, string? source = null) : base(packageName, [], source)
        {
        }

        protected override string GetSaveLocationRoot(MergeBuilderContext context)
        {
            return context.MergeModsFolder;
        }
    }
}
