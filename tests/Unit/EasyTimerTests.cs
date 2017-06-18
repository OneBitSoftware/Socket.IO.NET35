using Socket.IO.NET35;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Unit
{
    public class EasyTimerTests
    {
        [Fact]
        public void EasyTimer_ShouldCancelThread()
        {
            // Arrange
            var workDone1 = false;
            var watch = System.Diagnostics.Stopwatch.StartNew();

            // Act
            var timer = EasyTimer.SetTimeout(() => {
                workDone1 = true;
            }, 5000);
            timer.Stop();
            watch.Stop();

            // Assert
            if (workDone1) throw new Exception("Task 1 has done work. Expected to cancel.");
            Assert.InRange(watch.ElapsedMilliseconds, 0, 500); //should be more than enough

        }

        [Fact]
        public void EasyTimer_ShouldDoWork()
        {
            // Arrange
            var workDone1 = false;
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var manualResetEvent = new ManualResetEvent(false);

            // Act
            var timer = EasyTimer.SetTimeout(() => {
                workDone1 = true;
                manualResetEvent.Set(); // continue wait
            }, 0);

            watch.Stop();

            manualResetEvent.WaitOne(1000); // pause until set

            // Assert
            if (!workDone1) throw new Exception("Task 1 must do work.");
            Assert.InRange(watch.ElapsedMilliseconds, 0, 500); //should be more than enough
        }
    }
}
