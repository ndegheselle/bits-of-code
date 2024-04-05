using System.Net;

namespace TaskScheduler.Lib
{
    public struct TaskEndpoint
    {
        public ITaskNode Task { get; set; }

        public int EndpointIndex { get; set; }

        public TaskEndpoint(ITaskNode task, int index = 0)
        {
            Task = task;
            EndpointIndex = index;
        }
    }

    public class TaskLink
    {
        public TaskEndpoint From { get; set; }
        public TaskEndpoint To { get; set; }

        public TaskLink(TaskEndpoint from, TaskEndpoint to)
        {
            From = from;
            To = to;
        }

        public TaskLink(ITaskNode from, ITaskNode to, int fromIndex = 0, int toIndex = 0)
        {
            From = new TaskEndpoint(from, fromIndex);
            To = new TaskEndpoint(to, toIndex);
        }
    }

    public class TaskGraph : ITaskNode
    {
        // By default no input or output
        public dynamic?[] Inputs { get; set; } = [];
        public dynamic?[] Outputs { get; set; } = [];

        public ITaskNode StartingTask { get; set; }

        protected List<ITaskNode> _tasks;
        protected List<TaskLink> _links;

        private CancellationTokenSource _token;
        private ITaskNode _currentTask;

        public bool IsReady() => true;

        public virtual async Task Do(CancellationToken? token = null)
        {
            if (token == null)
                _token = new CancellationTokenSource();
            else
                _token = CancellationTokenSource.CreateLinkedTokenSource(token.Value);

            if (token?.IsCancellationRequested == true)
                return;

            StartingTask.Inputs = Inputs;
            await StartTasks(_token.Token, StartingTask);
            Outputs = _currentTask.Outputs;
        }

        private async Task StartTasks(CancellationToken token, ITaskNode currentTask)
        {
            if (token.IsCancellationRequested)
                return;

            await currentTask.Do(_token.Token);

            for (int i = 0; i < currentTask.Outputs.Count(); i++)
            {
                var output = currentTask.Outputs[i];
                // If there is a link from the current task to another task
                TaskLink? nextLink = _links.FirstOrDefault(
                    l => l.From.Task == currentTask && l.From.EndpointIndex == i);

                if (nextLink == null)
                    continue;

                // Start the next task
                _currentTask = nextLink.To.Task;
                _currentTask.Inputs[nextLink.To.EndpointIndex] = output;
                if (_currentTask.IsReady())
                    await StartTasks(token, _currentTask);
            }
        }
    }

    public class LinearGraph : TaskGraph
    {
        public LinearGraph(List<ITaskNode> tasks)
        {
            _tasks = tasks;
            _links = new List<TaskLink>();
            StartingTask = _tasks.First();

            for (int i = 0; i < _tasks.Count - 1; i++)
            {
                _links.Add(new TaskLink(
                    new TaskEndpoint(_tasks[i]),
                    new TaskEndpoint(_tasks[i + 1])
                    ));
            }
        }
    }
}
