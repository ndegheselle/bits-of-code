using System.Diagnostics;
using TaskScheduler.Lib;

namespace TaskScheduler.Test
{
    internal class TestTask : TaskNode
    {
        private readonly int _factor;

        public TestTask(int factor)
        {
            _factor = factor;
            //Default Input on 0
            Inputs = [null];
            // Default output on 0
            Outputs = [null];
        }

        public override Task Do(CancellationToken? token = null)
        {
            Outputs[0] = _factor * Inputs[0];
            Debug.WriteLine($"Task {_factor} : {Inputs[0]} => {Outputs[0]}");
            return Task.CompletedTask;
        }

        public override bool IsReady()
        {
            return Inputs.FirstOrDefault() != null;
        }
    }

    internal class Test2OutputTask : TaskNode
    {
        private readonly int _factor;

        public Test2OutputTask(int factor)
        {
            _factor = factor;
            //Default Input on 0
            Inputs = [null];
            // Two outputs
            Outputs = [null, null];
        }

        public override Task Do(CancellationToken? token = null)
        {
            Outputs[0] = _factor * Inputs[0];
            Outputs[1] = _factor + Inputs[0];
            Debug.WriteLine($"Test2OutputTask {_factor} : {Inputs[0]} => {Outputs[0]} | {Outputs[0]}");
            return Task.CompletedTask;
        }

        public override bool IsReady()
        {
            return Inputs.FirstOrDefault() != null;
        }
    }

    internal class Test2InputTask : TaskNode
    {
        private readonly int _factor;

        public Test2InputTask(int factor)
        {
            _factor = factor;
            //Default Input on 0
            Inputs = [null, null];
            // Two outputs
            Outputs = [null];
        }

        public override Task Do(CancellationToken? token = null)
        {
            Outputs[0] = _factor * (Inputs[0] + Inputs[1]);
            Debug.WriteLine($"Test2InputTask {_factor} : {Inputs[0]} | {Inputs[1]} => {Outputs[0]}");
            return Task.CompletedTask;
        }

        public override bool IsReady()
        {
            return Inputs.FirstOrDefault() != null;
        }
    }

    internal class TestLinearGraph : LinearGraph
    {
        public TestLinearGraph(int startingValue) : base(new List<ITaskNode>()
        {
            new TestTask(2),
            new TestTask(1),
            new TestTask(2)
        })
        {
            Inputs = [startingValue];
        }
    }

    internal class TestGraph : LinearGraph
    {
        public TestGraph(int startingValue) : base(new List<ITaskNode>()
        {
            new Test2OutputTask(1),
            new TestTask(2),
            new TestTask(3),
            new Test2InputTask(4)
        })
        {
            Inputs = [startingValue];
            _links = new List<TaskLink>()
            {
                new TaskLink(_tasks[0], _tasks[1], 0, 0),
                new TaskLink(_tasks[0], _tasks[2], 1, 0),
                new TaskLink(_tasks[1], _tasks[3], 0, 0),
                new TaskLink(_tasks[2], _tasks[3], 0, 1 )
            };
        }
    }
}
