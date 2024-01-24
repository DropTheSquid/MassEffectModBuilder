using LegendaryExplorerCore.Packages;

namespace MassEffectModBuilder.LEXHelpers
{
    public static class LexExtensions
    {
        public static IEntry GetRootEntry(this IEntry entry)
        {
            IEntry root = entry;
            while (root.HasParent)
            {
                root = root.Parent;
            }

            return root;
        }
    }
}
