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
    public interface IWork
    {
        // Should cancel a task cancel the workflow or just go back ?
        // Probably should separate the navigation from the cancel logic
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
                // TODO : handle cancel and going back
                throw new Exception("Can't set RoutingNode next Node.");
            }
        }

        public WorkflowRoutingNode(Lazy<TWork> work) : base(work)
        { }
    }

    public class TreeWorkflow<TContext> : IWork
    {
        public TContext Context { get; set; }
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

    public class LinearWorkflow<TContext> : IWork
    {
        public TContext Context { get; set; }
        public List<Lazy<IWork>> Works { get; set; }

        public async Task Do(CancellationToken? cancellationToken = null)
        {
            int currentIndex = 0;
            while (currentIndex >= 0 && currentIndex < Works.Count)
            {
                // TODO : handle going back with cancel ?
                await Works[currentIndex].Value.Do(cancellationToken);
                currentIndex++;
            }
        }
    }
}
