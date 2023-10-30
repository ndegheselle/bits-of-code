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

    // Don't use an interface
    public interface INode
    {
        public INode? Previous { get; set; }
        public Lazy<IWork> Create { get; }
        public INode? Next { get; }

        public INode Then(INode node)
        {
            node.Previous = this;
            return node;
        }
    }

    // Should probably give up on having Work and Node in the same object (or find a way to make it Lazy )
    public class Workflow<TContext> : IWork, INode
    {
        public INode? Previous { get; set; }
        public Func<IWork> Create => () => this;
        public INode? Next => throw new NotImplementedException();

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
}
