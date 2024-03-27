using MoonlyBird.IRC.Core.Abstract.Message.CommandParameter;

namespace MoonlyBird.IRC.Core.Abstract.Handler;

public delegate Task IrcCommandEventHandlerAsync<in TParam>(IIrcClientSender sender, TParam param, CancellationToken cancellationToken)
    where TParam : MessageParameter;