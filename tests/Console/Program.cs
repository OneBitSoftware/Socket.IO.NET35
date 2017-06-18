using SOCKETNET35 = Socket.IO.NET35;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socket.IO.NET35.Tasks;

namespace Console
{
    class Program
    {
        private static SOCKETNET35.Socket _mainSocket { get; set; }
        static Queue<TaskWorker> workerQueue = new Queue<TaskWorker>();

        static void Main(string[] args)
        {
            System.Console.WriteLine("Starting application...");

            ConnectToSocket();
            //TestTimer();

            System.Console.WriteLine("Done.");

            while(true)
            {
                var key = System.Console.ReadKey();

                if (key.KeyChar == 13)
                {
                    var dataString = "Key " + DateTime.Now.Ticks.ToString();

                    //_mainSocket.Emit("hi", dataString);

                    System.Console.WriteLine("Emitted " + dataString);
                }
            }
        }

        private static void TestTimer()
        {
            var task = new TaskWorker(null);
            task.QueueWorker(
                workerQueue,
                null,
                (x, e) =>
                {
                    //// some custom do work logic.
                    System.Threading.Thread.Sleep(4000);
                    System.Console.WriteLine("wait task completed.");
                },
                (x, e) =>
                {
                    //// some custom completed logic.
                },
                (e) =>
                {
                    //// some custom display error logic.
                },
                (x, e) =>
                {
                    //// Progress change logic.
                    var p = e.ProgressPercentage;
                    var s = e.UserState.ToString();
                });

            //System.Threading.Thread.Sleep(4000);

            task.CancelAll(workerQueue);
            System.Console.WriteLine("Overall task completed.");

        }

        private static void ConnectToSocket()
        {
            var queryData = new Dictionary<string, string>();
            queryData.Add("token", "12345");
            var dataString = "Test" + DateTime.Now.Ticks.ToString();
            _mainSocket = SOCKETNET35.IO.Socket("http://localhost:3000/",
                new SOCKETNET35.IO.Options()
                {
                    Query = queryData,
                    RememberUpgrade = true
                });

            _mainSocket.On("hi", (data) =>
            {
                System.Console.WriteLine("Received data: " + data.ToString());
            });

            _mainSocket.On(Socket.IO.NET35.Socket.EVENT_CONNECT, () =>
            {
                System.Console.WriteLine("Connected...");
                _mainSocket.Emit("hi", dataString);
            });
        }
    }
}
