using Xunit.Abstractions;

namespace XJK
{
    public class BaseTestClass
    {
        private readonly ITestOutputHelper TestOutputHelper;

        public BaseTestClass(ITestOutputHelper output)
        {
            this.TestOutputHelper = output;
        }

        public void WriteLine(params object[] objs)
        {
            foreach (var o in objs)
            {
                TestOutputHelper.WriteLine(o.ToString());
            }
        }
    }
}
