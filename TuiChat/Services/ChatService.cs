using TuiAgent.Services;

namespace TuiChat.Services;

public class ChatService : IChatService
{
    private readonly string modelId = "phi-3.5-mini";
    private FoundryLocalManager? manager;
    private AIAgent? agent;
    private bool isInitialized = false;
    private readonly List<ChatMessage> conversationHistory = new();
    private string? lastError;

    public bool IsInitialized => isInitialized;
    public IReadOnlyList<ChatMessage> ConversationHistory => conversationHistory.AsReadOnly();
    public string? LastError => lastError;

    public async Task<string> GetInitializationStatusAsync()
    {
        if (isInitialized)
        {
            return "Ready";
        }

        try
        {
            manager = await FoundryLocalManager.StartModelAsync(modelId);
            var modelInfo = await manager.GetModelInfoAsync(modelId);

            var client = new OpenAIClient(new ApiKeyCredential("unused"), new OpenAIClientOptions
            {
                Endpoint = manager.Endpoint
            });

            agent = client.GetChatClient(modelInfo!.ModelId).CreateAIAgent();
            isInitialized = true;
            lastError = null;
            
            return "AI Model initialized successfully!";
        }
        catch (Exception ex)
        {
            lastError = ex.Message;
            return $"Error initializing AI: {ex.Message}";
        }
    }

    public async IAsyncEnumerable<string> SendMessageStreamingAsync(string message)
    {
        if (!isInitialized || agent == null)
        {
            yield return "Error: AI not initialized";
            yield break;
        }

        // Add user message to history
        conversationHistory.Add(new ChatMessage
        {
            Role = "User",
            Content = message,
            Timestamp = DateTime.Now
        });

        var fullResponse = new StringBuilder();

        // Stream responses without try-catch (to avoid yield in try-catch)
        await foreach (var update in agent.RunStreamingAsync(message).ConfigureAwait(false))
        {
            if (!string.IsNullOrEmpty(update.Text))
            {
                fullResponse.Append(update.Text);
                yield return update.Text;
            }
        }

        // Add assistant response to history
        if (fullResponse.Length > 0)
        {
            conversationHistory.Add(new ChatMessage
            {
                Role = "Assistant",
                Content = fullResponse.ToString(),
                Timestamp = DateTime.Now
            });
        }
    }

    public void ClearHistory()
    {
        conversationHistory.Clear();
    }

    public async Task<bool> SaveConversationAsync(string filePath)
    {
        try
        {
            var lines = conversationHistory.Select(m => 
                $"[{m.Timestamp:yyyy-MM-dd HH:mm:ss}] {m.Role}: {m.Content}");
            await File.WriteAllLinesAsync(filePath, lines);
            return true;
        }
        catch (Exception ex)
        {
            lastError = ex.Message;
            return false;
        }
    }

    public string GetConversationSummary()
    {
        var userMessages = conversationHistory.Count(m => m.Role == "User");
        var assistantMessages = conversationHistory.Count(m => m.Role == "Assistant");
        return $"Messages: {conversationHistory.Count} (User: {userMessages}, AI: {assistantMessages})";
    }
}

public class ChatMessage
{
    public required string Role { get; init; }
    public required string Content { get; init; }
    public DateTime Timestamp { get; init; }
}
