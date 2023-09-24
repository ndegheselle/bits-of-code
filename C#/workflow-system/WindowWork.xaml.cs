using AdonisUI.Controls;
using AppNamespace.Base;
using System;
using System.Collections.Generic;
using System.Windows;

namespace AppNamespace
{
    public class WorkflowTest : Workflow<string, object, bool>
    {
        public WorkflowTest(WindowWork window) {

            SubWorks = new Dictionary<Type, Workflow<string, object, bool>.CreateWork>()
            {
                { typeof(TestWork), (_) => {
                    return new TestWork(window);
                }},
                { typeof(UserControlWork), (_) => {
                    return new UserControlWork(window);
                }},
                { typeof(ContextWork), (workflowContext) => {
                    return new ContextWork(window, workflowContext + " from ContextWork");
                }}
            };
        }
    }

    public interface ILoadingWindow
    {
        void ShowDebugInfo(string context, string parameters);
        void Loading(bool show, string message);
    }

    public partial class WindowWork : AdonisWindow, ILoadingWindow
    {
        public WindowWork()
        {
            InitializeComponent();
            Loaded += WindowWork_Loaded;
        }

        private async void WindowWork_Loaded(object sender, RoutedEventArgs e)
        {
            var workflow = new WorkflowTest(this);
            await workflow.Do();
            AdonisUI.Controls.MessageBox.Show("Finished !", "Info", AdonisUI.Controls.MessageBoxButton.OK, AdonisUI.Controls.MessageBoxImage.Information);
            this.Close();
        }

        public void Display(FrameworkElement element)
        {
            Loading(false);
            ContentContainer.Content = element;
        }

        public void Loading(bool show, string message = "")
        {
            if (show)
                LoadingContainer.Visibility = Visibility.Visible;
            else
                LoadingContainer.Visibility = Visibility.Collapsed;

            LoadingMessage.Text = message;
        }

        public void ShowDebugInfo(string context, string parameters)
        {
            TextDebugContext.Text = "Context : " + context;
            TextDebugParams.Text = "Params : " + parameters;
        }
    }
}
