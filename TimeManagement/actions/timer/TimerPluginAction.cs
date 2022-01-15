using System.Threading;
using System.Threading.Tasks;
using StreamDeckLib;
using StreamDeckLib.Messages;
using TimeManagement.Actions.Timer.Models;

namespace TimeManagement.Actions.Timer
{
	[ActionUuid(Uuid = "com.vincidev.timemanagement.action.timerPluginAction")]
	public class TimerPluginAction : BaseStreamDeckActionWithSettingsModel<TimerSettingsModel>
    {
		private bool _isWorking = false;
		private bool _tick = true;

		private int _timerValueInSeconds = 0;

		public override async Task OnKeyUp(StreamDeckEventPayload args)
		{
			await SetParameter(args.context);

			_isWorking = !_isWorking;

			if (_isWorking)
			{
				Task.Run(async () => await Timer(args.context)).Start();
			}
		}

		public override async Task OnDidReceiveSettings(StreamDeckEventPayload args)
		{
			await base.OnDidReceiveSettings(args);
			await SetParameter(args.context);
		}

		public override async Task OnWillAppear(StreamDeckEventPayload args)
		{
			await base.OnWillAppear(args);
			await SetParameter(args.context);
		}

		private async Task Timer(string context)
		{
			while (_isWorking)
			{
				Thread.Sleep(500);

				if (_isWorking)
				{
					_tick = !_tick;

					if (_tick)
					{
						_timerValueInSeconds--;
					}

					if (_timerValueInSeconds <= 0)
                    {
						_isWorking = false;

						await Manager.ShowOkAsync(context);
						Thread.Sleep(2000);
						await SetParameter(context);
					}
					else
                    {
						await Manager.SetTitleAsync(context, GetText());
					}
				}
			}
		}

		private async Task SetParameter(string context)
        {
			_tick = true;
			_timerValueInSeconds = SettingsModel.Minutes * 60 + SettingsModel.Seconds;

			await Manager.SetTitleAsync(context, GetText());
		}

		private string GetText()
		{
			var minutes = _timerValueInSeconds / 60;
			var seconds = _timerValueInSeconds % 60;
			string symbol = _tick ? ":" : " ";

			return string.Concat(
				minutes.ToString("00"),
				symbol,
				seconds.ToString("00"));
		}
	}
}
