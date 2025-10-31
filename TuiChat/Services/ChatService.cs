using TuiAgent.Services;

namespace TuiChat.Services;

public class ChatService : IChatService
{
    private readonly string modelId = "phi-3.5-mini";
    private FoundryLocalManager? manager;
    private AIAgent? agent;
    private bool isInitialized = false;

    public bool IsInitialized => isInitialized;

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
            
            return "AI Model initialized successfully!";
        }
        catch (Exception ex)
        {
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

        await foreach (var update in agent.RunStreamingAsync(message))
        {
            if (!string.IsNullOrEmpty(update.Text))
            {
                yield return update.Text;
            }
        }
    }
}
