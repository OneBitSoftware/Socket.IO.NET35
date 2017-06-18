using Socket.IO.NET35.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Unit
{
    public class TaskTests
    {
        Queue<TaskWorker> workerQueue = new Queue<TaskWorker>();

        /// <summary>
        /// This test validates that "cancelling" actually kills all background workers
        /// and especially the time Thread.Sleep
        /// The test should not wait 5 seconds
        /// </summary>
        [Fact]
        public void CancelAll_ShouldStopAllTasks()
        {
            // Arrange
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var task = new TaskWorker(this);
            var workDone = false;
            task.QueueWorkerWithDelay(this.workerQueue, null, (x, e) => 
                {
                    // some custom do work logic.
                    workDone = true;
                    throw new Exception("Thread not cancelled. Expected to never reach here.");
                }, (x, e) => {
                }, (e) => { }, (x, e) => {
                }, 5000);

            // Act
            task.CancelAll(workerQueue);
            watch.Stop();

            // Assert
            Assert.InRange(watch.ElapsedMilliseconds, 0, 500); //should be more than enough
            if (workDone) throw new Exception("Task not cancelled on time.");
        }

        [Fact]
        public void Task_MustCompleteWork()
        {
            // Arrange
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var task = new TaskWorker(this);
            var workDone = false;
            var completed = false;
            var manualResetEvent = new ManualResetEvent(false);

            // Act
            task.QueueWorker(this.workerQueue, null, (x, e) =>
            {
                workDone = true;
            }, (x, e) => {
                completed = true;
                manualResetEvent.Set(); // continue wait
            }, (e) => {

            }, (x, e) => {
            });

            manualResetEvent.WaitOne(1000); // pause until set

            // Assert
            watch.Stop();
            Assert.InRange(watch.ElapsedMilliseconds, 0, 500); //should be more than enough
            if (!workDone) throw new Exception("Task has not done any work.");
            if (!completed) throw new Exception("Task has not completed.");
        }

        /// <summary>
        /// This test ensures that queued items do work
        /// A 200 millisecond thread.sleep is added to ensure that 
        /// the background tasks can do work before the test completes, at which it will kill the tasks
        /// </summary>
        [Fact]
        public void AllTasks_MustCompleteWork()
        {
            // Arrange
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var task = new TaskWorker(this);
            var workDone1 = false;
            var workDone2 = false;
            var workDone3 = false;
            var workDone4 = false;
            var workDone5 = false;
            var workDone6 = false;
            var completed1 = false;
            var completed2 = false;
            var completed3 = false;
            var completed4 = false;
            var completed5 = false;
            var completed6 = false;

            // Act
            task.QueueWorker(this.workerQueue, null, (x, e) => { workDone1 = true; }, (x, e) => { completed1 = true; }, (e) => {}, (x, e) => {});
            task.QueueWorker(this.workerQueue, null, (x, e) => { workDone2 = true; }, (x, e) => { completed2 = true; }, (e) => { }, (x, e) => { });
            task.QueueWorker(this.workerQueue, null, (x, e) => { workDone3 = true; }, (x, e) => { completed3 = true; }, (e) => { }, (x, e) => { });
            task.QueueWorker(this.workerQueue, null, (x, e) => { workDone4 = true; }, (x, e) => { completed4 = true; }, (e) => { }, (x, e) => { });
            task.QueueWorker(this.workerQueue, null, (x, e) => { workDone5 = true; }, (x, e) => { completed5 = true; }, (e) => { }, (x, e) => { });
            task.QueueWorker(this.workerQueue, null, (x, e) => { workDone6 = true; }, (x, e) => { completed6 = true; }, (e) => { }, (x, e) => { });


            // This thread.sleep is necessary to ensure that the background threads
            // have enough time to start before the end of the unit test, after which
            // the runner will kill the appdomain
            // TODO: reqrite with ManualResetEvent
            System.Threading.Thread.Sleep(200); // used to ensure the below code is executed

            watch.Stop();

            // Assert
            Assert.InRange(watch.ElapsedMilliseconds, 0, 500); //should be more than enough
            if (!workDone1) throw new Exception("Task 1 has not done any work.");
            if (!workDone2) throw new Exception("Task 2 has not done any work.");
            if (!workDone3) throw new Exception("Task 3 has not done any work.");
            if (!workDone4) throw new Exception("Task 4 has not done any work.");
            if (!workDone5) throw new Exception("Task 5 has not done any work.");
            if (!workDone6) throw new Exception("Task 6 has not done any work.");
            if (!completed1) throw new Exception("Task 1 has not completed.");
            if (!completed2) throw new Exception("Task 2 has not completed.");
            if (!completed3) throw new Exception("Task 3 has not completed.");
            if (!completed4) throw new Exception("Task 4 has not completed.");
            if (!completed5) throw new Exception("Task 5 has not completed.");
            if (!completed6) throw new Exception("Task 6 has not completed.");
        }

        /// <summary>
        /// This test validates that "cancelling" actually kills all background workers
        /// and especially the time Thread.Sleep, when multiple tasks are queued
        /// The test should not wait 5 seconds
        /// </summary>
        [Fact]
        public void AllTasks_ShouldNotDoWork()
        {
            // Arrange
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var task = new TaskWorker(this);
            var manualResetEvent = new ManualResetEvent(false);

            var workDone1 = false;
            var workDone2 = false;
            var workDone3 = false;
            var workDone4 = false;
            var workDone5 = false;
            var workDone6 = false;
            var completed1 = false;
            var completed2 = false;
            var completed3 = false;
            var completed4 = false;
            var completed5 = false;
            var completed6 = false;

            // Act
            task.QueueWorkerWithDelay(this.workerQueue, null, (x, e) => {
                System.Threading.Thread.Sleep(100); // used to ensure the below code is executed
                workDone1 = true;
            }, (x, e) => {
                completed1 = true;
                manualResetEvent.Set();
            }, (e) => { }, (x, e) => { }, 5000);
            task.QueueWorker(this.workerQueue, null, (x, e) => { workDone2 = true; }, (x, e) => { completed2 = true; }, (e) => { }, (x, e) => { });
            task.QueueWorker(this.workerQueue, null, (x, e) => { workDone3 = true; }, (x, e) => { completed3 = true; }, (e) => { }, (x, e) => { });
            task.QueueWorker(this.workerQueue, null, (x, e) => { workDone4 = true; }, (x, e) => { completed4 = true; }, (e) => { }, (x, e) => { });
            task.QueueWorkerWithDelay(this.workerQueue, null, (x, e) => { workDone5 = true; }, (x, e) => { completed5 = true; }, (e) => { }, (x, e) => { }, 4000);
            task.QueueWorker(this.workerQueue, null, (x, e) => { workDone6 = true; }, (x, e) => { completed6 = true; }, (e) => { }, (x, e) => { });


            task.CancelAll(workerQueue);

            watch.Stop();
            manualResetEvent.WaitOne(1000);

            // Assert
            Assert.InRange(watch.ElapsedMilliseconds, 0, 500); //should be more than enough
            if (workDone1) throw new Exception("Task 1 has done work.");
            if (workDone2) throw new Exception("Task 2 has done work.");
            if (workDone3) throw new Exception("Task 3 has done work.");
            if (workDone4) throw new Exception("Task 4 has done work.");
            if (workDone5) throw new Exception("Task 5 has done work.");
            if (workDone6) throw new Exception("Task 6 has done work.");
            if (!completed1) throw new Exception("Task 1 should be completed.");
            //if (!completed2) throw new Exception("Task 2 should be completed.");
            //if (!completed3) throw new Exception("Task 3 should be completed.");
            //if (!completed4) throw new Exception("Task 4 should be completed.");
            //if (!completed5) throw new Exception("Task 5 should be completed.");
            //if (!completed6) throw new Exception("Task 6 should be completed.");
        }

        [Fact]
        public void RunAndWait_ShouldCompleteWork()
        {
            // Arrange
            var task = new TaskWorker(null);
            var workDone1 = false;
            var completed1 = false;

            // Act
            task.RunAndWait((x, e) => {
                workDone1 = true;
            }, (x, e) => {
                completed1 = true;
            }, 5000);

            // Assert
            if (!workDone1) throw new Exception("Task 1 has not done work.");
            if (!completed1) throw new Exception("Task 1 should be completed.");
        }

        //[Fact] TODO: implement cancelling
        //public void RunAndWait_ShouldCancel()
        //{
        //    // Arrange
        //    var task = new TaskWorker(null);
        //    var workDone1 = false;
        //    var completed1 = false;

        //    // Act
        //    task.RunAndWait((x, e) => {
        //        workDone1 = true;
        //    }, (x, e) => {
        //        completed1 = true;
        //    }, 1000);

        //    // Assert
        //    if (!workDone1) throw new Exception("Task 1 has not done work.");
        //    if (!completed1) throw new Exception("Task 1 should be completed.");
        //}
    }
}
