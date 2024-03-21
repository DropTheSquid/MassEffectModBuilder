namespace MassEffectModBuilder.Models
{
    public abstract class ModConfigFile
    {
        protected List<ModConfigClass> ClassConfigs = [];

        // I think this will actually live in the subclasses because the interface is slightly different
        public abstract ModConfigClass GetOrCreateClass(string classFullPath);

        public ModConfigClass? GetClass(string classFullPath)
        {
            return ClassConfigs.FirstOrDefault(x => x.ClassFullPath == classFullPath);
        }
    }
}
