using Microsoft.Extensions.Logging;
using MoonlyBird.IRC.Core.Abstract.Handler;
using MoonlyBird.IRC.Core.Abstract.Message.CommandParameter;

namespace MoonlyBird.IRC.Core.DefaultHandler;

public class NoticeHandlerAsync : INoticeHandlerAsync
{
    private readonly ILogger _logger;

    public NoticeHandlerAsync(ILogger logger)
    {
        _logger = logger;
    }
    
    public IrcCommandEventHandlerAsync<NoticeMessageParameter> HandlerAsync => async (sender, parameter, token) =>
    {
        _logger.LogInformation("{message}", parameter.Message);
    };
}