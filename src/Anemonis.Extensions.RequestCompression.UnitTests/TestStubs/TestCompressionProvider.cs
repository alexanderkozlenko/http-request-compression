using System.IO.Compression;

namespace Anemonis.Extensions.RequestCompression.UnitTests.TestStubs;

internal sealed class TestCompressionProvider : IRequestCompressionProvider
{
    public Stream CreateStream(Stream outputStream, CompressionLevel compressionLevel)
    {
        var inputStream = new TestDelegatingStream(outputStream, leaveOpen: true);

        using var streamWriter = new StreamWriter(inputStream);

        streamWriter.Write(EncodingName);
        streamWriter.Flush();

        return inputStream;
    }

    public string EncodingName
    {
        get
        {
            return "e1";
        }
    }
}
