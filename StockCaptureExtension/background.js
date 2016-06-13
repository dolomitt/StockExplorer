// A function to use as callback
function ProcessPriceData(domContent) {
	jQuery.ajax({
		type: "POST",
		url: "http://localhost:8082/",
		data: domContent,
		success: function(data) {
			console.log(data);
		}
	});
}

var tabId;

// When the browser-action button is clicked...
chrome.browserAction.onClicked.addListener(function (tab) {
	//console.log('Background.js Received something ' + tab.id);
	chrome.tabs.sendMessage(tab.id, {text: 'report_back'}, ProcessPriceData);
	tabId = tab.id;
	AlarmUpdated(false);
});

var RequestUpdate = function() {
	if (tabId!=null)
		chrome.tabs.sendMessage(tabId, {text: 'report_back'}, ProcessPriceData);
}

var AlarmUpdated = function(wasCleared) {
	chrome.alarms.create('CollectStock', {when: Date.now() + 1000});
}

chrome.alarms.onAlarm.addListener(function(alarm) {
	if (alarm.name == 'CollectStock')
	{
		RequestUpdate();
		chrome.alarms.clear('CollectStock', AlarmUpdated);
	}
});

