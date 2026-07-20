namespace MassEffectModBuilder.DLC
{
    public interface IDlcTask
    {
        public void RunDlcTask(DlcBuilderContext context);
    }

    public class CustomDlcTask(Action<DlcBuilderContext> task) : IDlcTask
    {
        public void RunDlcTask(DlcBuilderContext context)
        {
            task.Invoke(context);
        }
    }
}
