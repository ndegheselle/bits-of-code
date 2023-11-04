using BitsOfCode.WorkflowSystem.Base;
using BitsOfCode.WorkflowSystem.Workflows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BitsOfCode.WorkflowSystem.Views
{
    /// <summary>
    /// Logique d'interaction pour HomeWork.xaml
    /// </summary>
    public partial class HomeWork : WorkUi, IRoutingWork
    {
        public Type NextWorkType { get; set; }

        public HomeWork(IWorkUiContainer container) : base(container)
        {
            InitializeComponent();
        }

        private void TreeWorkflow_Click(object sender, RoutedEventArgs e)
        {
            NextWorkType = typeof(TestTreeWorkflow);
            Finish();
        }

        private void LinearWorkflow_Click(object sender, RoutedEventArgs e)
        {
            NextWorkType = typeof(TestLinearWorkflow);
            Finish();
        }
    }
}
