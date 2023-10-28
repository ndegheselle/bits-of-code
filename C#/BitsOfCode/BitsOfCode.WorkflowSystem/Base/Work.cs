using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;

namespace BitsOfCode.WorkflowSystem.Base
{
    // Linear workflow
    // Chain workflow

    // Access to previous work (for exemple if it have context)

    public interface IWork
    {
        public Task Do(CancellationToken? cancellationToken = null);
    }

    public interface IChainedWork : IWork
    {
        public IChainedWork? PreviousWork { get; set; }
        public IChainedWork? NextWork { get; }
    }

    public class Workflow<TContext> : IChainedWork
    {
        public IChainedWork? PreviousWork { get; set; }
        public IChainedWork? ActualWork { get; protected set; }
        public IChainedWork? NextWork { get; protected set; }

        public Workflow(IChainedWork work)
        {
            ActualWork = work;
        }

        public async Task Do(CancellationToken? cancellationToken = null)
        {
            while (ActualWork != null)
            {
                ActualWork.PreviousWork = PreviousWork;
                await ActualWork.Do(cancellationToken);
                PreviousWork = ActualWork;
                ActualWork = ActualWork.NextWork;
            }
        }
    }

    public interface IWorkflowNode
    {
        public Type GetWorkType();
        public Func<IWork> Create { get; set; }
    }

    public class WorkflowNode
    {
        public IWork? NextWork { get; set; }

    }

}
