using System.Net;
using System.Reflection;
using Newtonsoft.Json;
using System;

namespace Server.ItSelf;

public class ControllersHandler : IHandler
{
    private readonly Assembly _controllersAssembly;
    private Dictionary<string, Func<object?>> _routes;

    public ControllersHandler(Assembly controllersAssembly)
    {
        _controllersAssembly = controllersAssembly;
        _routes =_controllersAssembly
            .GetTypes()
            .Where(x => typeof(IController).IsAssignableFrom(x))
            .SelectMany(controller => controller
                .GetMethods()
                .Select(method => new {Controller = controller, Method = method}))
            .ToDictionary(
                key => GetPath(key.Controller, key.Method), 
                value => GetEndPointMethod(value.Controller, value.Method));
        Console.WriteLine("daw");
    }

    private string GetPath(Type controller, MethodInfo method)
    {
        var name = controller.Name.ToLower();
        const string postFix = "controller";
        if (name.EndsWith(postFix, StringComparison.InvariantCultureIgnoreCase)) 
            name = name[..^postFix.Length];
        if (method.Name.Equals("Index", StringComparison.InvariantCultureIgnoreCase))
            return "/" + name;
        return "/" + name + "/" + method.Name;
    }
    
    private Func<object?> GetEndPointMethod(Type controller, MethodInfo method)
    {
        return () => method.Invoke(Activator.CreateInstance(controller), Array.Empty<object>());
    }

    public async Task HandleAsync(Stream networkStream, Request request)
    {
        if (!_routes.TryGetValue(request.Path, out var func))
        {
            await ResponseWriter.WriteStatus(HttpStatusCode.NotFound, networkStream);
        }
        else
        {
            await ResponseWriter.WriteStatus(HttpStatusCode.OK, networkStream);
            await WriteControllerResponse(func.Invoke(), networkStream);
        }
    }

    private async Task WriteControllerResponse(object? response, Stream stream)
    {
        switch (response)
        {
            case string str:
            {
                await using var writer = new StreamWriter(stream, leaveOpen:true);
                await writer.WriteLineAsync(str);
                break;
            }
            case byte[] buffer:
                await stream.WriteAsync(buffer);
                break;
            case Task task:
                await task;
                await WriteControllerResponse(task.GetType().GetProperty("Result")?.GetValue(task), stream);
                break;
            default:
                await WriteControllerResponse(JsonConvert.SerializeObject(response), stream);
                break;
        }
    }
}