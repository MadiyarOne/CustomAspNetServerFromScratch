using System.Net;
using System.Net.Sockets;

namespace Server.ItSelf;

public class ServerHost 
{
    private readonly IHandler _handler;

    public ServerHost(IHandler handler)
    {
        _handler = handler;
    }

    public async Task Start()
    {
        var listener = new TcpListener(IPAddress.Any, 80);
        listener.Start();
        while (true)
        {
                var client = await listener.AcceptTcpClientAsync();
                ProcessClientAsync(client);
        }
    }

    private async Task ProcessClientAsync(TcpClient client)
    {
        try
        {
            await using var stream = client.GetStream();
            using var reader = new StreamReader(stream);
            var header = await reader.ReadLineAsync();
            for (string? line = null; line != string.Empty; line = await reader.ReadLineAsync())
                ;
            var request = RequestParser.Parse(header);
            await _handler.HandleAsync(stream, request);
            ;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}