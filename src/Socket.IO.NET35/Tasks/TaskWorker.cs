using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace Socket.IO.NET35.Tasks
{
    public class TaskWorker : BackgroundWorker
    {
        //public bool CancelAll { get; set; }
        public CancellationTokenSource CancellationTokenSource
        {
            get;
            private set;
        }
        public TaskWorker(object sender)
        {
            this.Sender = sender;
            this.WorkerSupportsCancellation = true;
            CancellationTokenSource = new CancellationTokenSource();
        }

        public object Sender { get; private set; }

        public void CancelAll(Queue<TaskWorker> queue)
        {
            CancellationTokenSource.Cancel();

            foreach (var item in queue)
            {
                item.CancellationTokenSource.Cancel();
                item.CancelAsync();
            }

            if (queue.Count > 0) queue.Clear();
        }
        public void Stop()
        {

        }

        public void QueueWorkerWithDelay(
            Queue<TaskWorker> queue,
            object item,
            Action<object, DoWorkEventArgs> action,
            Action<object, RunWorkerCompletedEventArgs> actionComplete,
            Action<RunWorkerCompletedEventArgs> displayError,
            Action<object, ProgressChangedEventArgs> progressChange,
            int millisecondsDelay)
        {
            if (queue == null)
                throw new ArgumentNullException("queue");

            using (var worker = new TaskWorker(item))
            {
                worker.WorkerReportsProgress = true;
                worker.WorkerSupportsCancellation = true;

                worker.ProgressChanged += (sender, args) =>
                {
                    if (worker.CancellationPending == false && progressChange != null)
                    {
                        progressChange.Invoke(sender, args);
                    }
                };

                worker.DoWork += (sender, args) =>
                {
                    if (worker.CancellationPending == false)
                    {
                        if (millisecondsDelay > 0)
                        {
                            var cancelled = worker.CancellationTokenSource.Token
                                .WaitHandle.WaitOne(millisecondsDelay);

                            if (cancelled)
                            {
                                args.Cancel = true;
                                return;
                            }
                        }

                        if (action != null)
                            action.Invoke(sender, args);
                    }
                    else
                    {
                        args.Cancel = true;
                        return;
                    }
                };

                // This method may not fire if the worker is destroyed prematurely! 
                worker.RunWorkerCompleted += (sender, args) =>
                {
                    if (actionComplete != null)
                        actionComplete.Invoke(sender, args);

                    if (queue.Count != 0) queue.Dequeue();
                    if (queue.Count > 0)
                    {
                        var next = queue.Peek();

                        if (next.WorkerReportsProgress)
                            next.ReportProgress(0, "Performing operation...");

                        next.RunWorkerAsync(next.Sender);
                    }
                    else
                    {
                        if (displayError != null)
                            displayError.Invoke(args);
                    }
                };

                queue.Enqueue(worker);

                if (queue.Count > 0)
                {
                    var next = queue.Peek();
                    if (next.WorkerReportsProgress)
                        next.ReportProgress(0, "Performing operation...");

                    if (!next.CancellationPending && !next.IsBusy)
                        next.RunWorkerAsync(next.Sender);
                }
            }
        }

        public void QueueWorker(Queue<TaskWorker> queue,
                            object item,
                            Action<object, DoWorkEventArgs> action,
                            Action<object, RunWorkerCompletedEventArgs> actionComplete,
                            Action<RunWorkerCompletedEventArgs> displayError,
                            Action<object, ProgressChangedEventArgs> progressChange)
        {
            QueueWorkerWithDelay(queue, item, action, actionComplete, displayError, progressChange, 0);
        }


        public void RunAndWait(
            Action<object, DoWorkEventArgs> action,
            Action<object, RunWorkerCompletedEventArgs> actionComplete,
            int millisecondsTimeout)
        {
            using (var worker = new TaskWorker(null))
            {
                // can be rewritten to support cancelling by adding
                // the worker to the TasksQueue and implementing
                // worker.CancellationTokenSource.Token.WaitHandle.WaitOne
                worker.WorkerReportsProgress = false;
                worker.WorkerSupportsCancellation = true;
                AutoResetEvent waitEvent = new AutoResetEvent(false);
                worker.DoWork += (sender, args) =>
                        {
                            if (worker.CancellationPending == false && !worker.CancellationTokenSource.IsCancellationRequested)
                            {
                                if (action != null)
                                    action.Invoke(sender, args);
                            }
                            else
                            {
                                args.Cancel = true;
                                return;
                            }
                        };

                worker.RunWorkerCompleted += (sender, args) =>
                {
                    if (actionComplete != null)
                        actionComplete.Invoke(sender, args);
                    waitEvent.Set();
                };

                worker.RunWorkerAsync();
                var waitResult = waitEvent.WaitOne(millisecondsTimeout);
                if (waitResult == false)
                {
                    throw new TimeoutException("Task thread timed out.");
                }
            }
        }
    }
}