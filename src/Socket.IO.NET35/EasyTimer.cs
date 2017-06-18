using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Socket.IO.NET35
{
    public class EasyTimer
    {
        private CancellationTokenSource _ts;

        public EasyTimer(CancellationTokenSource ts)
        {
            this._ts = ts;
        }

        public static EasyTimer SetTimeout(Action method, int delayInMilliseconds)
        {
            var ts = new CancellationTokenSource();
            var ct = ts.Token;

            var worker = new BackgroundWorker();

            worker.DoWork += (s, e) => {
                //System.Threading.Thread.Sleep(delayInMilliseconds);

                if (worker.CancellationPending == false)
                {
                    if (delayInMilliseconds > 0)
                    {
                        var cancelled = ct.WaitHandle.WaitOne(delayInMilliseconds);

                        if (cancelled)
                        {
                            return;
                        }
                    }
                }
                else
                {
                    return;
                }

            };

            worker.RunWorkerCompleted += (s, e) =>
            {
                if (!ts.IsCancellationRequested && !worker.CancellationPending)
                {
                    //Task.Factory.StartNew(method, ct, TaskCreationOptions.AttachedToParent, TaskScheduler.Current).Wait();
                    if (method != null)
                        method.Invoke();
                }
            };

            worker.RunWorkerAsync();

            // Returns a stop handle which can be used for stopping
            // the timer, if required
            // The static SetTimeOut returns an instance with the new ts in the constructor
            // The caller can then call EasyTimer.Stop
            return new EasyTimer(ts);
        }

        public void Stop()
        {
            //var log = LogManager.GetLogger(Global.CallerName());
            //log.Info("EasyTimer stop");
            if (_ts != null)
            {
                _ts.Cancel();
            }
        }

        //public static void TaskRun(Action action)
        //{
        //    Task.Factory.StartNew(action).Wait();
        //}

        public static Task TaskRunNoWait(Action action)
        {
            return Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.AttachedToParent, TaskScheduler.Current);
        }
    }
}
