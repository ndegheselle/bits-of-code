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

    public class WorkUi : UserControl, IWork
    {
        protected IWorkUiContainer Container { get; set; }
        protected bool DoGoBack { get; set; } = false;
        protected TaskCompletionSource TaskCompletion { get; set; } = new TaskCompletionSource();

        public WorkUi(IWorkUiContainer container)
        {
            Container = container;
        }


        protected void GoBack()
        {
            DoGoBack = true;
            Finish();
        }

        protected void Finish()
        {
            TaskCompletion.TrySetResult();
        }

        #region IWork
        public IWork? PreviousWork { get; set; }
        public async Task Do(CancellationToken? cancellationToken = null)
        {
            // Handle CancellationToken through TaskCompletionSource
            if (cancellationToken != null)
                cancellationToken?.Register(() =>
                {
                    DoGoBack = true;
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

        public virtual IWork? GetNext()
        {
            if (DoGoBack) return PreviousWork;
            return null;
        }

        #endregion
    }
}
