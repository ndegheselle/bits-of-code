using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace BitsOfCode.WorkflowSystem.Base
{
    public interface IWorkUiContainer
    {
        public void Show(FrameworkElement element);
    }

    public class WorkUi : UserControl, IChainedWork
    {
        protected IWorkUiContainer Container { get; set; }
        protected TaskCompletionSource TaskCompletion { get; set; }

        public WorkUi(IWorkUiContainer container)
        {
            Container = container;
        }

        protected void GoBack()
        {
            NextWork = PreviousWork;
            Finish();
        }

        protected void Finish()
        {
            TaskCompletion.TrySetResult();
        }

        #region IWork
        public IChainedWork? PreviousWork { get; set; }
        public IChainedWork? NextWork { get; protected set; }
        public async Task Do(CancellationToken? cancellationToken = null)
        {
            TaskCompletion = new TaskCompletionSource();

            // Handle CancellationToken through TaskCompletionSource
            if (cancellationToken != null)
                cancellationToken?.Register(() =>
                {
                    NextWork = PreviousWork;
                    TaskCompletion.TrySetCanceled();
                });

            Container.Show(this);
            try
            {
                // Block until Finish() is called
                await TaskCompletion.Task;
            }
            catch (TaskCanceledException)
            {}
        }

        #endregion
    }
}
