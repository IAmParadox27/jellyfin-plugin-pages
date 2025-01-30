<h1 align="center">Plugin Pages</h1>
<h2 align="center">A Jellyfin Plugin</h2>
<p align="center">
	<img alt="Logo" width="256" height="256" src="https://camo.githubusercontent.com/ab4b1ec289bed0a0ac8dd2828c41b695dbfeaad8c82596339f09ce23b30d3eb3/68747470733a2f2f63646e2e6a7364656c6976722e6e65742f67682f73656c666873742f69636f6e732f776562702f6a656c6c7966696e2e77656270" />
	<br />
	<sub>Custom Logo Coming Soon</sub>
	<br />
	<br />
	<a href="https://github.com/IAmParadox27/jellyfin-plugin-pages">
		<img alt="GPL 3.0 License" src="https://img.shields.io/github/license/IAmParadox27/jellyfin-plugin-pages.svg" />
	</a>
	<a href="https://github.com/IAmParadox27/jellyfin-plugin-pages/releases">
		<img alt="Current Release" src="https://img.shields.io/github/release/IAmParadox27/jellyfin-plugin-pages.svg" />
	</a>
</p>

## Introduction
Plugin Pages is a Jellyfin plugin which allows other Jellyfin plugins to add user facing pages while maintaining the theming and style of the server owner. These pages feel more "at home" than other pages provided via the custom links feature Jellyfin provide.

### Examples
Below are some examples from the plugin that required this one **Home Screen Sections** which you can find here: https://github.com/IAmParadox27/jellyfin-plugin-home-sections

| <img alt="Settings Menu" src="https://raw.githubusercontent.com/IAmParadox27/jellyfin-plugin-home-sections/refs/heads/main/screenshots/settings-location.png" /> | <img alt="Settings" src="https://raw.githubusercontent.com/IAmParadox27/jellyfin-plugin-home-sections/refs/heads/main/screenshots/settings.png" /> |
| ---------------------------------------------------------------------------------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------- |
| _Plugin Settings section at the bottom of the web client hamburger menu_                                                                                         | _A settings page delivered using Plugin Pages_                                                                                                     |
## Installation

### Prerequisites
- This plugin is based on Jellyfin Version `10.10.3`
- A custom deployment of `jellyfin-web` is required to run this plugin. It can be cloned from https://github.com/IAmParadox27/jellyfin-web or there is release with the data pre-packaged for simpler install available to download.
- The `jellyfin-web` directory should be writable by whatever user is running the Jellyfin server instance. If on Windows this might mean changing the permissions for `C:\Program Files\Jellyfin\Server\jellyfin-web` to allow write access the Jellyfin user.
### Installation
1. Add `https://www.iamparadox.dev/jellyfin/plugins/manifest.json` to your plugin repositories.
2. Install `Plugin Pages` from the Catalogue.
3. Restart Jellyfin.
## Upcoming Features/Known Issues
If you find an issue with this plugin, please open an issue on GitHub.
## Contribution
### Adding your pages
There are two ways your plugin can add pages to the Plugin Pages section.

1. Reference the DLL directly and request `IPluginPagesManager` from the Dependency Injection flow and call `RegisterPluginPage` providing the config
2. Edit `Jellyfin.Plugin.PluginPages/config.json` found in the `plugins/configurations` folder of the installed Jellyfin instance. This folder location can be retrieved from `IApplicationPaths.PluginConfigurationsPath` in the .NET API.
### Pull Requests
I'm open to any and all pull requests that expand the functionality of this plugin, while keeping within the scope of what its outlined to do.
