namespace MassEffectModBuilder.Merge
{
    public interface IMergeTask
    {
        public void RunMergeTask(MergeBuilderContext context);
    }

    public class CustomMergeTask(Action<MergeBuilderContext> task) : IMergeTask
    {
        public void RunMergeTask(MergeBuilderContext context)
        {
            task.Invoke(context);
        }
    }
}
