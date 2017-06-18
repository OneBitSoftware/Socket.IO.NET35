using Socket.IO.NET35;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Integration
{
    public class Threading : BaseIntegrationTest
    {
        [Fact]
        public void TestThreadingRelease()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var manager = new Manager(new Uri(ServerUrl));
            manager.Close();
            watch.Stop();
            Assert.InRange(watch.ElapsedMilliseconds, 0, 1000); //should be more than enough
        }
    }
}
