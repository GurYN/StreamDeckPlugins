using Crypto.actions.watcher.services;
using Crypto.Actions.Watcher.Models;
using Crypto.Actions.Watcher.Services;
using StreamDeckLib;
using StreamDeckLib.Messages;
using System.IO;
using System.Threading.Tasks;

namespace Crypto.Actions.Watcher
{
    [ActionUuid(Uuid = "com.vincidev.crypto.WatcherPlugin")]
    public class WatcherPluginAction : BaseStreamDeckActionWithSettingsModel<WatcherSettingsModel>
    {
        public override async Task OnKeyUp(StreamDeckEventPayload args)
        {
            await RefreshCoin(args.context);
        }

        public override async Task OnDidReceiveSettings(StreamDeckEventPayload args)
        {
            await base.OnDidReceiveSettings(args);

            await RefreshCoin(args.context);
        }

        public override async Task OnWillAppear(StreamDeckEventPayload args)
        {
            await base.OnWillAppear(args);

            Task.Run(async () => await StartScheduler(args.context)).Start();
        }

        private async Task StartScheduler(string context)
        {
            while (true)
            {
                await RefreshCoin(context);

                await Task.Delay(SettingsModel.RefreshDelay * 60 * 1000);
            }
        }

        private async Task RefreshCoin(string context)
        {
            if (!string.IsNullOrEmpty(SettingsModel.Name) && !string.IsNullOrEmpty(SettingsModel.Currency))
            {
                var coin = await ApiService.GetCoinData(SettingsModel.Name);
                if (coin != null)
                {
                    var imageService = new ImageService(coin, SettingsModel.Currency);
                    await Manager.SetImageAsync(context, await imageService.GetImage());
                }
                else
                {
                    var img = Path.Combine(Directory.GetCurrentDirectory(), "images/notFound.png");
                    await Manager.SetImageAsync(context, img);
                }
            }
        }
    }
}
