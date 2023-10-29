using BitsOfCode.WorkflowSystem.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;
using static System.Net.Mime.MediaTypeNames;

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

            new WorkflowNode()
            {
                Create = () => new WorkA(null),
                Next = () =>
                {
                    if (true)
                        return new WorkflowNode()
                        {
                            Create = () => new WorkB(null),
                            Next = () => new WorkflowNode()
                            {
                            }
                        };
                    else
                        return new WorkflowNode()
                        {
                            Create = () => new WorkC(null),
                            Next = () => new WorkflowNode()
                            {
                            }
                        };
                }
            };
        }
    }

    public interface IWorkflowNode
    {
        public Type GetWorkType();
        public Func<IWork> Create { get; set; }
    }

    public class WorkflowNode
    {
        public Func<IWork> Create { get; set; }
        public Func<WorkflowNode> Next { get; set; }

        public WorkflowNode Then(Func<WorkflowNode> next)
        {
            Next = next;
            return this;
        }
    }
}
