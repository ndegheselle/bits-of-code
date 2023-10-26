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

namespace AppNamespace
{
    public class TestWork : IWork
    {
        private readonly ILoadingWindow _Window;

        public TestWork(ILoadingWindow window)
        {
            _Window = window;
        }

        public async Task<object> Do(object? parameters = null)
        {
            _Window.ShowDebugInfo(null, (string)parameters);
            _Window.Loading(true, $"{nameof(TestWork)} doing things");
            await Task.Delay(3000);
            return $"{nameof(TestWork)}.params[{parameters}]";
        }
    }

    public class ContextWork : IWork
    {
        private readonly ILoadingWindow _Window;
        public string Context { get; set; }

        public ContextWork(ILoadingWindow window, string context)
        {
            _Window = window;
            Context = context;
        }

        public async Task<object> Do(object? parameters = null)
        {
            _Window.ShowDebugInfo(Context, (string)parameters);
            _Window.Loading(true, $"{nameof(ContextWork)} doing things");
            await Task.Delay(3000);
            return $"{nameof(ContextWork)}.params[{parameters}] & ContextWork.Context[{Context}]";
        }
    }

    public partial class UserControlWork : WorkUi<string, string, string>
    {
        private readonly WindowWork _Window;
        public UserControlWork(WindowWork window)
        {
            Context = "whatever";
            _Window = window;
            InitializeComponent();
        }

        public override Task<object> Do(object? parameters = null)
        {
            _Window.ShowDebugInfo(Context, (string)parameters);
            _Window.Display(this);
            return base.Do(parameters);
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            this.TaskCompletion.SetResult(this.Parameters);
        }
    }
}
