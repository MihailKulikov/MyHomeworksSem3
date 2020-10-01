using System.Collections.Generic;
using System.Threading;

namespace ThreadPoolRealisation
{
    public class SynchronizedQueue<T>
    {
        private readonly Queue<T> buffer = new Queue<T>();

        public void Enqueue(T item)
        {
            lock (buffer)
            {
                buffer.Enqueue(item);
                Monitor.Pulse(buffer);
            }
        }

        public T Dequeue()
        {
            lock (buffer)
            {
                while (buffer.Count == 0)
                {
                    Monitor.Wait(buffer);
                }

                var result = buffer.Dequeue();
                Monitor.Pulse(buffer);
                return result;
            }
        }
    }
}