namespace Core.Shared;

public class Networking
{
    public string ApiEndpoint { get; init; } = string.Empty;
    public bool NoDelay { get; init; }
    public int MaxBacklog { get; init; }
    public int BufferSize { get; init; }

    public override string ToString()
    {
        return $"EndPoint: {ApiEndpoint}, NoDelay: {NoDelay}, MaxBacklog: {MaxBacklog}, BufferSize: {BufferSize}";
    }
}