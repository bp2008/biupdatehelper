# Blue Iris Update Helper

This tool can do a number of things:

## 1) **Save a copy of your Blue Iris update files.**  
Update Blue Iris with less worry that something will get broken.  This tool automatically makes a backup of every update file that Blue Iris downloads.

![Update Backups](https://i.imgur.com/bN5REZa.png)

## 2) **Make backups of Blue Iris's registry settings**  
This tool automatically backs up Blue Iris's registry settings to aid in restoration of a previous version.  Restoring registry settings is not usually necessary, but when it is, you'll be glad you have backups.  By default, backups occur daily and again each time Blue Iris performs an update.

![Registry Backups](https://i.imgur.com/dpIRjer.png)

## 3) **Submit anonymous performance data**  
Starting with version 1.6, BiUpdateHelper gathers performance data and uploads it anonymously to:

https://biupdatehelper.hopto.org/default.html#stats  
[![Performance Data](https://i.imgur.com/LfquxAw.png)](https://biupdatehelper.hopto.org/default.html#stats)  

All collected data is freely available to anyone interested in seeing how well Blue Iris performs in different system configurations.  The data upload occurs weekly.

## 4) **Help Blue Iris close itself**  
In some large systems with many cameras, Blue Iris fails to close gracefully (notably, during updates).  When this happens, your system can go unresponsive.  This tool can detect when that occurs and assist by having Windows kill the Blue Iris process from the outside, effectively working around the issue.

## 5) **Build a list of camera configuration links**  
Keeping track of camera IP addresses is easy if you have this.

![Camera Configuration Links](https://i.imgur.com/szkEXZ6.png)

## 6) **Tell you your Blue Iris registration key, in case you lost it**  
Your registration key is stored in the Windows Registry, and this tool can read it for you in case you lost it.  You'll need your key when you want to deactivate and reactivate your license on a different PC.

![Recovered Keys](https://i.imgur.com/CMz51Qj.png)


# Installation

1) Download the latest release from the [releases section](https://github.com/bp2008/biupdatehelper/releases). **NOT the green `Clone or download` button**
2) Extract to a directory of your choice.
3) Run `BiUpdateHelper.exe`.
4) Click `Install Service`.  The service will now auto-start when your computer boots.
5) To start the service without rebooting, click `Start Service`.  You may now close the Service Manager and the program will continue running in the background.

![BiUpdateHelper Service Manager](https://i.imgur.com/103K8yq.png)

## Updating

To update: Stop the service, overwrite BiUpdateHelper.exe with the new version, and start the service again.

## Configuration

Clicking `Edit Service Settings` in the BiUpdateHelper Service Manager interface will open a dialog.

![BiUpdateHelper Settings](http://i.imgur.com/52fQxhq.png)

There are 5 settings:

`Kill Blue Iris processes during update` -- (Default: `enabled`) On some systems with many cameras, Blue Iris cannot stop itself gracefully, and it maxes out the CPU while it shuts down over several minutes.  This can cause updates to fail.  This setting allows the helper service to kill Blue Iris quickly the moment an update begins.

`Backup update files` -- (Default: `enabled`) If `enabled`, the helper service will automatically make backups of update.exe, saving them in the Blue Iris directory with names like "update64_4.5.3.3.exe" or "update32_4.5.3.3.exe".

`Backup registry before each update` -- (Default: `enabled`) If enabled, this program will make a backup of Blue Iris's registry settings at the moment when Blue Iris begins to download a new update. Registry backups are stored alongside BiUpdateHelper.

`Daily registry backup` -- (Default: `enabled`) If enabled, Blue Iris's registry settings will be backed up each day and stored alongside BiUpdateHelper.

`Log verbose` -- (Default: `disabled`) For debugging purposes, you may turn on verbose logging.  This will cause the service to write a lot of information to the log file, and should only be used if troubleshooting a problem.

## Recovering Registration Keys

Though it is not the primary purpose of this tool, clicking the "BI Registration Info" button will show you what registration key you used to activate Blue Iris 3 or Blue Iris 4.  This is helpful in case you've lost your registration key and you want to deactivate and reinstall on a new PC.

## Restoring a registry backup created by BiUpdateHelper

In most cases, you can simply decompress the .reg file and double-click it to restore.  However if you are using 32 bit Blue Iris on 64 bit Windows, then this procedure will not restore the registry settings to the correct location.  To correctly restore a 32 bit Blue Iris registry backup onto 64 bit Windows, open BiUpdateHelper's GUI, click Edit Service Settings, and click "Launch 32 bit regedit". Use Regedit's file menu to Import the desired .reg file.

## Issues

Please report any defects using the project's Issues tab above.

## Notes

When this program backs up the registry, it exports `HKEY_LOCAL_MACHINE\SOFTWARE\Perspective Software\Blue Iris`.  On systems I have inspected, there is a second, much smaller set of registry entries `HKEY_CURRENT_USER\SOFTWARE\Perspective Software\Blue Iris` which is not backed up because I have not seen this used for anything more important than remembering the last window position.
