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
    load() {
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
