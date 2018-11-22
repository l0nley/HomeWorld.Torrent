using System;
using System.IO;

namespace HomeWorld.Torrent
{
    internal class ReadonlyChunkStream : Stream
    {
        private readonly int _max;
        private readonly Stream _bs;
        private int _cur;

        public ReadonlyChunkStream(Stream bs, int max)
        {
            if (max < 0)
            {
                throw new ArgumentException(nameof(max));
            }
            _bs = bs ?? throw new ArgumentNullException(nameof(bs));
            _max = max;
        }

        public override bool CanRead => _bs.CanRead;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => _bs.Length;

        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var canRead = _max - _cur;
            if(count > canRead)
            {
                count = canRead;
            }
            var readed = _bs.Read(buffer, offset, count);
            _cur += readed;
            return readed;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}
