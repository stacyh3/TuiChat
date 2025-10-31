using TuiChat.Views;


Application.Init();

try
{
    Application.Run(new ChatView());
}
finally
{
    Application.Shutdown();
}