namespace MassEffectModBuilder.UtilityTasks
{
    public record class DynamicTask(Func<ModBuilderContext, IModBuilderTask> CustomTaskAction) : IModBuilderTask
    {
        public void RunModTask(ModBuilderContext context)
        {
            CustomTaskAction(context).RunModTask(context);
        }
    }
}
