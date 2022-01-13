var websocket = null,
	uuid = null,
	inInfo = null,
	actionInfo = {},
	settingsModel = {
		Minutes: 5,
		Seconds: 0
	};

function connectElgatoStreamDeckSocket(inPort, inUUID, inRegisterEvent, inInfo, inActionInfo) {
	uuid = inUUID;
	actionInfo = JSON.parse(inActionInfo);
	inInfo = JSON.parse(inInfo);
	websocket = new WebSocket('ws://localhost:' + inPort);

	if (actionInfo.payload.settings.settingsModel) {
		settingsModel.Minutes = actionInfo.payload.settings.settingsModel.Minutes;
		settingsModel.Seconds = actionInfo.payload.settings.settingsModel.Seconds;
	}

	document.getElementById('minutesValue').value = settingsModel.Minutes;
	document.getElementById('secondsValue').value = settingsModel.Seconds;

	websocket.onopen = function () {
		var json = { event: inRegisterEvent, uuid: inUUID };
		websocket.send(JSON.stringify(json));
	};

	websocket.onmessage = function (evt) {
		var jsonObj = JSON.parse(evt.data);
		var sdEvent = jsonObj['event'];
		switch (sdEvent) {
			case "didReceiveSettings":
				if (jsonObj.payload.settings.settingsModel.Minutes) {
					settingsModel.Minutes = jsonObj.payload.settings.settingsModel.Minutes;
					document.getElementById('minutesValue').value = settingsModel.Minutes;

					settingsModel.Seconds = jsonObj.payload.settings.settingsModel.Seconds;
					document.getElementById('secondsValue').value = settingsModel.Seconds;
				}
				break;
			default:
				break;
		}
	};
}

const setSettings = (value, param) => {
	if (websocket) {
		settingsModel[param] = value;
		var json = {
			"event": "setSettings",
			"context": uuid,
			"payload": {
				"settingsModel": settingsModel
			}
		};
		websocket.send(JSON.stringify(json));
	}
};