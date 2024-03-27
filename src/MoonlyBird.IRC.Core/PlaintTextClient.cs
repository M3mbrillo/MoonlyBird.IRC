using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Logging;
using MoonlyBird.Common.Extensions.String;
using MoonlyBird.IRC.Core.Abstract;
using MoonlyBird.IRC.Core.Abstract.Attribute;
using MoonlyBird.IRC.Core.Abstract.Handler;
using MoonlyBird.IRC.Core.Abstract.Message.CommandParameter;
using MoonlyBird.IRC.Core.DefaultHandler;
using MoonlyBird.IRC.Core.Message;

namespace MoonlyBird.IRC.Core;

public class PlaintTextClient : Abstract.IIrcClient, IDisposable
{
    private readonly IPEndPoint _endPoint;
    private readonly ILogger _logger;
    private Task? _taskReadingMessages;

    private TcpClient Client { get; set; }
    private NetworkStream? _stream;
    private NetworkStream Stream => _stream ??= Client.GetStream();

    private StreamWriter? _writer;
    private StreamWriter Writer => _writer ??= new StreamWriter(Stream, Encoding.ASCII);

    private StreamReader? _reader;
    private StreamReader Reader => _reader ??= new StreamReader(Stream, Encoding.ASCII);

    private Dictionary<IrcCommand, IrcCommandEventHandlerAsync<MessageParameter>> IrcCommandEvents { get; } = new();

    public PlaintTextClient(
        IPEndPoint endPoint, ILogger logger)
    {
        _endPoint = endPoint;
        _logger = logger;

        RegisterDefaultCommandHandlers();
        
        IrcCommandEvents.Add(IrcCommand.PING, WrapperAsyncRaiseCommandEvent(OnCommandPing, (MessageParameter message) => new PingMessageParameter(message) ));
        IrcCommandEvents.Add(IrcCommand.NOTICE, WrapperAsyncRaiseCommandEvent(OnCommandNotice, (MessageParameter message) => new NoticeMessageParameter(message) ));

        Client = new TcpClient();
    }

    /// <summary>
    ///     wrap execute of all command event to run events async
    /// </summary>
    /// <param name="eventToWrap"></param>
    /// <param name="paramToWrap"></param>
    /// <returns></returns>
    private IrcCommandEventHandlerAsync<MessageParameter> WrapperAsyncRaiseCommandEvent<TParam>(IrcCommandEventHandlerAsync<TParam>? eventToWrap, Func<MessageParameter, TParam> paramToWrap)
        where TParam : MessageParameter
    {
        return (async (sender, parameter, token) =>
        {
            if (eventToWrap == null)
                return;
            
            var messageParam = paramToWrap(parameter);
            await eventToWrap(sender, messageParam, token);
        });
    }

    
    public void RegisterDefaultCommandHandlers()
    {
        OnCommandPing += new PingHandlerAsync().HandlerAsync;
        OnCommandNotice += new NoticeHandlerAsync(_logger).HandlerAsync;
        
#if DEBUG
        // OnMessageReceived += (sender, parameter) => { _logger.LogDebug("OnMessageReceived | {0}", parameter); };
        // OnMessageSending += (sender, parameter) => { _logger.LogDebug("RISE EVENT -> OnMessageSending: RAW -> {0}", parameter); };
        OnMessageSent += (sender, parameter) => { _logger.LogDebug("OnMessageSent | {0}", parameter); };
#endif

    }
    
    public event EventHandler<string>? OnMessageReceived;
    public event EventHandler<string>? OnMessageSending;
    public event EventHandler<string>? OnMessageSent;
    public event EventHandler<Exception>? OnException;
    
    public event IrcCommandEventHandlerAsync<PingMessageParameter>? OnCommandPing;
    public event IrcCommandEventHandlerAsync<NoticeMessageParameter>? OnCommandNotice;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        
        try
        {
            await Client.ConnectAsync(_endPoint, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Error at connect to [{_endPoint}]", _endPoint.ToString());
            OnException?.Invoke(this, e);
            return;
        }

        _taskReadingMessages = Task.Run(() =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var message = string.Empty;
                try
                {
                    message = Reader.ReadLine();
                }
                catch (Exception e)
                {
                    _logger.LogCritical(e, "Error at read message ");
                    OnException?.Invoke(this, e);
                }

                if (!message?.IsNullOrEmpty() ?? false)
                {
                    OnMessageReceived?.Invoke(this, message);
                    var serverMessage = new RawServerMessage(message);

                    if (IrcCommandEvents.TryGetValue(serverMessage.IrcCommand, out var @event))
                    {
                        @event(this, serverMessage.GetMessageParameter(), cancellationToken);
                    }
                    else if (serverMessage.IrcCommand < IrcCommand.NOTICE)
                    {
                        // All RPL from 001 to 999
                        var raw = serverMessage.GetMessageParameter().RawMessage;
                        _logger.LogInformation("PLR {code:000} | {message}", (int)serverMessage.IrcCommand ,raw);
                    }
                    else
                    {
                        _logger.LogWarning("Unhandled command [{command}] from raw message: {raw}", serverMessage.IrcCommand, serverMessage.GetMessageParameter().RawMessage);
                    }
                }
            }
        }, cancellationToken);
    }

    private readonly SemaphoreSlim _semaphoreSendMessageAsync = new(1, 1);

    public async Task SendMessageAsync(StringBuilder messageBuilder, CancellationToken cancellationToken)
    {
        await _semaphoreSendMessageAsync.WaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            OnMessageSending?.Invoke(this, messageBuilder.ToString());
            await Writer.WriteLineAsync(messageBuilder, cancellationToken).ConfigureAwait(false);
            await Writer.FlushAsync(cancellationToken).ConfigureAwait(false);
            OnMessageSent?.Invoke(this, messageBuilder.ToString());
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error sending message: [{message}]", messageBuilder.ToString());
            OnException?.Invoke(this, e);
        }
        finally
        {
            _semaphoreSendMessageAsync.Release();
        }
    }

    public async Task SendMessageAsync(string message, CancellationToken cancellationToken)
    {
        var messageBuilder = new StringBuilder(message);
        await SendMessageAsync(messageBuilder, cancellationToken);
    }
    
    public void Dispose()
    {
        Client.Dispose();
    }
    
}