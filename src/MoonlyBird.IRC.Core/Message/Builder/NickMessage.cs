using System.Text;

namespace MoonlyBird.IRC.Core.Message.Builder;

public static class NickMessage
{
    private const string Command = "NICK";
    
    public static StringBuilder Build(string nickname)
    {
        var messageBuilder = new StringBuilder();

        messageBuilder
            .Append(Command)
            .Append(' ')
            .Append(nickname);
        
        return messageBuilder;
    }
}