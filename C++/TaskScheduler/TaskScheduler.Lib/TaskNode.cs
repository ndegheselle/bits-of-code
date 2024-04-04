namespace TaskScheduler.Lib
{
    public interface ITaskNode
    {
        Task Do();
    }

    public class TaskNode : ITaskNode
    {
        public event EventHandler<EventArgs>? Events;
        virtual public Task Do()
        {
            return Task.CompletedTask;
        }
    }
}
