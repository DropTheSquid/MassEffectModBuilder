using LegendaryExplorerCore.Packages;

namespace MassEffectModBuilder.Package
{
    public interface IPackageBuilderTask<T>
    {
        public void RunPackageTask(IMEPackage package, T context);
    }

    public class CustomPackageBuilderTask<T>(Action<IMEPackage, T> action): IPackageBuilderTask<T>
    {
        public void RunPackageTask(IMEPackage package, T context)
        {
            action.Invoke(package, context);
        }
    }
}
