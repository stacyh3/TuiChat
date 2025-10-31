# TuiChat - Full-Featured AI Chat Application

A terminal-based AI chat application built with Terminal.Gui and Microsoft AI Foundry Local, featuring the phi-3.5-mini model.

## Features

### 🎯 Core Features
- **Real-time streaming AI responses** - See AI responses as they're generated
- **Conversation history management** - All messages are tracked with timestamps
- **Intuitive terminal UI** - Clean, easy-to-use interface with Terminal.Gui
- **Local AI model** - Runs phi-3.5-mini locally using Microsoft AI Foundry

### 💬 Chat Features
- **Message input** - Type your messages with Enter key support
- **Scrollable chat history** - Review previous conversations
- **Timestamped messages** - Each message shows when it was sent
- **Status indicators** - Real-time status updates (Initializing, Ready, Processing)
- **Error handling** - Graceful error messages for initialization and runtime issues

### 🛠️ Advanced Features
- **Save conversations** - Export chat history to text files with F3 or Save button
- **Clear history** - Remove all messages with confirmation dialog (F2)
- **Conversation summary** - View message counts in status bar
- **Keyboard shortcuts** - Quick access to all features
- **Help system** - Built-in help dialog (F1)

## Keyboard Shortcuts

| Shortcut | Action |
|----------|--------|
| **Enter** | Send message |
| **F1** | Show help dialog |
| **F2** | Clear chat history |
| **F3** | Save conversation to file |
| **Ctrl+Q** | Quit application |

## UI Components

### Chat Window Layout
```
┌─ AI Chat - Terminal UI (Ctrl+Q to quit) ─────────────┐
│                                                      │
│  [Chat History - Scrollable Text View]               │
│  - Displays all messages                             │
│  - Auto-scrolls to latest messages                   │
│  - Shows user and AI messages with timestamps        │
│                                                      │
│                                                      │
├──────────────────────────────────────────────────────┤
│  Status: Ready - Messages: 10 (User: 5, AI: 5)       │
├──────────────────────────────────────────────────────┤
│  You: [Message Input Field]                          │
├──────────────────────────────────────────────────────┤
│  [Send (Enter)]  [Clear History]  [Save Chat]        │
├──────────────────────────────────────────────────────┤
│  F1 Help  F2 Clear  F3 Save  ^Q Quit                 │
└──────────────────────────────────────────────────────┘
```

## Architecture

### Services
- **IChatService** - Interface defining chat functionality
- **ChatService** - Main service implementation
  - Manages AI agent initialization
  - Handles streaming responses
  - Tracks conversation history
  - Provides save/clear operations
  - Error handling and status reporting

### Models
- **ChatMessage** - Represents a single message
  - Role (User, Assistant, System)
  - Content (message text)
  - Timestamp

### Views
- **ChatView** - Main UI window
  - Chat history display
  - Message input
  - Action buttons
  - Status bar
  - Help system

## Usage

### Starting the Application
```bash
dotnet run
```

The application will:
1. Initialize the Terminal UI
2. Start the phi-3.5-mini AI model
3. Display initialization status
4. Enable message input when ready

### Sending Messages
1. Type your message in the input field
2. Press **Enter** or click the **Send** button
3. Watch the AI response stream in real-time
4. Continue the conversation naturally

### Saving Conversations
1. Press **F3** or click **Save Chat**
2. Choose a location and filename
3. The conversation is saved with timestamps and roles

### Sample Saved Conversation Format
```
[2024-01-15 14:30:22] User: Hello, how are you?
[2024-01-15 14:30:25] Assistant: Hello! I'm doing well, thank you for asking. How can I help you today?
[2024-01-15 14:30:45] User: Can you explain what you can do?
[2024-01-15 14:30:50] Assistant: I'm an AI assistant powered by the phi-3.5-mini model...
```

## Technical Details

### Dependencies
- **.NET 9.0** - Latest .NET runtime
- **Terminal.Gui** - Cross-platform terminal UI framework
- **Microsoft.AI.Foundry.Local** - Local AI model hosting
- **Microsoft.Agents.AI** - AI agent abstractions
- **Azure.AI.OpenAI** - OpenAI-compatible client
- **Microsoft.Extensions.Hosting** - Hosting infrastructure

### AI Model
- **Model**: phi-3.5-mini
- **Hosting**: Microsoft AI Foundry Local
- **Interface**: OpenAI-compatible API
- **Streaming**: Real-time token streaming support

### Error Handling
- Graceful initialization failures
- Runtime error messages in chat
- Status bar error indicators
- User-friendly error dialogs

## Future Enhancements

Potential features to add:
- [ ] Multiple conversation threads/tabs
- [ ] Model selection dropdown
- [ ] System prompt customization
- [ ] Token usage statistics
- [ ] Export to different formats (JSON, Markdown)
- [ ] Search within conversation history
- [ ] Dark/Light theme toggle
- [ ] Custom keyboard shortcuts
- [ ] Response regeneration
- [ ] Message editing
- [ ] Copy/paste support for messages

## Troubleshooting

### AI Model Not Initializing
- Check internet connection (first-time model download)
- Ensure sufficient disk space for model
- Verify Microsoft.AI.Foundry.Local is properly installed

### Terminal Display Issues
- Ensure terminal supports ANSI colors
- Try resizing terminal window
- Check Terminal.Gui compatibility with your terminal

### Performance Issues
- First response may be slower (model loading)
- Subsequent responses should be faster
- Consider system resources (RAM, CPU)

## License

This project uses:
- Terminal.Gui (MIT License)
- Microsoft AI components (Microsoft License)
- phi-3.5-mini model (Microsoft License)

---

Built with ❤️ using Terminal.Gui and Microsoft AI Foundry
