namespace MassEffectModBuilder.UtilityTasks
{
    public record class CustomTask(Action<ModBuilderContext> CustomTaskAction) : IModBuilderTask
    {
        public void RunModTask(ModBuilderContext context)
        {
            CustomTaskAction(context);
        }
    }
}
