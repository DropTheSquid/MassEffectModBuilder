namespace MassEffectModBuilder
{
    public interface IModBuilderTask
    {
        public void RunModTask(ModBuilderContext context);
    }
 
    //public interface IModBuilderTask<T> : IModBuilderTask where T : new()
    //{
    //    public void RunModTask(ModBuilderContext<T> context);
    //}
}
