namespace LabWebApplication;

public class Response<T>
{
    public Response(T value, IDictionary<string, string> links)
    {
        Value = value;
        Links = links;
    }

    public T Value { get; set; }
    public IDictionary<string, string> Links { get; set; }

}