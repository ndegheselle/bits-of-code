using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Controls;

namespace BitsOfCode.WorkflowSystem.Base
{
    public interface IWork
    {
        public Task Do(object? parameters = default);
    }

    public interface IWork<T> : IWork
    {
        public override Task<T> Do(object? parameters = default);
    }

    public class WorkUi<TContext, TParams, TResult> : UserControl, IWork
    {
        public TaskCompletionSource<TResult> TaskCompletion { get; set; } = new TaskCompletionSource<TResult>();

        public TContext? Context { get; set; } = default;
        public TParams? Parameters{ get; set; } = default;

        // May be good to add a OnStart event (for exempel to refresh the UI)
        public virtual async Task<object> Do(object? parameters = default)
        {
            Parameters = (TParams)parameters;
            return await TaskCompletion.Task;
        }

        public void Cancel()
        {
            TaskCompletion.SetCanceled();
        }
    }

    public class Workflow<TContext, TParams, TResult> : IWork
    {
        // May not need to pass context there since the delegate will have access to the workflow data anyway
        public delegate IWork CreateWork(TContext workflowContext);
        public TContext? Context { get; set; } = default;

        protected Dictionary<Type, CreateWork> SubWorks { get; set; } = new Dictionary<Type, CreateWork>();

        public virtual async Task<object> Do(object? workflowParameters = default)
        {
            int currentIndex = 0;
            object? chainParameters = null;
            while(currentIndex >= 0 && currentIndex < SubWorks.Count)
            {
                try
                {
                    var work = SubWorks.ElementAt(currentIndex).Value(Context);
                    // In a workflow we chain the parameters of each work
                    chainParameters = await work.Do(chainParameters);
                }
                catch (TaskCanceledException)
                {
                    currentIndex--;
                    continue;
                }
                currentIndex++;
            }

            if (currentIndex < 0) throw new TaskCanceledException();

            return default(TResult);
        }
    }
}
