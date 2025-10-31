using TuiChat.Services;

namespace TuiAgent.Services;

public interface IChatService
{
    Task<string> GetInitializationStatusAsync();
    IAsyncEnumerable<string> SendMessageStreamingAsync(string message);
    bool IsInitialized { get; }
    IReadOnlyList<ChatMessage> ConversationHistory { get; }
    string? LastError { get; }
    void ClearHistory();
    Task<bool> SaveConversationAsync(string filePath);
    string GetConversationSummary();
}
