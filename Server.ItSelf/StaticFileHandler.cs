using System.Net;

namespace Server.ItSelf;

public class StaticFileHandler : IHandler
{
    private readonly string _path;

    public StaticFileHandler(string path)
    {
        _path = path;
    }
    
    public async Task HandleAsync(Stream networkStream, Request request)
    {
        await using var writer = new StreamWriter(networkStream);

        var filePath = Path.Combine(_path, request.Path[1..request.Path.Length]);
        if (!File.Exists(filePath))
        {
            await ResponseWriter.WriteStatus(HttpStatusCode.NotFound, networkStream);
        }
        else
        {
            await using var fileStream = File.OpenRead(filePath);
            await fileStream.CopyToAsync(networkStream);
            await ResponseWriter.WriteStatus(HttpStatusCode.OK, networkStream);
        }

        Console.WriteLine(filePath);
        
    }
}