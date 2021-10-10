using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UnityBotService;
using UnityBotService.Credentials;
using UnityBotService.Twitter;
using UnityBotService.Unity;

var host = Host.CreateDefaultBuilder().ConfigureServices((context, services) =>
{
    services.AddSingleton<ITwitterClient, TweetInviClient>();
    services.AddSingleton<ITwitterManager, TwitterManager>();
    services.AddSingleton<IUnityArchiveReader, UnityArchiveReader>();
    services.AddSingleton<ICredentialManager, CredentialManager>();
    services.AddSingleton<ISecretClient, MountSecretClient>();
    services.AddSingleton<UnityArchiveParser>();
    services.AddSingleton<UnityArchiveFetcher>();
    services.AddHostedService<Bot>();
}).Build();

await host.RunAsync();
