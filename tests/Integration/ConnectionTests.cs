using SOCKETNET35 = Socket.IO.NET35;
using Socket.IO.NET35.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Socket.IO.NET35;
using Newtonsoft.Json.Linq;

namespace Integration
{
    public class ConnectionTests : BaseIntegrationTest
    {
        private ManualResetEvent ManualResetEvent = null;
        private SOCKETNET35.Socket socket;
        public string Message;
        private int Number;
        private bool Flag;

        [Fact]
        public void InstantiateIO_ShouldNotThrow()
        {
            var queryData = new Dictionary<string, string>();
            queryData.Add("token", "12345");
            var dataString = "Test" + DateTime.Now.Ticks.ToString();
            var socket = SOCKETNET35.IO.Socket(ServerUrl,
                new SOCKETNET35.IO.Options()
                {
                    Query = queryData
                });


            socket.On("hi", (data) =>
            {
                Assert.Equal(dataString, data);
                socket.Disconnect();
            });

            socket.On(Socket.IO.NET35.Socket.EVENT_CONNECT, () =>
            {
                socket.Emit("hi", dataString);
            });


            //System.Threading.Thread.Sleep(30000);

        }

        [Fact]
        public void ConnectDisconnect_ShouldReturnCorrectEvents()
        {
            var log = LogManager.GetLogger(GlobalHelper.CallerName());
            log.Info("Start ConnectDisconnect_ShouldReturnCorrectEvents");
            ManualResetEvent = new ManualResetEvent(false);

            var options = CreateOptions();
            var uri = CreateUri();
            socket = SOCKETNET35.IO.Socket(ServerUrl);
            socket.On(SOCKETNET35.Socket.EVENT_CONNECT, () =>
            {
                log.Info("EVENT_CONNECT");
                socket.Disconnect();
            });

            socket.On(SOCKETNET35.Socket.EVENT_DISCONNECT,
                (data) =>
                {
                    log.Info("EVENT_DISCONNECT");
                    Message = (string)data;
                    ManualResetEvent.Set();
                });

            //socket.Open();
            ManualResetEvent.WaitOne(15000);
            socket.Close();
            Assert.Equal("io client disconnect", this.Message);
        }

        [Fact]
        public void ConnectDisconnectConnect_ShouldReturnCorrectEvents()
        {
            var log = LogManager.GetLogger(GlobalHelper.CallerName());
            log.Info("Start ConnectDisconnectConnect_ShouldReturnCorrectEvents");
            ManualResetEvent = new ManualResetEvent(false);
            var connectCount = 0;
            var disconnectCount = 0;

            var options = CreateOptions();
            var uri = CreateUri();
            socket = SOCKETNET35.IO.Socket(ServerUrl, new IO.Options() { AutoConnect = false });

            socket.On(SOCKETNET35.Socket.EVENT_CONNECT, () =>
            {
                connectCount++;
                log.Info("EVENT_CONNECT");
                socket.Disconnect();
            });

            socket.On(SOCKETNET35.Socket.EVENT_DISCONNECT,
                (data) =>
                {
                    disconnectCount++;
                    log.Info("EVENT_DISCONNECT");
                    Message = (string)data;

                    if (connectCount < 2)
                    {
                        log.Info("Connecting count: " + connectCount);
                        socket.Connect();
                    }
                    else
                    {
                        ManualResetEvent.Set();
                    }
                });

            socket.Connect();
            ManualResetEvent.WaitOne(65000);
            Assert.Equal(2, connectCount);
            Assert.Equal(2, disconnectCount);
            socket.Close();
            Assert.Equal("io client disconnect", this.Message);
        }

        [Fact]
        public void CreateSession_ShouldReturnCorrectSessionData()
        {
            // Arrange
            var randomSessionData = DateTime.Now.Ticks.ToString() + Guid.NewGuid();
            var sessionId = Guid.NewGuid();
            var responseResult = string.Empty;
            var options = CreateOptions();
            var uri = CreateUri();
            ManualResetEvent = new ManualResetEvent(false);

            socket = SOCKETNET35.IO.Socket(ServerUrl);

            socket.On(SOCKETNET35.Socket.EVENT_CONNECT, () =>
            {
                socket.Emit("createSession", sessionId, randomSessionData);
            });

            socket.On("createdSession", (responseData) =>
            {
                responseResult = responseData.ToString();
                ManualResetEvent.Set();
            });

            // Act
            ManualResetEvent.WaitOne(15000);
            socket.Close();

            // Assert
            Assert.Equal(randomSessionData, responseResult);
            Assert.NotEqual(string.Empty, responseResult);
            Assert.NotEqual(" ", responseResult);
        }

        [Fact]
        public void MessageTest()
        {
            //var log = LogManager.GetLogger(GlobalHelper.CallerName());
            //log.Info("Start");
            ManualResetEvent = new ManualResetEvent(false);
            var events = new Queue<object>();
            var uniqueString = DateTime.Now.Ticks.ToString();
            var options = CreateOptions();
            var uri = CreateUri();
            socket = IO.Socket(uri, options);
            socket.On(SOCKETNET35.Socket.EVENT_CONNECT, () =>
            {
                socket.Emit("hi", uniqueString);
            });

            socket.On("hi",
                (data) =>
                {
                    events.Enqueue(data);
                    ManualResetEvent.Set();
                });

            ManualResetEvent.WaitOne(15000);
            socket.Close();
            if (events.Count < 1) throw new Exception("No work has been done");
            var obj = events.Dequeue();
            Assert.Equal(uniqueString, obj);
        }


        [Fact]
        public void ShouldNotConnectWhenAutoconnectOptionSetToFalse()
        {
            var log = LogManager.GetLogger(GlobalHelper.CallerName());
            log.Info("Start");
            ManualResetEvent = new ManualResetEvent(false);

            var options = CreateOptions();
            options.AutoConnect = false;
            var uri = CreateUri();
            socket = IO.Socket(uri, options);
            Assert.Null(socket.Io().EngineSocket);
        }

        [Fact]
        public void ShouldWorkWithAcks()
        {
            var log = LogManager.GetLogger(GlobalHelper.CallerName());
            log.Info("Start");
            ManualResetEvent = new ManualResetEvent(false);

            var result = "";

            var options = CreateOptions();
            var uri = CreateUri();
            socket = IO.Socket(uri, options);
            socket.Emit("ack");

            socket.On("ack", (cb) =>
            {
                var obj = new JObject();
                obj["b"] = true;
                var iack = (IAck)cb;
                iack.Call(5, obj);
            });

            socket.On("got it",
                (data) =>
                {
                    log.Info("got it");
                    Assert.Null(data);
                    result = "got it";
                    ManualResetEvent.Set();
                });

            ManualResetEvent.WaitOne(15000);
            socket.Close();
            Assert.Equal("got it", result);
        }

        [Fact]
        public void ShouldReceiveDateWithAck()
        {
            var log = LogManager.GetLogger(GlobalHelper.CallerName());
            log.Info("Start");
            Message = "";
            Number = 0;
            ManualResetEvent = new ManualResetEvent(false);
            var obj = new JObject();
            obj["b"] = true;

            var options = CreateOptions();
            var uri = CreateUri();
            socket = IO.Socket(uri, options);
            socket.Emit("getAckDate", new AckImpl((date, n) =>
            {
                log.Info("getAckDate data=" + date);
                Message = ((DateTime)date).ToString("O");
                Number = int.Parse(n.ToString());
                ManualResetEvent.Set();
            }), obj);


            ManualResetEvent.WaitOne(15000);
            Assert.Equal(28, Message.Length);
            Assert.Equal(5, Number);
            socket.Close();
        }

        [Fact]
        public void ShouldReceiveDateWithAckAsAction()
        {
            var log = LogManager.GetLogger(GlobalHelper.CallerName());
            log.Info("Start");
            Message = "";
            Number = 0;
            ManualResetEvent = new ManualResetEvent(false);
            var obj = new JObject();
            obj["b"] = true;

            var options = CreateOptions();
            var uri = CreateUri();
            socket = IO.Socket(uri, options);
            socket.Emit("getAckDate", (date, n) =>
            {
                log.Info("getAckDate data=" + date);
                Message = ((DateTime)date).ToString("O");
                Number = int.Parse(n.ToString());
                ManualResetEvent.Set();
            }, obj);


            ManualResetEvent.WaitOne(15000);
            Assert.Equal(28, Message.Length);
            Assert.Equal(5, Number);
            socket.Close();
        }


        [Fact]
        public void ShouldWorkWithFalse()
        {
            var log = LogManager.GetLogger(GlobalHelper.CallerName());
            log.Info("Start");
            ManualResetEvent = new ManualResetEvent(false);
            var events = new Queue<object>();

            var options = CreateOptions();
            var uri = CreateUri();
            socket = IO.Socket(uri, options);
            socket.On(SOCKETNET35.Socket.EVENT_CONNECT, () =>
            {
                socket.Emit("false");
            });

            socket.On("false", (data) =>
                {
                    events.Enqueue(data);
                    ManualResetEvent.Set();
                });

            ManualResetEvent.WaitOne(15000);
            socket.Close();
            var obj = events.Dequeue();
            Assert.False((bool)obj);
        }


        [Fact]
        public void ShouldReceiveUtf8MultibyteCharacters()
        {
            var log = LogManager.GetLogger(GlobalHelper.CallerName());
            log.Info("Start");
            ManualResetEvent = new ManualResetEvent(false);
            var events = new Queue<object>();
            var result = string.Empty;
            var correct = new string[]
            {
                "てすと",
                "Я Б Г Д Ж Й",
                "Ä ä Ü ü ß",
                "李O四",
                "utf8 — string"
            };
            var i = 0;

            var options = CreateOptions();
            options.Transports = new List<string>() { "polling" };
            var uri = CreateUri();
            socket = IO.Socket(uri, options);

            socket.On("takeUtf8", (data) =>
            {
                events.Enqueue(data);
                i++;
                log.Info(string.Format("takeUtf8 data={0} i={1}", (string)data, i));
                result = "1234";
                if (i >= correct.Length)
                {
                    ManualResetEvent.Set();
                }
            });


            socket.Emit("getUtf8");
            ManualResetEvent.WaitOne(15000);
            socket.Close();
            var j = 0;
            foreach (var obj in events)
            {
                var str = (string)obj;
                Assert.Equal(correct[j++], str);
            }
            Assert.Equal("1234", result);

        }


        //[Fact]
        //public void ShouldReceiveUtf8MultibyteCharactersOverWebsocket()
        //{
        //    var log = LogManager.GetLogger(GlobalHelper.CallerName());
        //    log.Info("Start");
        //    ManualResetEvent = new ManualResetEvent(false);
        //    var events = new Queue<object>();

        //    var correct = new string[]
        //    {
        //        "てすと",
        //        "Я Б Г Д Ж Й",
        //        "Ä ä Ü ü ß",
        //        "utf8 — string",
        //        "utf8 — string"
        //    };
        //    var i = 0;

        //    var options = CreateOptions();
        //    var uri = CreateUri();
        //    socket = IO.Socket(uri, options);

        //    socket.On("takeUtf8", (data) =>
        //    {
        //        events.Enqueue(data);
        //        i++;
        //        log.Info(string.Format("takeUtf8 data={0} i={1}", (string)data, i));

        //        if (i >= correct.Length)
        //        {
        //            ManualResetEvent.Set();
        //        }
        //    });


        //    socket.Emit("getUtf8");
        //    ManualResetEvent.WaitOne();
        //    socket.Close();
        //    var j = 0;
        //    foreach (var obj in events)
        //    {
        //        var str = (string)obj;
        //        Assert.Equal(correct[j++], str);
        //    }
        //}

        //[Fact]
        //public void ShouldConnectToANamespaceAfterConnectionEstablished()
        //{
        //    var log = LogManager.GetLogger(GlobalHelper.CallerName());
        //    log.Info("Start");
        //    ManualResetEvent = new ManualResetEvent(false);
        //    Flag = false;

        //    var options = CreateOptions();
        //    var uri = CreateUri();

        //    var manager = new Manager( new Uri(uri), options);
        //    socket = manager.Socket("/");

        //    socket.On(SOCKETNET35.Socket.EVENT_CONNECT, () =>
        //    {
        //        var foo = manager.Socket("/foo");
        //        foo.On(SOCKETNET35.Socket.EVENT_CONNECT, () =>
        //        {
        //            Flag = true;
        //            foo.Disconnect();
        //            socket.Disconnect();
        //            manager.Close();
        //            ManualResetEvent.Set();
        //        });
        //        foo.Open();
        //    });


        //    ManualResetEvent.WaitOne();
        //    Assert.True(Flag);
        //}

        [Fact]
        public void ShouldOpenANewNamespaceAfterConnectionGetsClosed()
        {
            var log = LogManager.GetLogger(GlobalHelper.CallerName());
            log.Info("Start");
            ManualResetEvent = new ManualResetEvent(false);
            Flag = false;

            var options = CreateOptions();
            var uri = CreateUri();

            var manager = new Manager(new Uri(uri), options);
            socket = manager.Socket("/");

            socket.On(SOCKETNET35.Socket.EVENT_CONNECT, () =>
            {
                socket.Disconnect();
            });

            socket.On(SOCKETNET35.Socket.EVENT_DISCONNECT, () =>
            {
                var foo = manager.Socket("/foo");
                foo.On(SOCKETNET35.Socket.EVENT_CONNECT, () =>
                {
                    Flag = true;
                    foo.Disconnect();
                    socket.Disconnect();
                    manager.Close();
                    ManualResetEvent.Set();
                });
                foo.Open();
            });
            var result = ManualResetEvent.WaitOne(15000);
            Assert.True(Flag);
            socket.Close();
        }



        [Fact]
        public void ReconnectEventShouldFireInSocket()
        {
            var log = LogManager.GetLogger(GlobalHelper.CallerName());
            log.Info("Start");
            ManualResetEvent = new ManualResetEvent(false);
            Flag = false;

            var options = CreateOptions();
            var uri = CreateUri();

            var manager = new Manager(new Uri(uri), options);
            socket = manager.Socket("/");

            socket.On(SOCKETNET35.Socket.EVENT_RECONNECT, () =>
            {
                log.Info("EVENT_RECONNECT");
                Flag = true;

                ManualResetEvent.Set();
            });

            //Task.Delay(2000).Wait();
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2));
            manager.EngineSocket.Close();
            ManualResetEvent.WaitOne(15000);
            log.Info("before EngineSocket close");

            Assert.True(Flag);
        }

        [Fact]
        public void ShouldReconnectByDefault()
        {
            var log = LogManager.GetLogger(GlobalHelper.CallerName());
            log.Info("Start");
            ManualResetEvent = new ManualResetEvent(false);
            Flag = false;

            var options = CreateOptions();
            var uri = CreateUri();

            var manager = new Manager(new Uri(uri), options);
            socket = manager.Socket("/");

            manager.On(SOCKETNET35.Socket.EVENT_RECONNECT, () =>
            {
                log.Info("EVENT_RECONNECT");
                Flag = true;
                socket.Disconnect();
                ManualResetEvent.Set();
            });

            //await Task.Delay(500);
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
            log.Info("before EngineSocket close");
            manager.EngineSocket.Close();

            ManualResetEvent.WaitOne(15000);
            Assert.True(Flag);
        }

        [Fact]
        public void ShouldTryToReconnectTwiceAndFailWhenRequestedTwoAttemptsWithImmediateTimeoutAndReconnectEnabled()
        {
            var log = LogManager.GetLogger(GlobalHelper.CallerName());
            log.Info("Start");
            ManualResetEvent = new ManualResetEvent(false);
            Flag = false;
            var reconnects = 0;

            var options = CreateOptions();
            options.Reconnection = true;
            options.Timeout = 0;
            options.ReconnectionAttempts = 2;
            options.ReconnectionDelay = 10;
            var uri = CreateUri();

            var manager = new Manager(new Uri(uri), options);

            manager.On(Manager.EVENT_RECONNECT_ATTEMPT, () =>
            {
                log.Info("EVENT_RECONNECT_ATTEMPT");
                reconnects++;
            });

            manager.On(Manager.EVENT_RECONNECT_FAILED, () =>
            {
                log.Info("EVENT_RECONNECT_FAILED");
                Flag = true;
                manager.Close();
                ManualResetEvent.Set();
            });

            ManualResetEvent.WaitOne(15000);
            Assert.True(Flag);
            Assert.Equal(2, reconnects);
        }

        [Fact]
        public void ShouldFireReconnectEventsOnSocket()
        {
            var log = LogManager.GetLogger(GlobalHelper.CallerName());
            log.Info("Start");
            ManualResetEvent = new ManualResetEvent(false);
            Flag = false;
            var reconnects = 0;
            var events = new Queue<int>();

            var correct = new int[]
            {
                1,2,2
            };

            var options = CreateOptions();
            options.Reconnection = true;
            options.Timeout = 0;
            options.ReconnectionAttempts = 2;
            options.ReconnectionDelay = 10;
            var uri = CreateUri();

            var manager = new Manager(new Uri(uri), options);
            socket = manager.Socket("/timeout_socket");

            socket.On(SOCKETNET35.Socket.EVENT_RECONNECT_ATTEMPT, (attempts) =>
            {
                log.Info("EVENT_RECONNECT_ATTEMPT");
                reconnects++;
                events.Enqueue(int.Parse((attempts).ToString()));
            });

            socket.On(SOCKETNET35.Socket.EVENT_RECONNECT_FAILED, () =>
            {
                log.Info("EVENT_RECONNECT_FAILED");
                Flag = true;
                events.Enqueue(reconnects);
                socket.Close();
                manager.Close();
                ManualResetEvent.Set();
            });

            ManualResetEvent.WaitOne(1000);
            var j = 0;
            foreach (var number in events)
            {
                Assert.Equal(correct[j++], number);
            }
        }


        [Fact]
        public void ShouldFireErrorOnSocket()
        {
            var log = LogManager.GetLogger(GlobalHelper.CallerName());
            log.Info("Start");
            ManualResetEvent = new ManualResetEvent(false);
            Flag = false;
            var events = new Queue<object>();

            var options = CreateOptions();
            options.Reconnection = true;
            var uri = CreateUri();

            var manager = new Manager(new Uri(uri), options);
            socket = manager.Socket("/timeout_socket");

            socket.On(SOCKETNET35.Socket.EVENT_ERROR, (e) =>
            {
                var exception = (SocketEngineException)e;
                log.Info("EVENT_ERROR");
                events.Enqueue(exception.code);
                socket.Close();
                manager.Close();
                ManualResetEvent.Set();
            });

            socket.On(SOCKETNET35.Socket.EVENT_CONNECT, () =>
            {
                log.Info("EVENT_CONNECT");
                manager.EngineSocket.OnPacket(new EnginePacket(Packet.ERROR, "test"));
            });

            ManualResetEvent.WaitOne(15000);
            var obj = (string)events.Dequeue();
            Assert.Equal("test", obj);
        }

        [Fact]
        public void ShouldFireReconnectingOnSocketWithAttemptsNumberWhenReconnectingTwice()
        {
            var log = LogManager.GetLogger(GlobalHelper.CallerName());
            log.Info("Start");
            ManualResetEvent = new ManualResetEvent(false);
            Flag = false;
            var reconnects = 0;
            var events = new Queue<int>();
            var result = string.Empty;

            var correct = new int[]
            {
                1,2,2
            };

            var options = CreateOptions();
            options.Reconnection = true;
            options.Timeout = 0;
            options.ReconnectionAttempts = 2;
            options.ReconnectionDelay = 10;
            var uri = CreateUri();

            var manager = new Manager(new Uri(uri), options);
            socket = manager.Socket("/timeout_socket");

            socket.On(SOCKETNET35.Socket.EVENT_RECONNECTING, (attempts) =>
            {
                reconnects++;
                events.Enqueue(int.Parse((attempts).ToString()));
            });

            socket.On(SOCKETNET35.Socket.EVENT_RECONNECT_FAILED, () =>
            {
                log.Info("EVENT_RECONNECT_FAILED");
                Flag = true;
                events.Enqueue(reconnects);
                socket.Close();
                manager.Close();
                result = "1234";
                ManualResetEvent.Set();
            });

            ManualResetEvent.WaitOne(1000);
            var j = 0;
            foreach (var number in events)
            {
                Assert.Equal(correct[j++], number);
            }
            Assert.Equal("1234", result);

        }

        [Fact]
        public void ShouldNotTryToReconnectAndShouldFormAConnectionWhenConnectingToCorrectPortWithDefaultTimeout()
        {
            var log = LogManager.GetLogger(GlobalHelper.CallerName());
            log.Info("Start");
            ManualResetEvent = new ManualResetEvent(false);
            Flag = false;


            var options = CreateOptions();
            options.Reconnection = true;
            options.ReconnectionDelay = 10;
            var uri = CreateUri();

            var manager = new Manager(new Uri(uri), options);
            socket = manager.Socket("/valid");

            manager.On(Manager.EVENT_RECONNECT_ATTEMPT, () =>
            {
                Flag = true;
            });

            socket.On(SOCKETNET35.Socket.EVENT_CONNECT, () =>
            {
                // set a timeout to let reconnection possibly fire
                log.Info("EVENT_CONNECT");
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));

                ManualResetEvent.Set();
            });

            ManualResetEvent.WaitOne(15000);
            log.Info("after WaitOne");
            socket.Close();
            manager.Close();
            Assert.False(Flag);
        }

        [Fact]
        public void ShouldTryToReconnectTwiceAndFailWhenRequestedTwoAttemptsWithIncorrectAddressAndReconnectEnabled()
        {
            var log = LogManager.GetLogger(GlobalHelper.CallerName());
            log.Info("Start");
            ManualResetEvent = new ManualResetEvent(false);
            Flag = false;
            var reconnects = 0;

            var options = CreateOptions();
            options.Reconnection = true;
            options.ReconnectionAttempts = 2;
            options.ReconnectionDelay = 10;
            var uri = "http://localhost:3940";


            var manager = new Manager(new Uri(uri), options);
            socket = manager.Socket("/asd");

            manager.On(Manager.EVENT_RECONNECT_ATTEMPT, () =>
            {
                log.Info("EVENT_RECONNECT_ATTEMPT");
                reconnects++;
            });

            manager.On(Manager.EVENT_RECONNECT_FAILED, () =>
            {
                log.Info("EVENT_RECONNECT_FAILED");
                Flag = true;
                socket.Disconnect();
                manager.Close();
                ManualResetEvent.Set();
            });

            ManualResetEvent.WaitOne(15000);
            Assert.Equal(2, reconnects);
        }

        [Fact]
        public void ShouldNotTryToReconnectWithIncorrectPortWhenReconnectionDisabled()
        {
            var log = LogManager.GetLogger(GlobalHelper.CallerName());
            log.Info("Start");
            ManualResetEvent = new ManualResetEvent(false);
            Flag = false;


            var options = CreateOptions();
            options.Reconnection = false;
            var uri = "http://localhost:3940";

            var manager = new Manager(new Uri(uri), options);
            socket = manager.Socket("/invalid");

            manager.On(Manager.EVENT_RECONNECT_ATTEMPT, () =>
            {
                Flag = true;
            });

            manager.On(Manager.EVENT_CONNECT_ERROR, () =>
            {
                // set a timeout to let reconnection possibly fire
                log.Info("EVENT_CONNECT_ERROR");
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
                ManualResetEvent.Set();
            });

            ManualResetEvent.WaitOne(15000);
            log.Info("after WaitOne");
            socket.Disconnect();
            manager.Close();
            Assert.False(Flag);
        }

        [Fact]
        public void ShouldEmitDateAsDate()
        {
            var log = LogManager.GetLogger(GlobalHelper.CallerName());
            log.Info("Start");
            ManualResetEvent = new ManualResetEvent(false);
            var events = new Queue<object>();

            var options = CreateOptions();
            var uri = CreateUri();
            socket = IO.Socket(uri, options);
            socket.On("takeDate", (data) =>
            {
                log.Info("takeDate");
                events.Enqueue(data);
                ManualResetEvent.Set();
            });

            socket.Emit("getDate");

            //socket.Open();
            ManualResetEvent.WaitOne(15000);
            socket.Close();
            var obj = events.Dequeue();
            Assert.IsType<DateTime>(obj);
        }



        [Fact]
        public void ShouldEmitDateInObject()
        {
            var log = LogManager.GetLogger(GlobalHelper.CallerName());
            log.Info("Start");
            ManualResetEvent = new ManualResetEvent(false);
            var events = new Queue<object>();

            var options = CreateOptions();
            var uri = CreateUri();
            socket = IO.Socket(uri, options);
            socket.On("takeDateObj", (data) =>
            {
                log.Info("takeDate");
                events.Enqueue(data);
                ManualResetEvent.Set();
            });

            socket.Emit("getDateObj");

            //socket.Open();
            ManualResetEvent.WaitOne(15000);
            socket.Close();
            var obj = (JObject)events.Dequeue();

            Assert.IsType<JObject>(obj);
            var date = (obj["date"]).Value<DateTime>();
            Assert.IsType<DateTime>(date);
        }

        [Fact]
        public void ShouldGetBase64DataAsALastResort()
        {
            var log = LogManager.GetLogger(GlobalHelper.CallerName());
            log.Info("Start");
            ManualResetEvent = new ManualResetEvent(false);
            var events = new Queue<object>();

            var options = CreateOptions();
            var uri = CreateUri();
            socket = IO.Socket(uri, options);
            socket.On("takebin", (data) =>
            {
                events.Enqueue(data);
                ManualResetEvent.Set();
            });

            socket.Emit("getbin");

            //socket.Open();
            ManualResetEvent.WaitOne(15000);
            socket.Close();

            var binData = (byte[])events.Dequeue();
            var exptected = System.Text.Encoding.UTF8.GetBytes("asdfasdf");
            var i = 0;
            foreach (var b in exptected)
            {
                Assert.Equal(b, binData[i++]);
            }

        }

        [Fact]
        public void ShouldGetBinaryDataAsAnArraybuffer()
        {
            var log = LogManager.GetLogger(GlobalHelper.CallerName());
            log.Info("Start");
            ManualResetEvent = new ManualResetEvent(false);
            var events = new Queue<object>();

            var options = CreateOptions();
            var uri = CreateUri();
            socket = IO.Socket(uri, options);
            socket.On("doge", (data) =>
            {
                events.Enqueue(data);
                ManualResetEvent.Set();
            });

            socket.Emit("doge");

            //socket.Open();
            ManualResetEvent.WaitOne(15000);
            socket.Close();

            var binData = (byte[])events.Dequeue();
            var exptected = System.Text.Encoding.UTF8.GetBytes("asdfasdf");
            var i = 0;
            foreach (var b in exptected)
            {
                Assert.Equal(b, binData[i++]);
            }
        }

        [Fact]
        public void ShouldSendBinaryDataAsAnArraybuffer()
        {
            var log = LogManager.GetLogger(GlobalHelper.CallerName());
            log.Info("Start");
            ManualResetEvent = new ManualResetEvent(false);
            Flag = false;

            var exptected = System.Text.Encoding.UTF8.GetBytes("asdfasdf");

            var options = CreateOptions();
            var uri = CreateUri();
            socket = IO.Socket(uri, options);
            socket.On("buffack", () =>
            {
                Flag = true;
                ManualResetEvent.Set();
            });

            socket.Emit("buffa", exptected);

            ManualResetEvent.WaitOne(15000);
            socket.Close();
            Assert.True(Flag);
        }

        [Fact]
        public void BuffAck()
        {
            var log = LogManager.GetLogger(GlobalHelper.CallerName());
            log.Info("Start");
            ManualResetEvent = new ManualResetEvent(false);
            Flag = false;

            var exptected = System.Text.Encoding.UTF8.GetBytes("asdfasdf");

            var options = CreateOptions();
            //options.Transports = ImmutableList.Create<string>(Polling.NAME);
            var uri = CreateUri();
            socket = IO.Socket(uri, options);
            socket.On("buffack", () =>
            {
                Flag = true;
                ManualResetEvent.Set();
            });

            socket.Emit("buffa", exptected);

            ManualResetEvent.WaitOne(15000);
            //Task.Delay(8000).Wait();
            socket.Close();
            //Task.Delay(4000).Wait();
            Assert.True(Flag);
            log.Info("Finish");
        }

        [Fact]
        public void DoubleCallTest()
        {
            ShouldSendBinaryDataAsAnArraybufferMixedWithJson();
            ShouldSendBinaryDataAsAnArraybufferMixedWithJson();
            ShouldSendBinaryDataAsAnArraybufferMixedWithJson();

        }

        [Fact]
        public void ShouldSendBinaryDataAsAnArraybufferMixedWithJson()
        {
            var log = LogManager.GetLogger(GlobalHelper.CallerName());
            log.Info("Start");
            ManualResetEvent = new ManualResetEvent(false);
            Flag = false;

            var buf = System.Text.Encoding.UTF8.GetBytes("howdy");
            var jobj = new JObject();
            jobj.Add("hello", "lol");
            jobj.Add("message", buf);
            jobj.Add("goodbye", "gotcha");

            var options = CreateOptions();
            //options.Transports = ImmutableList.Create<string>(Polling.NAME);

            var uri = CreateUri();
            socket = IO.Socket(uri, options);
            socket.On("jsonbuff-ack", () =>
            {
                Flag = true;
                ManualResetEvent.Set();
            });

            socket.On(SOCKETNET35.Socket.EVENT_DISCONNECT, () =>
            {
                log.Info("EVENT_DISCONNECT");
            });

            socket.Emit("jsonbuff", jobj);

            ManualResetEvent.WaitOne(15000);
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));

            log.Info("About to wait 1sec");

            //Task.Delay(1000).Wait();
            log.Info("About to call close");
            socket.Close();
            //Task.Delay(1000).Wait();
            Assert.True(Flag);
            log.Info("Finish");
        }

        [Fact]
        public void ShouldSendEventsWithArraybuffersInTheCorrectOrder()
        {
            var log = LogManager.GetLogger(GlobalHelper.CallerName());
            log.Info("Start");
            ManualResetEvent = new ManualResetEvent(false);
            Flag = false;
            var buf = System.Text.Encoding.UTF8.GetBytes("abuff1");

            var options = CreateOptions();
            var uri = CreateUri();
            socket = IO.Socket(uri, options);
            socket.On("abuff2-ack", () =>
            {
                Flag = true;
                ManualResetEvent.Set();
            });


            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(5));
            socket.Emit("abuff1", buf);
            socket.Emit("abuff2", "please arrive second");
            ManualResetEvent.WaitOne(15000);
            Assert.True(Flag);
        }


        [Fact]
        public void D10000CharsTest()
        {
            var log = LogManager.GetLogger(GlobalHelper.CallerName());
            log.Info("Start");
            ManualResetEvent = new ManualResetEvent(false);
            var events = new Queue<object>();

            var options = CreateOptions();
            var uri = CreateUri();
            socket = IO.Socket(uri, options);
            socket.On(SOCKETNET35.Socket.EVENT_CONNECT, () =>
            {
                log.Info("EVENT_CONNECT");
                socket.Emit("d10000chars");
            });

            socket.On("d10000chars",
                (data) =>
                {
                    log.Info("EVENT_MESSAGE data=" + data);
                    events.Enqueue(data);
                    ManualResetEvent.Set();
                });

            //socket.Open();
            ManualResetEvent.WaitOne(15000);
            socket.Close();
            var obj = (string)events.Dequeue();
            Assert.Equal(10000, obj.Length);
        }


        [Fact]
        public void D100000CharsTest()
        {
            var log = LogManager.GetLogger(GlobalHelper.CallerName());
            log.Info("Start");
            ManualResetEvent = new ManualResetEvent(false);
            var events = new Queue<object>();

            var options = CreateOptions();
            var uri = CreateUri();
            socket = IO.Socket(uri, options);
            socket.On(SOCKETNET35.Socket.EVENT_CONNECT, () =>
            {
                log.Info("EVENT_CONNECT");
                socket.Emit("d100000chars");
            });

            socket.On("d100000chars",
                (data) =>
                {
                    log.Info("EVENT_MESSAGE data=" + data);
                    events.Enqueue(data);
                    ManualResetEvent.Set();
                });

            //socket.Open();
            ManualResetEvent.WaitOne(160000);
            socket.Close();
            var obj = (string)events.Dequeue();
            Assert.Equal(100000, obj.Length);
        }

        [Fact]
        public void Json10000CharsTest()
        {
            var log = LogManager.GetLogger(GlobalHelper.CallerName());
            log.Info("Start");
            ManualResetEvent = new ManualResetEvent(false);
            var events = new Queue<object>();

            var options = CreateOptions();
            var uri = CreateUri();
            socket = IO.Socket(uri, options);
            socket.On(SOCKETNET35.Socket.EVENT_CONNECT, () =>
            {
                log.Info("EVENT_CONNECT");
                socket.Emit("json10000chars");
            });

            socket.On("json10000chars",
                (data) =>
                {
                    log.Info("EVENT_MESSAGE data=" + data);
                    events.Enqueue(data);
                    ManualResetEvent.Set();
                });

            //socket.Open();
            ManualResetEvent.WaitOne(15000);
            socket.Close();
            var obj = (JObject)events.Dequeue();
            var str = (string)obj["data"];
            Assert.Equal(10000, str.Length);
        }

        [Fact]
        public void Json10000000CharsTest()
        {
            var log = LogManager.GetLogger(GlobalHelper.CallerName());
            log.Info("Start");
            ManualResetEvent = new ManualResetEvent(false);
            var events = new Queue<object>();

            var options = CreateOptions();
            var uri = CreateUri();
            socket = IO.Socket(uri, options);
            socket.On(SOCKETNET35.Socket.EVENT_CONNECT, () =>
            {
                log.Info("EVENT_CONNECT");
                socket.Emit("json10000000chars");
            });

            socket.On("json10000000chars",
                (data) =>
                {
                    log.Info("EVENT_MESSAGE data=" + data);
                    events.Enqueue(data);
                    ManualResetEvent.Set();
                });

            //socket.Open();
            ManualResetEvent.WaitOne(15000);
            socket.Close();
            var obj = (JObject)events.Dequeue();
            var str = (string)obj["data"];
            Assert.Equal(10000000, str.Length);
        }


    }
}
