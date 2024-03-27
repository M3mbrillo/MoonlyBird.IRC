using MoonlyBird.IRC.Core.Abstract.Message.CommandParameter;

namespace MoonlyBird.IRC.Core.Abstract.Handler;

public interface IHandlerAsync<TParamater>
    where TParamater : MessageParameter
{
    IrcCommandEventHandlerAsync<TParamater> HandlerAsync { get; }
}