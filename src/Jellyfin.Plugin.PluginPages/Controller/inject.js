'use strict';

$(document.body).on('click', '.headerButton[title=Menu]', function() {
    onReady();
    populateSidebar();
});

function onReady() {
    // When the mainDrawer first get's created, lets add the plugin pages section at the bottom
    let length = $(".pluginMenuOptions").length;
    
    if (length !== 0)
    {
        return;
    }
    
    $(".mainDrawer-scrollContainer").children('.userMenuOptions').after('<div class="pluginMenuOptions"></div>');
}

function populateSidebar() {
    
    if (ApiClient !== undefined && ApiClient !== null) {
        const url = ApiClient.getUrl('PluginPages/User');
        ApiClient.getJSON(url).then(function(items) {

            let pluginMenuOptions = $(".pluginMenuOptions")[0];

            if (items.TotalRecordCount > 0) {
                let html = `<h3 class="sidebarHeader">Plugin Settings</h3>`;

                html += items.Items.map(function(item) {
                    const icon = item.Icon;
                    const itemId = item.Id;

                    return `<a is="emby-linkbutton" data-itemid="${itemId}" class="lnkMediaFolder navMenuOption" href="#/userpluginsettings.html?pageUrl=${item.Url}">
                                    <span class="material-icons navMenuOptionIcon ${icon}" aria-hidden="true"></span>
                                    <span class="sectionName navMenuOptionText">${item.DisplayText}</span>
                                </a>`;
                }).join('');

                pluginMenuOptions.innerHTML = html;
                const elem = pluginMenuOptions;
                const sidebarLinks = elem.querySelectorAll('.navMenuOption');

                for (const sidebarLink of sidebarLinks) {
                    sidebarLink.removeEventListener('click', onSidebarLinkClick);
                    sidebarLink.addEventListener('click', onSidebarLinkClick);
                }
            }
        });
    }
}