using System;
using System.Threading;
using System.Threading.Tasks;
using StreamDeckLib;
using StreamDeckLib.Messages;

namespace TimeManagement.Actions.Stopwatch
{
	[ActionUuid(Uuid= "com.vincidev.timemanagement.action.stopwatchPluginAction")]
	public class StopwatchPluginAction : BaseStreamDeckAction
	{
		private bool _isWorking;
		private bool _tick;

		private DateTime _beginStopwatch, _currentStopwatch;

        public StopwatchPluginAction()
        {
			_beginStopwatch = _currentStopwatch = DateTime.Now;
			_tick = true;
			_isWorking = false;
		}

		public override async Task OnKeyUp(StreamDeckEventPayload args)
		{
			_beginStopwatch = _currentStopwatch = DateTime.Now;
			_tick = true;

			await Manager.SetTitleAsync(args.context, GetText());

			_isWorking = !_isWorking;

			if (_isWorking)
			{
				Task.Run(async () => await Stopwatch(args.context)).Start();
			}
		}

		public override async Task OnWillAppear(StreamDeckEventPayload args)
		{
			await base.OnWillAppear(args);
			await Manager.SetTitleAsync(args.context, GetText());
		}

		private async Task Stopwatch(string context)
        {
			while (_isWorking)
            {
				Thread.Sleep(500);

				if (_isWorking)
				{
					_currentStopwatch = DateTime.Now;
					_tick = !_tick;

					await Manager.SetTitleAsync(context, GetText());
				}
			}
		}

		private string GetText()
        {
			var currentValue = _currentStopwatch.Subtract(_beginStopwatch);
			string symbol = _tick ? ":" : " ";

			return string.Concat(
				currentValue.Minutes.ToString("00"),
				symbol,
				currentValue.Seconds.ToString("00"));
		}
	}
}
