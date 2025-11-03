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

## Development Update - 20th August 2025

Hey all! Things are changing with my plugins are more and more people start to use them and report issues. In order to make it easier for me to manage I'm splitting bugs and features into different areas. For feature requests please head over to <a href="https://features.iamparadox.dev/">https://features.iamparadox.dev/</a> where you'll be able to signin with GitHub and make a feature request. For bugs please report them on the relevant GitHub repo and they will be added to the <a href="https://github.com/users/IAmParadox27/projects/1/views/1">project board</a> when I've seen them. I've found myself struggling to know when issues are made and such recently so I'm also planning to create a system that will monitor a particular view for new issues that come up and send me a notification which should hopefully allow me to keep more up to date and act faster on various issues.

As with a lot of devs, I am very momentum based in my personal life coding and there are often times when these projects may appear dormant, I assure you now that I don't plan to let these projects go stale for a long time, there just might be times where there isn't an update or response for a couple weeks, but I'll try to keep that better than it has been. With all new releases to Jellyfin I will be updating as soon as possible, I have already made a start on 10.11.0 and will release an update to my plugins hopefully not long after that version is officially released!

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

### FAQ

#### I've updated Jellyfin to latest version but I can't see the plugin available in the catalogue

The likelihood is the plugin hasn't been updated for that version of Jellyfin and the plugins are strictly 1 version compatible. Please wait until an update has been pushed. If you can see the version number in the release assets then please make an issue, but if its not in the assets, please wait. I know Jellyfin has updated, I'll update when I can.

## Upcoming Features/Known Issues
If you find an issue with this plugin, please open an issue on GitHub.

## Contribution
### Adding your pages
Currently the only way you can add your own pages is with the following steps.

1. Edit `Jellyfin.Plugin.PluginPages/config.json` found in the `plugins/configurations` folder of the installed Jellyfin instance. This folder location can be retrieved from `IApplicationPaths.PluginConfigurationsPath` in the .NET API.

There are plans to add a HTTP request to register pages too but this hasn't been done just yet.

### Pull Requests
I'm open to any and all pull requests that expand the functionality of this plugin, while keeping within the scope of what its outlined to do.
