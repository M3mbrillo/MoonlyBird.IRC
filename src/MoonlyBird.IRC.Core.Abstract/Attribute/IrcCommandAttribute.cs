namespace MoonlyBird.IRC.Core.Abstract.Attribute;

[System.AttributeUsage(System.AttributeTargets.Event)]
public class IrcCommandAttribute(IrcCommand ircCommand) : System.Attribute
{
    public IrcCommand IrcCommand { get; } = ircCommand;
}