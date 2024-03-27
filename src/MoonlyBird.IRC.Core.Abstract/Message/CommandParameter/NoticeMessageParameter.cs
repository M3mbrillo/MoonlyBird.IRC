namespace MoonlyBird.IRC.Core.Abstract.Message.CommandParameter;

public record NoticeMessageParameter : MessageParameter
{
    public NoticeMessageParameter(MessageParameter message) : base(message.RawMessage)
    {
        Message = RawMessage;
    }

    public string Message { get; set; }
}