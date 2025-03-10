<h1 align="center">Plugin Pages</h1>
<h2 align="center">A Jellyfin Plugin</h2>
<p align="center">
	<img alt="Logo" src="https://raw.githubusercontent.com/IAmParadox27/jellyfin-plugin-pages/main/src/logo.png" />
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
- This plugin is based on Jellyfin Version `10.10.6`
- Required Plugins:
  - [file-transformation](https://github.com/IAmParadox27/jellyfin-plugin-file-transformation) at least v2.2.1.0

### Installation
1. Add `https://www.iamparadox.dev/jellyfin/plugins/manifest.json` to your plugin repositories.
2. Ensure that `File Transformation` plugin is installed.
2. Install `Plugin Pages` from the Catalogue.
3. Restart Jellyfin.

## Upcoming Features/Known Issues
If you find an issue with this plugin, please open an issue on GitHub.

## Contribution
### Adding your pages
Currently the only way you can add your own pages is with the following steps.

1. Edit `Jellyfin.Plugin.PluginPages/config.json` found in the `plugins/configurations` folder of the installed Jellyfin instance. This folder location can be retrieved from `IApplicationPaths.PluginConfigurationsPath` in the .NET API.

There are plans to add a HTTP request to register pages too but this hasn't been done just yet.

### Pull Requests
I'm open to any and all pull requests that expand the functionality of this plugin, while keeping within the scope of what its outlined to do.
