using MassEffectModBuilder.DLC;

namespace MassEffectModBuilder.Package
{
    public class DlcPackageBuilder : PackageBuilder<DlcBuilderContext>
    {
        public DlcPackageBuilder(string packageName, string[]? subfolder = null, string? source = null) : base(packageName, subfolder, source)
        {
        }

        /// <summary>
        /// Whether this package will be used as a startup file.
        /// </summary>
        public bool Startup { get; set; } = false;

        protected override string GetSaveLocationRoot(DlcBuilderContext dlcContext)
        {
            return dlcContext.CookedFolderPath;
        }

        public override void Build(ModBuilderContext modContext, DlcBuilderContext context)
        {
            if (Startup && Source != "existing")
            {
                context.WithStartupPackage(PackageName);
            }

            base.Build(modContext, context);
        }
    }
}
