using BitsOfCode.WorkflowSystem.Base;
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
    /// Logique d'interaction pour A.xaml
    /// </summary>
    public partial class WorkA : WorkUi
    {
        private readonly IWorkUiContainer _container;

        public WorkA(IWorkUiContainer container) : base(container)
        {
            _container = container;
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            GoBack();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            Finish();
        }
    }
}
