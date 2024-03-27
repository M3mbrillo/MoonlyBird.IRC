using MoonlyBird.IRC.Core.Abstract.Message.CommandParameter;

namespace MoonlyBird.IRC.Core.Abstract.Message;

public interface IRawServerMessage
{
    public IrcCommand IrcCommand { get; init; }
    public string Prefix { get; init; }

    MessageParameter GetMessageParameter();
}