'use strict';

$(document.body).on('click', '.headerButton[title=Menu]', function() {
    onReady();
    populateSidebar();
});

window.navigation.addEventListener("navigate", (event) => {
    
    if (event.destination.url.includes('#')) {
        let parts = event.destination.url.split('#');
        
        if (parts[1].startsWith('/userpluginsettings.html')) {
            loadHtml(parts[1]);
        }
    }
});

function loadHtml(url) {
    // ApiClient.ajax({
    //     type: 'GET',
    //     url: '/web' + url
    // }).then(function(response) {
    //     const fragment = document.createRange().createContextualFragment(response);
    //
    //     const element = $('.mainAnimatedPages')[0];
    //     element.append(fragment);
    // });
}

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

$(document.body).on('ready', '.mainDrawer-scrollContainer', onReady);

$(document.body).on('ready', '.pluginMenuOptions', populateSidebar);

function onSidebarLinkClick() {
    const section = this.getElementsByClassName('sectionName')[0];
    const text = section ? section.innerHTML : this.innerHTML;
    LibraryMenu.setTitle(text);
}

setTimeout(function() {
    if (window.location.hash.includes('userpluginsettings.html')) {
        loadHtml(window.location.hash.split('#')[1]);
    }
}, 1000);