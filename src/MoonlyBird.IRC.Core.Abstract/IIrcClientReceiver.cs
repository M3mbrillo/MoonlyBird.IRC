using MoonlyBird.IRC.Core.Abstract.Attribute;
using MoonlyBird.IRC.Core.Abstract.Handler;
using MoonlyBird.IRC.Core.Abstract.Message.CommandParameter;

namespace MoonlyBird.IRC.Core.Abstract;

public interface IIrcClientReceiver
{
    /// <summary>
    ///     Dispatch when a message is received from IRC Server
    /// </summary>
    event EventHandler<string> OnMessageReceived;
    
    /// <summary>
    ///     Dispatch when a command PING is received
    /// </summary>
    [IrcCommand(IrcCommand.PING)]
    event IrcCommandEventHandlerAsync<PingMessageParameter> OnCommandPing;

    /// <summary>
    ///     Dispatch when a command PING is received
    /// </summary>
    [IrcCommand(IrcCommand.NOTICE)]
    event IrcCommandEventHandlerAsync<NoticeMessageParameter> OnCommandNotice;

    
    void RegisterDefaultCommandHandlers();
}