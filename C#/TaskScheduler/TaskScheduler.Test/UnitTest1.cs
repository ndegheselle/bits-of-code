using TaskScheduler.Lib;

namespace TaskScheduler.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task SimpleTask()
        {
            TestTask task = new TestTask(2);
            task.Inputs[1] = 2;
            await task.Do();
        }

        [TestMethod]
        public async Task LinearGraph()
        {
            TestLinearGraph graph = new TestLinearGraph(2);
            await graph.Do();
        }

        [TestMethod]
        public async Task TestGraph()
        {
            TestGraph graph = new TestGraph(2);
            await graph.Do();
        }
    }
}