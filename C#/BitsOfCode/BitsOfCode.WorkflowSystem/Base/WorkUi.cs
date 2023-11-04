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
        protected TaskCompletionSource<bool> TaskCompletion { get; set; }

        public WorkUi(IWorkUiContainer container)
        {
            Container = container;
        }

        protected void GoBack()
        {
            TaskCompletion.TrySetResult(false);
        }

        protected void Finish()
        {
            TaskCompletion.TrySetResult(true);
        }

        #region IWork
        public Task<bool> Do(CancellationToken? cancellationToken = null)
        {
            TaskCompletion = new TaskCompletionSource<bool>();

            // Handle CancellationToken through TaskCompletionSource
            if (cancellationToken != null)
                cancellationToken?.Register(() =>
                {
                    TaskCompletion.TrySetResult(false);
                });

            Container.Show(this);
            return TaskCompletion.Task;
        }

        #endregion
    }
}
