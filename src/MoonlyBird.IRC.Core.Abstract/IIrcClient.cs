using System.Text;
using MoonlyBird.IRC.Core.Abstract.Attribute;
using MoonlyBird.IRC.Core.Abstract.Message.CommandParameter;

namespace MoonlyBird.IRC.Core.Abstract;

public interface IIrcClient : 
    IIrcClientReceiver,
    IIrcClientSender
{
    /// <summary>
    ///     Dispatch when a Exception is detected
    /// </summary>
    event EventHandler<Exception> OnException;
    
    Task StartAsync(CancellationToken cancellationToken);
}