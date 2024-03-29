using System.Diagnostics;

namespace BitsOfCode.DependencyInjection
{
    public class TestService
    {
        public string DoSomething()
        {
            return "Some test service work";
        }
    }

    public class TestClass
    {
        public TestClass(TestService testService)
        {
            Console.WriteLine(testService.DoSomething());
        }
    }

    class Program
    {
        static void Main()
        {
            ServiceResolver serviceResolver = new ServiceResolver();
            serviceResolver.AddSingleton<TestService>();
            var test = serviceResolver.InjectDependencies<TestClass>();
        }
    }
}