using TaskScheduler.Lib;

namespace TaskScheduler.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void SimpleTask()
        {
            TaskNode task = new TaskNode();
            task.Do();
        }
    }
}