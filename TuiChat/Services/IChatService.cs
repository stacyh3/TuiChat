namespace TuiAgent.Services;

public interface IChatService
{
    Task<string> GetInitializationStatusAsync();
    IAsyncEnumerable<string> SendMessageStreamingAsync(string message);
    bool IsInitialized { get; }
}
