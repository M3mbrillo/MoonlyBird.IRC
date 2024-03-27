using System.Text;
using MoonlyBird.Common.Extensions.String;

namespace MoonlyBird.IRC.Core.Message.Builder;

public static class PongMessage
{
    private const string Command = "PONG";

    public static StringBuilder Build(string server1, string server2)
    {
        var messageBuilder = new StringBuilder();

        messageBuilder
            .Append(Command)
            .Append(' ')
            .Append(server1);

        if (!server2.IsNullOrEmpty())
            messageBuilder
                .Append(' ')
                .Append(server2);

        return messageBuilder;
    }
}