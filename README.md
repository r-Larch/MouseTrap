# MouseTrap

MouseTrap is a small tool to map the cursor between multiple monitors with <br>
different resolutions and scaling settings.

## Min requirements

This tool needs the dpiAwareness api `PerMonitorV2` first introduced in **Windows 10 Creators update**.<br>
So **for this tool to function correctly** you should have at least **Windows build 1703**


## Download and Setup

You can find the latest release hier:
> [Download](https://github.com/r-Larch/MouseTrap/releases)

You can install MouseTrap with **chocolatey**:
```Powershell
# install the package
choco install mousetrap --version 1.0.1

# running it
mousetrap
```

If you don't have chocolatey allready, I highly recommend installing it.<br>
It's as simple as pasting the following in a cmd or Powershell window:
```Powershell
PowerShell
Set-ExecutionPolicy Bypass -Scope Process -Force; iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))
```
*[Why Chocolatey](https://chocolatey.org/docs/why) + [installation](https://chocolatey.org/docs/installation)*

## Usage and Configuration

You can allways find your running **MouseTrap** app as system tray icon in the right corner of your Taskbar.
Right clicking the icon shows some options:

   Option    |   Description
-------------|------------------------------------------------------------------
**Settings** | allows you to configure it
Reinit       | **[alpha-option]** to reinitialize it, in case it hangs :worried:
Full Restart | **[alpha-option]** in case something hangs heavily :fearful:
**Exit**     | to close the running instance

![Tray icon](https://raw.githubusercontent.com/r-Larch/MouseTrap/master/images/tray-snap.jpg)

Hit **Settings** in the menu and you see a screen like in the following picture, where you can see your monitors and their alignment.
or better how Windows sees them. *(Windows has no clue how big your screens really are).*
<br>
Hit **Configure Screen Bridges** to configure how this tool maps your mouse pointer between Monitors.<br>
And don't forget to check the checkbox on the left, if it isn't allready, to ensure MouseTrap starts automatically
if your computer turns of and on again.

![Settings Screen](https://raw.githubusercontent.com/r-Larch/MouseTrap/master/images/settings-screen.jpg)

### Configure Screen Bridges

On the configuration screen hit the '**+**' buttons to add bridges between your Monitors.<br>
Then resize them to match your physical screen size.

Look at the pictures to get an idea of how it should look.

![Config Screen](https://raw.githubusercontent.com/r-Larch/MouseTrap/master/images/config-screen.jpg)

The blue-reddish bar should cover the space where you wand to move your mouse-pointer between your Monitors.<br>
And **it counts the hole size**, from red tip to red tip.

![Bridge](https://raw.githubusercontent.com/r-Larch/MouseTrap/master/images/bridge-pic.jpg)


![Bridge top](https://raw.githubusercontent.com/r-Larch/MouseTrap/master/images/bridge-top-pic.jpg)
![Bridge bottom](https://raw.githubusercontent.com/r-Larch/MouseTrap/master/images/bridge-bottom-pic.jpg)
![Bridge](https://raw.githubusercontent.com/r-Larch/MouseTrap/master/images/bridge-snap.jpg)

Your can allways test your Settings before save them.<br>
Exit this configuration screen by hitting your `ESC` key or by hitting the `Close` button.

![Test and save](https://raw.githubusercontent.com/r-Larch/MouseTrap/master/images/test-and-save.jpg)

If you encounter any bugs don't hesitate to open an issue and I will give my best to fix it.
