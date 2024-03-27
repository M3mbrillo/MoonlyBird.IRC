using System.Text;

namespace MoonlyBird.IRC.Core.Abstract;

public interface IIrcClientSender
{
    /// <summary>
    ///     Dispatch before at a message is writer into StreamWriter 
    /// </summary>
    event EventHandler<string> OnMessageSending;
    
    /// <summary>
    ///     Dispatch after at a message is writer into StreamWriter
    /// </summary>
    event EventHandler<string> OnMessageSent;
    
    Task SendMessageAsync(string message, CancellationToken cancellationToken);
    Task SendMessageAsync(StringBuilder messageBuilder, CancellationToken cancellationToken);
}