namespace Server.ItSelf;

static class RequestParser
{
    public static Request Parse(string? header)
    {
        if (header == null) throw new FormatException();
        var headerStrs = header.Split();
        return new Request(headerStrs[1], GetHttpMethod(headerStrs[0]));

    }

    private static HttpMethod GetHttpMethod(string httpMethodStr)
    {
        return httpMethodStr switch
        {
            "GET" => HttpMethod.Get,
            "POST" => HttpMethod.Post,
            "PUT" => HttpMethod.Put,
            "DELETE" => HttpMethod.Delete,
            _ => throw new NotImplementedException()
        };
    }
}