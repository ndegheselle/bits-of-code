using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Controls;

namespace MeNoisySoundboard.App.Base
{
    public interface IWork : IWork<object, object, object>
    {}
    public interface IWork<TContext> : IWork<TContext, object, object>
    { }
    public interface IWork<TContext, TResult, TParams>
    {
        public TContext? Context { get; set; }
        public Task<TResult> Do(TParams? parameters = default);
    }

    public class WorkUi<TContext, TResult, TParams> : UserControl, IWork<TContext, TResult, TParams>
    {
        public TaskCompletionSource<TResult> TaskCompletion { get; set; } = new TaskCompletionSource<TResult>();

        public TContext? Context { get; set; } = default;
        protected TParams? Parameters{ get; set; } = default;

        // May be good to add a OnStart event (for exempel to refresh the UI)
        public virtual Task<TResult> Do(TParams? parameters = default)
        {
            Parameters = parameters;
            return TaskCompletion.Task;
        }

        public void Cancel()
        {
            TaskCompletion.SetCanceled();
        }
    }

    public class SimpleWorkUi<TContext> : WorkUi<TContext, bool, object>
    {}

    public class Workflow<TContext, TResult, TParams> : IWork<TContext, TResult, TParams>
    {
        public delegate IWork CreateWork(TContext workflowContext);
        public TContext? Context { get; set; } = default;

        protected Dictionary<Type, CreateWork> SubWorks { get; set; } = new Dictionary<Type, CreateWork>();

        public virtual async Task<TResult> Do(TParams? workflowParameters = default)
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

    public class SimpleWorkflow<TContext> : Workflow<TContext, bool, object>
    {
        public override async Task<bool> Do(object? workflowParameters = null)
        {
            await base.Do(workflowParameters);
            return true;
        }
    }
}
