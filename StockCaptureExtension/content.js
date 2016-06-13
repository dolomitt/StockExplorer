// Listen for messages
chrome.runtime.onMessage.addListener(function (msg, sender, sendResponse) {
    // If the received message has the expected format...
    if (msg.text === 'report_back') {
		var element = document.querySelector('#lastPrice');  "//td[@id='lastPrice'][2]/@data"
        sendResponse({'date' : Date.now(), 'price' : element.nextElementSibling.innerText});
    }
});