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

    // Use a list instead of simple next,
    // Use a method to get the next one 
    // Create a RoutingNode that can get a method to choose which next you take
    public class Node
    {
        public Node? Previous { get; set; }
        public Lazy<IWork> Create { get; }
        public Lazy<Node?> Next { get; set; }

        public Node(Lazy<IWork> create)
        {
            Create = create;
        }

        public Lazy<Node?> Then(Lazy<Node?> node)
        {
            node.Previous = this;
            Next = node;
            return node;
        }
    }

    // Should probably give up on having Work and Node in the same object (or find a way to make it Lazy )
    public class Workflow<TContext> : IWork
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

            Node nodes = new Node()
                .Then(new Lazy<Node>()).Then();
        }
    }
}
