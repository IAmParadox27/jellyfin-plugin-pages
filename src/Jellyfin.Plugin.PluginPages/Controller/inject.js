'use strict';

const PluginPages = {
    initialized: false,
    version: 1,
    init: function () {
        var MutationObserver = window.MutationObserver || window.WebKitMutationObserver;
        var myObserver = new MutationObserver(this.mutationHandler);
        var obsConfig = { childList: true, characterData: true, attributes: true, subtree: true };

        $("body").each(function () {
            myObserver.observe(this, obsConfig);
        });
    },
    mutationHandler: function (mutationRecords) {
        if (PluginPages.initialized) {
            return;
        }
        mutationRecords.forEach(function (mutation) {
            console.log(mutation.type);
            if (mutation.addedNodes && mutation.addedNodes.length > 0) {

                [].some.call(mutation.addedNodes, function (addedNode) {
                    if ($('.mainDrawer-scrollContainer').length > 0) {
                        if ($(".mainDrawer-scrollContainer").children('.userMenuOptions').length > 0) {
                            PluginPages.initialized = true;
                            PluginPages.onReady();
                            PluginPages.populateSidebar();
                        }
                    }
                });
            }
        });
    },
    onReady: function () {
        let length = $(".pluginMenuOptions").length;

        if (length !== 0) {
            return;
        }

        $(".mainDrawer-scrollContainer").children('.userMenuOptions').after('<div class="pluginMenuOptions"></div>');
    },
    populateSidebar: function () {
        if (ApiClient !== undefined && ApiClient !== null) {
            // 1. Determine the correct Base URL from the browser location
            // This ensures it matches the DNS/Proxy address currently in use
            const getBaseUrl = () => {
                return window.location.origin + window.location.pathname.split('/web/')[0];
            }

            // 2. Instead of ApiClient.getUrl(), construct the URL manually
            // to ensure it uses the browser's current context
            const url = getBaseUrl() + '/PluginPages/User';

            console.log('Plugin Pages - Final API URL:', url);

            // 3. Use the built-in Fetch or ApiClient with the explicit URL
            ApiClient.getJSON(url).then(function (items) {
                let pluginMenuOptions = $(".pluginMenuOptions")[0];

                if (items && items.TotalRecordCount > 0) {
                    let html = `<h3 class="sidebarHeader">Plugin Settings</h3>`;

                    html += items.Items.map(function (item) {
                        const icon = item.Icon;
                        const itemId = item.Id;

                        return `<a is="emby-linkbutton" data-itemid="${itemId}" class="lnkMediaFolder navMenuOption" href="#/userpluginsettings.html?pageUrl=${item.Url}">
                                    <span class="material-icons navMenuOptionIcon ${icon}" aria-hidden="true"></span>
                                    <span class="sectionName navMenuOptionText">${item.DisplayText}</span>
                                </a>`;
                    }).join('');

                    pluginMenuOptions.innerHTML = html;
                }
            }).catch(err => {
                console.error("PluginPages Error:", err);
            });
        }
    }
};

PluginPages.init();
