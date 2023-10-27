using AdonisUI.Controls;
using BitsOfCode.WorkflowSystem.Base;
using BitsOfCode.WorkflowSystem.Views;
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

namespace BitsOfCode.WorkflowSystem
{
    public class GlobalContext
    {

    }

    public class TestWorkflow : Workflow<GlobalContext>
    {
        public TestWorkflow(IWorkUiContainer container) : base(new WorkA(container))
        {}
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : AdonisWindow, IWorkUiContainer
    {
        public MainWindow()
        {
            InitializeComponent();
            TestWorkflow testWorkflow = new TestWorkflow(this);
            testWorkflow.Do();
        }

        public void Show(FrameworkElement element)
        {
            MainContainer.Content = element;
        }
    }
}
