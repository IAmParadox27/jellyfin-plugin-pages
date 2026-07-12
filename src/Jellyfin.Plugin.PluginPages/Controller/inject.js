'use strict';

const PluginPages = {
    initialized: false,
    version: 1,
    init: function () {
        var MutationObserver    = window.MutationObserver || window.WebKitMutationObserver;
        var myObserver          = new MutationObserver (this.mutationHandler);
        var obsConfig           = { childList: true, characterData: true, attributes: true, subtree: true };

        $("body").each ( function () {
            myObserver.observe (this, obsConfig);
        } );
    },
    mutationHandler: function (mutationRecords) {
        if (PluginPages.initialized) {
            return;
        }
        mutationRecords.forEach ( function (mutation) {
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
        } );
    },
    onReady: function () {
        let length = $(".pluginMenuOptions").length;

        if (length !== 0)
        {
            return;
        }

        var newLayoutSelector = $("[href='#/mypreferencesmenu']");
        
        if (newLayoutSelector.length === 0) {
            $(".mainDrawer-scrollContainer").children('.userMenuOptions').after('<div class="pluginMenuOptions"></div>');
        } else {
            newLayoutSelector.after('<div class="pluginMenuOptions marker"></div>');
        }
    },
    populateSidebar: function () {
        if (ApiClient !== undefined && ApiClient !== null) {
            const url = ApiClient.getUrl('PluginPages/User');
            ApiClient.getJSON(url).then(function(items) {

                let pluginMenuOptions = $(".pluginMenuOptions")[0];

                if (items.TotalRecordCount > 0) {
                    let html = `<h3 class="sidebarHeader">Plugin Settings</h3>`;

                    function getNextHtml(items) {
                        return items.filter(function (value, index, array) {
                            return array.map(function (i) { return i.Id; }).indexOf(value.Id) === index;
                        }).map(function(item) {
                            const icon = item.Icon;
                            const itemId = item.Id;

                            const container = $(".pluginMenuOptions");
                            const type = container.hasClass("marker") ? "paper" : "classic";
                            if (type === 'paper') {
                                return `<a class="MuiButtonBase-root MuiMenuItem-root MuiMenuItem-gutters MuiMenuItem-root MuiMenuItem-gutters css-mbeig7" tabindex="-1" role="menuitem" href="#/userpluginsettings.html?pageUrl=${item.Url}" data-plugin-pages="true" >
                                        <div class="MuiListItemIcon-root css-5pks8q">
                                            <span class="MuiButton-icon MuiButton-startIcon MuiButton-iconSizeMedium css-1ygddt1 material-icons ${icon}">
                                            </span>
                                        </div>
                                        <div class="MuiListItemText-root css-t3p1a1">
                                            <span class="MuiTypography-root MuiTypography-body1 MuiListItemText-primary css-pl8nxc">${item.DisplayText}</span>
                                        </div>
                                    </a>`;
                            } else {
                                return `<a is="emby-linkbutton" data-itemid="${itemId}" class="lnkMediaFolder navMenuOption" href="#/userpluginsettings.html?pageUrl=${item.Url}" data-plugin-pages="true">
                                        <span class="material-icons navMenuOptionIcon ${icon}" aria-hidden="true"></span>
                                        <span class="sectionName navMenuOptionText">${item.DisplayText}</span>
                                    </a>`;
                            }
                        }).join('');
                    }
                    
                    let nextHtml = getNextHtml(items.Items);

                    // Remove all previous plugin pages
                    $("[data-plugin-pages=true]").remove();
                    
                    const container = $(".pluginMenuOptions");
                    if (container.hasClass("marker")) {
                        container.after(`<hr data-plugin-pages="true" class="MuiDivider-root MuiDivider-fullWidth css-14093q0" />${nextHtml}`);
                    } else {
                        container[0].innerHTML = nextHtml;
                    }
                }
            });
        }
    }
};

PluginPages.init();