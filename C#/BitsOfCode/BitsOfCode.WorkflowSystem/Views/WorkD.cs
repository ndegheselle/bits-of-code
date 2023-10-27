using BitsOfCode.WorkflowSystem.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BitsOfCode.WorkflowSystem.Views
{
    internal class WorkD : IChainedWork
    {
        public IChainedWork? PreviousWork { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IChainedWork? NextWork => throw new NotImplementedException();

        public Task Do(CancellationToken? cancellationToken = null)
        {
            throw new NotImplementedException();
        }
    }
}
