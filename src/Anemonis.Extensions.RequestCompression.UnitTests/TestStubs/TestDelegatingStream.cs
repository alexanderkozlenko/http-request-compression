namespace Anemonis.Extensions.RequestCompression.UnitTests.TestStubs;

internal sealed class TestDelegatingStream : Stream
{
    private readonly Stream _innerStream;
    private readonly bool _leaveOpen;

    public TestDelegatingStream(Stream innerStream, bool leaveOpen)
    {
        _innerStream = innerStream;
        _leaveOpen = leaveOpen;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (!_leaveOpen)
            {
                _innerStream.Dispose();
            }
        }

        base.Dispose(disposing);
    }

    public override ValueTask DisposeAsync()
    {
        if (!_leaveOpen)
        {
            return _innerStream.DisposeAsync();
        }
        else
        {
            return ValueTask.CompletedTask;
        }
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return _innerStream.Seek(offset, origin);
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return _innerStream.Read(buffer, offset, count);
    }

    public override int Read(Span<byte> buffer)
    {
        return _innerStream.Read(buffer);
    }

    public override int ReadByte()
    {
        return _innerStream.ReadByte();
    }

    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        return _innerStream.ReadAsync(buffer, offset, count, cancellationToken);
    }

    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        return _innerStream.ReadAsync(buffer, cancellationToken);
    }

    public override void Flush()
    {
        _innerStream.Flush();
    }

    public override Task FlushAsync(CancellationToken cancellationToken)
    {
        return _innerStream.FlushAsync(cancellationToken);
    }

    public override void SetLength(long value)
    {
        _innerStream.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        _innerStream.Write(buffer, offset, count);
    }

    public override void Write(ReadOnlySpan<byte> buffer)
    {
        _innerStream.Write(buffer);
    }

    public override void WriteByte(byte value)
    {
        _innerStream.WriteByte(value);
    }

    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        return _innerStream.WriteAsync(buffer, offset, count, cancellationToken);
    }

    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        return _innerStream.WriteAsync(buffer, cancellationToken);
    }

    public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
    {
        return _innerStream.CopyToAsync(destination, bufferSize, cancellationToken);
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
