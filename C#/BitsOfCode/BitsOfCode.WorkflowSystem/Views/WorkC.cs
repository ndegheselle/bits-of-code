using BitsOfCode.WorkflowSystem.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace BitsOfCode.WorkflowSystem.Views
{
    internal class WorkC : IWork
    {
        public Task<bool> Do(CancellationToken? cancellationToken = null)
        {
            MainWindow mainWindow = App.Current.MainWindow as MainWindow;
            mainWindow.SetStatut("WorkC : background work.");
            return Task.FromResult(true);
        }
    }
}
