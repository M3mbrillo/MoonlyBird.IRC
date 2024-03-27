using System.Text;

namespace MoonlyBird.IRC.Core.Message.Builder;

public static class UserMessage
{
    private const string Command = "USER";
    
    public static StringBuilder Build(string nickname, string realName, string mode)
    {
        var messageBuilder = new StringBuilder();

        messageBuilder
            .Append(Command)
            .Append(' ')
            .Append(nickname)
            .Append(" ").Append(mode)
            .Append(" *")
            .Append(" :").Append(realName);
        
        return messageBuilder;
    }
}