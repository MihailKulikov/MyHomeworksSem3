using System;
using System.Runtime.Serialization;

namespace ThreadPoolRealisation
{
    public class MyThreadPoolShutdownedException : Exception
    {
        public MyThreadPoolShutdownedException()
        {
        }

        public MyThreadPoolShutdownedException(string message) : base(message)
        {
        }

        public MyThreadPoolShutdownedException(string message, Exception inner) : base(message, inner)
        {
        }

        protected MyThreadPoolShutdownedException(SerializationInfo info, StreamingContext context) : base(info,
            context)
        {
        }
    }
}