namespace MoonlyBird.IRC.Core.Abstract.Message.CommandParameter;

/// <summary>
/// 3.7.2 Ping message
/// https://datatracker.ietf.org/doc/html/rfc2812#section-3.7.2
/// </summary>
public sealed record PingMessageParameter : MessageParameter
{
    public string Server1 { get; init; }
    public string Server2 { get; init; } = string.Empty;

    public PingMessageParameter(MessageParameter messageParameter) : base(messageParameter.RawMessage)
    {
        var servers = RawMessage[("PING :".Length)..].Split(" ");
        Server1 = servers[0];
        if (servers.Length > 1)
        {
            Server2 = servers[1];
        }
    }
}