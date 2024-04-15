namespace MassEffectModBuilder
{
    public interface IModBuilderTask
    {
        public abstract void RunModTask(ModBuilderContext context);
    }
 
    public abstract class IModBuilderTaskWithCustomContext<T> : IModBuilderTask where T : class
    {
        public abstract void RunModTask(ModBuilderCustomContext<T> context);

        public void RunModTask(ModBuilderContext context)
        {
            // Implementer can override this to handle non generic context
            if (context is not ModBuilderCustomContext<T>)
            {
                throw new InvalidOperationException();
            }
            RunModTask((ModBuilderCustomContext<T>)context);
        }
    }
}
