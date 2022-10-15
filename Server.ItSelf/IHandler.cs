namespace Server.ItSelf;

public interface IHandler
{
    Task HandleAsync(Stream networkStream, Request request);
}