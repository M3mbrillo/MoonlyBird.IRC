using MoonlyBird.IRC.Core.Abstract;
using MoonlyBird.IRC.Core.Abstract.Message;
using MoonlyBird.IRC.Core.Abstract.Message.CommandParameter;

namespace MoonlyBird.IRC.Core.Message;

public class RawServerMessage : IRawServerMessage
{
    private readonly string _message;
    private string RawCommand { get; init; }
    public IrcCommand IrcCommand { get; init; }
    public string Prefix { get; init; } = string.Empty;
    
    public MessageParameter GetMessageParameter() => new MessageParameter(this.RawCommand);
    

    public RawServerMessage(string message)
    {
        _message = message;
        if (message.StartsWith(':'))
        {
            var indexDelimiter = message.IndexOf((char)0x20);
            Prefix = message[1..indexDelimiter];
            message = message[++indexDelimiter..];
        }
        
        var endCommandIndex = message.IndexOf((char)0x20);
        IrcCommand = DetectCommand(message[..endCommandIndex]);

        RawCommand = message;
    }


    private IrcCommand DetectCommand(string commandString)
    {
        var success = Enum.TryParse(commandString, out IrcCommand command);
        if (!success)
            command = IrcCommand._;
        
        return command;
    }

}