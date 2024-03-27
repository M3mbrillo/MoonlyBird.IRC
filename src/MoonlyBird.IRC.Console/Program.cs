

using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MoonlyBird.IRC.Core;
using MoonlyBird.IRC.Core.Message;
using MoonlyBird.IRC.Core.Message.Builder;
using Serilog;
using Serilog.Extensions.Logging;

CancellationTokenSource masterTokenSource = new CancellationTokenSource();

var serilogLogger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .MinimumLevel.Debug()
    .CreateLogger();

var container = new ServiceCollection();
// container.AddLogging(x => x.AddSerilog());

container.AddScoped<PlaintTextClient>(serviceProvider =>
{
    var ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6667);
    // var ipEndPoint = new IPEndPoint(Dns.GetHostAddresses("irc.freenode.net").First(), 6660);
    var logger = new SerilogLoggerFactory(serilogLogger).CreateLogger<PlaintTextClient>();

    return new PlaintTextClient(ipEndPoint, logger);
});

var serviceProvider = container.BuildServiceProvider(true);
using var scope = serviceProvider.CreateScope();

var client = scope.ServiceProvider.GetRequiredService<PlaintTextClient>();


await client.StartAsync(masterTokenSource.Token).ConfigureAwait(false);

await client.SendMessageAsync(NickMessage.Build("M3mbrillo"), masterTokenSource.Token);
await client.SendMessageAsync(UserMessage.Build("m3mbrillo", "Señor M3mbrillo", "0"), masterTokenSource.Token);

// await client.SendMessageAsync("NICK CCClient", masterTokenSource.Token);
// await client.SendMessageAsync("USER guest 0 * :Coding Challenges Client", masterTokenSource.Token);

Console.ReadLine();
masterTokenSource.Cancel();
