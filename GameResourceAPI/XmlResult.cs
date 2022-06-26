namespace GameResourceAPI.Data
{
    public class XmlResult<T> : IResult
    {
        private static readonly XmlSerializer _serializer = new XmlSerializer(typeof(T));
        private readonly T _result;
        public XmlResult(T result)
        {
            _result = result;
        }

        public Task ExecuteAsync(HttpContext httpContext)
        {
            using var ms = new MemoryStream();
            _serializer.Serialize(ms, _result);
            httpContext.Response.ContentType = "application/xml";
            ms.Position = 0;
            return ms.CopyToAsync(httpContext.Response.Body);
        }
    }

    static class XmlResultsExtensions //расширяем возможности Results в запросах контроллера
    {
        public static IResult Xml<T>(this IResultExtensions _, T result) => 
            new XmlResult<T>(result);
    }
}
