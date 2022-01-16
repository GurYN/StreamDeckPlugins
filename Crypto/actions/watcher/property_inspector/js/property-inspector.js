var websocket = null,
    uuid = null,
    inInfo = null,
    actionInfo = {},
    settingsModel = {
        Name: 'bitcoin',
        Currency: 'usd',
        RefreshDelay: 10
    };

function connectElgatoStreamDeckSocket(inPort, inUUID, inRegisterEvent, inInfo, inActionInfo) {
    uuid = inUUID;
    actionInfo = JSON.parse(inActionInfo);
    inInfo = JSON.parse(inInfo);
    websocket = new WebSocket('ws://localhost:' + inPort);

    if (actionInfo.payload.settings.settingsModel) {
        settingsModel.Name = actionInfo.payload.settings.settingsModel.Name;
        settingsModel.Currency = actionInfo.payload.settings.settingsModel.Currency;
        settingsModel.RefreshDelay = actionInfo.payload.settings.settingsModel.RefreshDelay;
    }

    document.getElementById('nameValue').value = settingsModel.Name;
    document.getElementById('currencyValue').value = settingsModel.Currency;
    document.getElementById('refreshDelayValue').value = settingsModel.RefreshDelay;

    websocket.onopen = function () {
        var json = { event: inRegisterEvent, uuid: inUUID };

        websocket.send(JSON.stringify(json));
    };

    websocket.onmessage = function (evt) {
        var jsonObj = JSON.parse(evt.data);
        var sdEvent = jsonObj['event'];
        switch (sdEvent) {
            case "didReceiveSettings":
                if (jsonObj.payload.settings.settingsModel.Name) {
                    settingsModel.Name = jsonObj.payload.settings.settingsModel.Name;
                    document.getElementById('nameValue').value = settingsModel.Name;
                }

                if (jsonObj.payload.settings.settingsModel.Currency) {
                    settingsModel.Currency = jsonObj.payload.settings.settingsModel.Currency;
                    document.getElementById('currencyValue').value = settingsModel.Currency;
                }

                if (jsonObj.payload.settings.settingsModel.RefreshDelay) {
                    settingsModel.RefreshDelay = jsonObj.payload.settings.settingsModel.RefreshDelay;
                    document.getElementById('refreshDelayValue').value = settingsModel.RefreshDelay;
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

