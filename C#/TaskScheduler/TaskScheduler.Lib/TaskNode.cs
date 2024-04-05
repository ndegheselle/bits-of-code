namespace TaskScheduler.Lib
{
    public interface ITaskNode
    {
        public bool IsReady();

        // Add more complete definition to handle optionnal inputs and outputs
        // May want to use a dictionary instead of a list
        public dynamic?[] Inputs { get; set; }
        public dynamic?[] Outputs { get; set; }

        Task Do(CancellationToken? token = null);
    }

    public abstract class TaskNode : ITaskNode
    {
        public event EventHandler<EventArgs>? Events;

        // By default no input or output
        public dynamic?[] Inputs { get; set; } = [];
        public dynamic?[] Outputs { get; set; } = [];

        abstract public Task Do(CancellationToken? token = null);
        virtual public bool IsReady() => true;
    }
}
