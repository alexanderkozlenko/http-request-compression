using System.IO.Compression;
using System.Text;

namespace Anemonis.Extensions.RequestCompression.UnitTests;

public partial class HttpCompressionHandlerTests
{
    private sealed class FakeMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> _handler;

        public FakeMessageHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> handler)
        {
            _handler = handler;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return _handler.Invoke(request);
        }
    }

    private sealed class FakeCompressionProvider : HttpCompressionProvider
    {
        private readonly byte[] _marker;

        public FakeCompressionProvider(string contentEncoding)
        {
            _marker = Encoding.ASCII.GetBytes($"+{contentEncoding}");
        }

        public override Stream CreateStream(Stream outputStreeam, CompressionLevel compressionLevel)
        {
            return new FakeCompressionStream(outputStreeam, _marker);
        }
    }

    private sealed class FakeCompressionStream : Stream
    {
        private readonly Stream _innerStream;
        private readonly byte[] _marker;

        public FakeCompressionStream(Stream innerStream, byte[] marker)
        {
            _innerStream = innerStream;
            _marker = marker;
        }

        public override void Close()
        {
            _innerStream.Write(_marker, 0, _marker.Length);
            _innerStream.Flush();

            base.Close();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _innerStream.Seek(offset, origin);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _innerStream.Read(buffer, offset, count);
        }

        public override void Flush()
        {
            _innerStream.Flush();
        }

        public override void SetLength(long value)
        {
            _innerStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _innerStream.Write(buffer, offset, count);

        }

        public override bool CanRead
        {
            get
            {
                return _innerStream.CanRead;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return _innerStream.CanSeek;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return _innerStream.CanWrite;
            }
        }

        public override long Length
        {
            get
            {
                return _innerStream.Length;
            }
        }

        public override long Position
        {
            get
            {
                return _innerStream.Position;
            }
            set
            {
                _innerStream.Position = value;
            }
        }
    }
}
