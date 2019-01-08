using System;
using System.IO;
using System.Text;

namespace ChunkingRedisClient.Client.Writer
{
    public class BufferedTextWriter : TextWriter
    {
        private readonly int _bufferSize;

        public BufferedTextWriter(int bufferSize)
        {
            _bufferSize = bufferSize;
            Buffer = new StringBuilder(_bufferSize);
        }

        public event EventHandler<BufferFullEventArgs> BufferFull;

        public override Encoding Encoding => Encoding.UTF8;

        public StringBuilder Buffer { get; }

        public override void Write(char value)
        {
            if (Buffer.Length == _bufferSize)
            {
                throw new InvalidOperationException("Buffer is full!");
            }

            Buffer.Append(value);

            if (Buffer.Length == _bufferSize)
            {
                OnBufferFull(new BufferFullEventArgs(Buffer));
            }
        }

        protected virtual void OnBufferFull(BufferFullEventArgs e)
        {
            BufferFull?.Invoke(this, e);
        }

        public class BufferFullEventArgs : EventArgs
        {
            public BufferFullEventArgs(StringBuilder buffer)
            {
                Buffer = buffer;
            }

            public StringBuilder Buffer { get; }
        }
    }
}