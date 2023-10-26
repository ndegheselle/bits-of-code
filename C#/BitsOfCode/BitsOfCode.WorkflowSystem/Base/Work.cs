using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;

namespace BitsOfCode.WorkflowSystem.Base
{
    public interface IWork
    {
        public IWork? PreviousWork { get; set; }
        public Task Do(CancellationToken? cancellationToken = null);
        public IWork? GetNext();
    }

    public class Workflow<TContext> : IWork
    {
        public IWork? PreviousWork { get; set; }
        public IWork? ActualWork { get; set; }

        public async Task Do(CancellationToken? cancellationToken = null)
        {
            while (ActualWork != null)
            {
                ActualWork.PreviousWork = PreviousWork;
                await ActualWork.Do(cancellationToken);
                PreviousWork = ActualWork;
                ActualWork = ActualWork.GetNext();
            }
        }

        public virtual IWork? GetNext()
        {
            return null;
        }
    }
}
