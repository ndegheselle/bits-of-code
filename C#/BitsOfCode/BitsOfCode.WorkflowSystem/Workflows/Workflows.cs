using BitsOfCode.WorkflowSystem.Base;
using BitsOfCode.WorkflowSystem.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitsOfCode.WorkflowSystem.Workflows
{
    internal class MainWorkflow : TreeWorkflow<GlobalContext>
    {
        public MainWorkflow(IWorkUiContainer container, GlobalContext context)
        {
            Context = context;
            RootNode = new WorkflowRoutingNode<HomeWork>(
                new Lazy<HomeWork>(() => new HomeWork(container)), 
                new List<IWorkflowNode>()
            {
                new WorkflowNode<TestLinearWorkflow>(new Lazy<TestLinearWorkflow>(() => new TestLinearWorkflow(container, context))),
                new WorkflowNode<TestTreeWorkflow>(new Lazy<TestTreeWorkflow>(() => new TestTreeWorkflow(container, context)))
            });
        }
    }

    internal class TestTreeWorkflow : TreeWorkflow<GlobalContext>
    {
        public TestTreeWorkflow(IWorkUiContainer container, GlobalContext context)
        {
            Context = context;

            var rootNode = new WorkflowNode<WorkA>(new Lazy<WorkA>(() => new WorkA(container)));
            rootNode
                .Then(new WorkflowNode<WorkB>(new Lazy<WorkB>(() => new WorkB(container))))
                .Then(new WorkflowNode<WorkC>(new Lazy<WorkC>(() => new WorkC())));

            RootNode = rootNode;
        }
    }

    internal class TestLinearWorkflow : LinearWorkflow<GlobalContext>
    {
        public TestLinearWorkflow(IWorkUiContainer container, GlobalContext context)
        {
            Context = context;
            Works = new List<Lazy<IWork>>
            {
                new Lazy<IWork>(() => new WorkA(container)),
                new Lazy<IWork>(() => new WorkB(container)),
                new Lazy<IWork>(() => new WorkC())
            };
        }
    }
}
