namespace MassEffectModBuilder.Package
{
    public class PackageBuilder
    {
        public PackageBuilder(string packageName, string[]? subfolder = null)
        {
            PackageName = packageName;
            Subfolder = subfolder;
        }

        // TODO you should be able to use an existing file from basegame or another mod in your library as a starting point

        public string PackageName { get; }
        public string[]? Subfolder { get; }
        /// <summary>
        /// Setting this to true means it will not be output into the final build, just used as temporary/intermediate file.
        /// Overrides the startup bool.
        /// </summary>
        public bool Temporary { get; set; } = false;

        /// <summary>
        /// Whether this package will be used as a startup file.
        /// </summary>
        public bool Startup { get; set; } = false;

        // TODO give this some context
        public void Build()
        {

        }
    }
}
