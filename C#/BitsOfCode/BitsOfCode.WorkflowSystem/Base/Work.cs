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
using System.Xml.Linq;
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
    public interface IRoutingWork : IWork
    {
        public Type NextWorkType { get; set; }
    }


    public interface IWorkflowNode
    {
        public Type GetWorkType();
        public IWorkflowNode? Previous { get; set; }
        public Lazy<IWork> Work { get; }
        public IWorkflowNode? Next { get; set; }
    }

    public class WorkflowNode<TWork> : IWorkflowNode where TWork : IWork
    {
        public IWorkflowNode? Previous { get; set; }
        public Lazy<IWork> Work { get; set; }
        public virtual IWorkflowNode? Next { get; set; }

        public WorkflowNode(Lazy<TWork> work)
        {
            Work = new Lazy<IWork>(() => work.Value);
        }

        public IWorkflowNode Then(IWorkflowNode node)
        {
            node.Previous = this;
            Next = node;
            return node;
        }

        public Type GetWorkType()
        {
            return typeof(TWork);
        }
    }

    public class WorkflowRoutingNode<TWork> : WorkflowNode<TWork> where TWork : IRoutingWork
    {
        public List<IWorkflowNode> Branches { get; set; } = new List<IWorkflowNode>();

        public override IWorkflowNode? Next
        {
            get
            {
                var nextType = ((IRoutingWork)Work.Value).NextWorkType;
                return Branches.FirstOrDefault(x => x.GetWorkType() == nextType);
            }
            set {
                throw new Exception("Can't set RoutingNode next Node.");
            }
        }

        public WorkflowRoutingNode(Lazy<TWork> work) : base(work)
        { }
    }


    // Should probably give up on having Work and Node in the same object (or find a way to make it Lazy )
    public class TreeWorkflow<TContext> : IWork
    {
        public IWorkflowNode? Node { get; set; }

        public async Task Do(CancellationToken? cancellationToken = null)
        {
            while (Node != null)
            {
                await Node.Work.Value.Do(cancellationToken);
                Node = Node.Next;
            }
        }
    }
}
