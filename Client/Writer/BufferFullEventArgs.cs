using System;
using System.Text;

namespace JsonRedis.Client.Writer
{
    public class BufferFullEventArgs : EventArgs
    {
        public BufferFullEventArgs(StringBuilder buffer)
        {
            Buffer = buffer;
        }

        public StringBuilder Buffer { get; }
    }
}