using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socket.IO.NET35
{
    public class On
    {
        private On() { }

        public static IHandle Create(Emitter obj, string ev, IListener fn)
        {
            obj.On(ev, fn);
            return new HandleImpl(obj, ev, fn);
        }

        public class HandleImpl : IHandle
        {
            private Emitter obj;
            private string ev;
            private IListener fn;

            public HandleImpl(Emitter obj, string ev, IListener fn)
            {
                this.obj = obj;
                this.ev = ev;
                this.fn = fn;
            }

            public void Destroy()
            {
                obj.Off(ev, fn);
            }
        }

        public class TimeoutActionHandle : IHandle
        {
            private Action fn;

            public TimeoutActionHandle(Action fn)
            {
                this.fn = fn;
            }

            // Not sure why Destroy invokes instead of nullifying
            public void Destroy()
            {
                fn();
            }
        }

        public interface IHandle
        {
            void Destroy();
        }

    }
}
