namespace TuiChat.Views;
using Terminal.Gui;
using TuiAgent.Services;
using TuiChat.Services;

public class ChatView : Terminal.Gui.Window
{
    private readonly IChatService chatService;
    private TextView chatHistoryView;
    private TextField messageInput;
    private Button sendButton;
    private Button clearButton;
    private Button saveButton;
    private StatusBar statusBar;
    private Label statusLabel;
    private bool isProcessing = false;

    public ChatView()
    {
        chatService = new ChatService();
        InitializeComponent();
        SetupEventHandlers();
        InitializeAI();
    }

    private void InitializeComponent()
    {
        // Window setup
        this.Width = Dim.Fill(0);
        this.Height = Dim.Fill(0);
        this.X = 0;
        this.Y = 0;
        this.Modal = false;
        this.Border.BorderStyle = Terminal.Gui.BorderStyle.Single;
        this.Border.Effect3D = true;
        this.Title = "AI Chat - Terminal UI (Ctrl+Q to quit)";

        // Chat history view (scrollable text area)
        chatHistoryView = new TextView
        {
            X = 1,
            Y = 1,
            Width = Dim.Fill(1),
            Height = Dim.Fill(5),
            ReadOnly = true,
            WordWrap = true,
            Text = "Welcome to AI Chat!\nInitializing AI model...\n\n"
        };
        this.Add(chatHistoryView);

        // Status label
        statusLabel = new Label
        {
            X = 1,
            Y = Pos.Bottom(chatHistoryView),
            Width = Dim.Fill(1),
            Height = 1,
            Text = "Status: Initializing...",
            ColorScheme = Colors.ColorSchemes["Dialog"]
        };
        this.Add(statusLabel);

        // Message input field
        messageInput = new TextField
        {
            X = 1,
            Y = Pos.Bottom(statusLabel) + 1,
            Width = Dim.Fill(1),
            Height = 1
        };
        
        var inputLabel = new Label
        {
            X = 1,
            Y = Pos.Bottom(statusLabel),
            Text = "You: "
        };
        this.Add(inputLabel);
        this.Add(messageInput);

        // Button panel
        var buttonY = Pos.Bottom(messageInput) + 1;

        sendButton = new Button
        {
            X = 1,
            Y = buttonY,
            Text = "Send (Enter)",
            IsDefault = true
        };
        this.Add(sendButton);

        clearButton = new Button
        {
            X = Pos.Right(sendButton) + 2,
            Y = buttonY,
            Text = "Clear History"
        };
        this.Add(clearButton);

        saveButton = new Button
        {
            X = Pos.Right(clearButton) + 2,
            Y = buttonY,
            Text = "Save Chat"
        };
        this.Add(saveButton);

        // Status bar at bottom
        statusBar = new StatusBar(new StatusItem[]
        {
            new StatusItem(Key.F1, "~F1~ Help", () => ShowHelp()),
            new StatusItem(Key.F2, "~F2~ Clear", () => ClearHistory()),
            new StatusItem(Key.F3, "~F3~ Save", () => SaveChat()),
            new StatusItem(Key.CtrlMask | Key.Q, "~^Q~ Quit", () => Application.RequestStop())
        });
        this.Add(statusBar);
    }

    private void SetupEventHandlers()
    {
        sendButton.Clicked += async () => await SendMessageAsync();
        clearButton.Clicked += ClearHistory;
        saveButton.Clicked += SaveChat;

        // Handle Enter key in message input
        messageInput.KeyPress += (e) =>
        {
            if (e.KeyEvent.Key == Key.Enter)
            {
                e.Handled = true;
                _ = SendMessageAsync();
            }
        };
    }

    private async void InitializeAI()
    {
        try
        {
            var status = await chatService.GetInitializationStatusAsync();
            AppendToChatHistory($"[SYSTEM] {status}\n\n");
            UpdateStatus(chatService.IsInitialized ? "Ready" : "Failed to initialize");

            if (chatService.IsInitialized)
            {
                AppendToChatHistory("You can now start chatting! Type your message and press Enter or click Send.\n\n");
                messageInput.SetFocus();
            }
        }
        catch (Exception ex)
        {
            AppendToChatHistory($"[ERROR] Failed to initialize: {ex.Message}\n\n");
            UpdateStatus("Initialization failed");
        }
    }

    private async Task SendMessageAsync()
    {
        if (isProcessing)
        {
            return;
        }

        var message = messageInput.Text?.ToString()?.Trim();
        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        if (!chatService.IsInitialized)
        {
            MessageBox.ErrorQuery("Error", "AI is not initialized yet. Please wait.", "OK");
            return;
        }

        isProcessing = true;
        sendButton.Enabled = false;
        messageInput.Enabled = false;
        UpdateStatus("Processing...");

        try
        {
            // Display user message
            AppendToChatHistory($"[You - {DateTime.Now:HH:mm:ss}]\n{message}\n\n");
            messageInput.Text = "";

            // Display AI response header
            AppendToChatHistory($"[AI - {DateTime.Now:HH:mm:ss}]\n");

            // Stream AI response
            await foreach (var chunk in chatService.SendMessageStreamingAsync(message))
            {
                AppendToChatHistory(chunk);
                Application.Refresh();
            }

            AppendToChatHistory("\n\n");
            UpdateStatus($"Ready - {chatService.GetConversationSummary()}");
        }
        catch (Exception ex)
        {
            AppendToChatHistory($"\n[ERROR] {ex.Message}\n\n");
            UpdateStatus("Error occurred");
        }
        finally
        {
            isProcessing = false;
            sendButton.Enabled = true;
            messageInput.Enabled = true;
            messageInput.SetFocus();
        }
    }

    private void AppendToChatHistory(string text)
    {
        Application.MainLoop.Invoke(() =>
        {
            chatHistoryView.Text += text;
            // Auto-scroll to bottom
            chatHistoryView.MoveEnd();
        });
    }

    private void UpdateStatus(string status)
    {
        Application.MainLoop.Invoke(() =>
        {
            statusLabel.Text = $"Status: {status}";
        });
    }

    private void ClearHistory()
    {
        var result = MessageBox.Query("Clear History",
          "Are you sure you want to clear the chat history?", "Yes", "No");

        if (result == 0)
        {
            chatService.ClearHistory();
            chatHistoryView.Text = "Chat history cleared.\n\n";
            UpdateStatus("Ready - History cleared");
        }
    }

    private void SaveChat()
    {
        if (chatService.ConversationHistory.Count == 0)
        {
            MessageBox.ErrorQuery("Save Chat", "No conversation to save.", "OK");
            return;
        }

        var fileName = $"chat_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
        var saveDialog = new SaveDialog("Save Conversation", "Save chat history to file")
        {
            FilePath = fileName
        };

        Application.Run(saveDialog);

        if (!saveDialog.Canceled && !string.IsNullOrEmpty(saveDialog.FilePath?.ToString()))
        {
            var filePath = saveDialog.FilePath.ToString();
            _ = SaveChatToFileAsync(filePath);
        }
    }

    private async Task SaveChatToFileAsync(string filePath)
    {
        try
        {
            var success = await chatService.SaveConversationAsync(filePath);
            if (success)
            {
                MessageBox.Query("Success", $"Conversation saved to:\n{filePath}", "OK");
                UpdateStatus($"Saved to {Path.GetFileName(filePath)}");
            }
            else
            {
                MessageBox.ErrorQuery("Error",
          $"Failed to save conversation.\n{chatService.LastError}", "OK");
            }
        }
        catch (Exception ex)
        {
            MessageBox.ErrorQuery("Error", $"Failed to save: {ex.Message}", "OK");
        }
    }

    private void ShowHelp()
    {
        var helpText = @"AI Chat - Help

Commands:
• Type your message and press Enter to send
• F1 - Show this help
• F2 - Clear chat history
• F3 - Save conversation to file
• Ctrl+Q - Quit application

Features:
• Real-time streaming AI responses
• Conversation history tracking
• Save conversations to text files
• Keyboard shortcuts for quick access

Model: phi-3.5-mini
Framework: Terminal.Gui + Microsoft AI Foundry";

        MessageBox.Query("Help", helpText, "OK");
    }
}
