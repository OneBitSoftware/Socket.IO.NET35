using SOCKETNET35 = Socket.IO.NET35;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socket.IO.NET35.Tasks;
using System.ComponentModel;

namespace Console
{
    class Program
    {
        private static SOCKETNET35.Socket _mainSocket { get; set; }
        static Queue<TaskWorker> _workerQueue = new Queue<TaskWorker>();
        static bool _runWorkers = true;
        static int _workerId = 0;

        static void Main(string[] args)
        {
            System.Console.WriteLine("Starting application...");

            ConnectToSocket();
            //TestTimer();
            //ManualEmit();

            RunLoad();

            System.Console.WriteLine("Running....");
            System.Console.ReadKey();
        }

        private static void RunLoad()
        {
            CreateWorkers(5);
        }

        private static void ManualEmit()
        {
            while (true)
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

        private static void SpawnWorker(string i)
        {
            var task = new TaskWorker(null);
            task.QueueWorker(
                _workerQueue,
                null,
                (x, e) =>
                {
                    //// some custom do work logic.
                    while (_runWorkers)
                    {
                        var dataString = task.Id + " Key " + DateTime.Now.Ticks.ToString();

                        _mainSocket.Emit("hi", dataString);

                        System.Console.WriteLine("Emitted " + dataString);

                        System.Threading.Thread.Sleep(50);
                    }
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
                });
        }

        private static void CreateWorkers(int workerCount)
        {
            for (int i = 0; i < workerCount; i++)
            {
                SpawnWorker(i.ToString());
            }
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
