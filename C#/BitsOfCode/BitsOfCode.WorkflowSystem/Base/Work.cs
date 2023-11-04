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
        public Task<bool> Do(CancellationToken? cancellationToken = null);
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

        public IWorkflowNode Then(IWorkflowNode node);
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
        public List<IWorkflowNode> Branches { get; set; }

        private IWorkflowNode? _next;
        public override IWorkflowNode? Next
        {
            get
            {
                if (_next != null) return _next;
                var nextType = ((IRoutingWork)Work.Value).NextWorkType;
                return Branches.FirstOrDefault(x => x.GetWorkType() == nextType);
            }
            set {
                _next = value;
            }
        }

        public WorkflowRoutingNode(Lazy<TWork> work, List<IWorkflowNode> branches) : base(work)
        {
            Branches = branches;
            // XXX : maybe move this in the setter of Branches ?
            foreach(var branch in Branches)
                branch.Previous = this;
        }
    }

    public class TreeWorkflow<TContext> : IWork
    {
        public TContext Context { get; set; }
        public IWorkflowNode RootNode { get; set; }
        private IWorkflowNode? _actualNode { get; set; }

        public async Task<bool> Do(CancellationToken? cancellationToken = null)
        {
            bool result = true;
            _actualNode = RootNode;
            while (_actualNode != null)
            {
                result = await _actualNode.Work.Value.Do(cancellationToken);
                if (result)
                    _actualNode = _actualNode.Next;
                else
                    _actualNode = _actualNode.Previous;
            }
            return result;
        }
    }

    public class LinearWorkflow<TContext> : IWork
    {
        public TContext Context { get; set; }
        public List<Lazy<IWork>> Works { get; set; }

        public async Task<bool> Do(CancellationToken? cancellationToken = null)
        {
            int currentIndex = 0;
            bool result = true;
            while (currentIndex >= 0 && currentIndex < Works.Count)
            {
                result = await Works[currentIndex].Value.Do(cancellationToken);
                if (result)
                    currentIndex++;
                else
                    currentIndex--;
            }
            return result;
        }
    }
}
