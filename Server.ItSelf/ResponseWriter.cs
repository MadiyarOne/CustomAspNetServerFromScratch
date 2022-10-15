using System.Net;

namespace Server.ItSelf;

static class ResponseWriter
{
    public static async Task WriteStatus(HttpStatusCode code, Stream stream)
    {
        await using var writer = new StreamWriter(stream, leaveOpen: true);
        await writer.WriteLineAsync($"HTTP/1.0 {(int)code} {code}");
        await writer.WriteLineAsync();
    }
}