using MoonlyBird.IRC.Core.Abstract;
using MoonlyBird.IRC.Core.Abstract.Handler;
using MoonlyBird.IRC.Core.Abstract.Message.CommandParameter;
using MoonlyBird.IRC.Core.Message.Builder;

namespace MoonlyBird.IRC.Core.DefaultHandler;

public class PingHandlerAsync : IPingHandlerAsync
{
    public IrcCommandEventHandlerAsync<PingMessageParameter> HandlerAsync =>
        async (sender, parameter, token) =>
        {
            var message = Message.Builder.PongMessage.Build(parameter.Server1, parameter.Server2);
            
            await sender.SendMessageAsync(message, token).ConfigureAwait(false);
        };
}