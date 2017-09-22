# biupdatehelper

This tool solves two common problems with Blue Iris updates.

1) Reverting to previous versions.  Often, people install updates through the Blue Iris user interface (or allow Blue Iris to update itself automatically), and later discover they wish to revert to a previous update.  *But they don't have the old update files!*  This tool will automatically make backups of update files that Blue Iris has downloaded, so you may run the previous update files at any time.

2) Ensuring that Blue Iris closes itself for the update.  In some large systems with many cameras, Blue Iris fails to shut down gracefully during the update process.  This tool will automatically kill blueiris.exe whenever Blue Iris updates itself, or when you run any `update*.exe` within the same directory as BlueIris.exe.

Additionally, this tool can back up Blue Iris's registry settings to aid in restoration of a previous version.  Restoring registry settings is not usually necessary, but when it is, **it really is**.

## Installation

1) Download the latest release from the [releases section](https://github.com/bp2008/biupdatehelper/releases). **NOT the green `Clone or download` button**
2) Extract to a directory of your choice.
3) Run `BiUpdateHelper.exe`.
4) Click `Install Service`.  The service will now auto-start when your computer boots.
5) To start the service without rebooting, click `Start Service`.  You may now close the Service Manager and the program will continue running in the background.

![BiUpdateHelper Service Manager](http://i.imgur.com/Ff4mFF0.png)

## Updating

To update: Stop the service, overwrite BiUpdateHelper.exe with the new version, and start the service again.

## Configuration

Clicking `Edit Service Settings` in the BiUpdateHelper Service Manager interface will open a dialog where you can edit 3 settings.

![BiUpdateHelper Settings](http://i.imgur.com/52fQxhq.png)

There are 5 settings:

`Kill Blue Iris processes during update` -- (Default: `enabled`) On some systems with many cameras, Blue Iris cannot stop itself gracefully, and it maxes out the CPU while it shuts down over several minutes.  This can cause updates to fail.  This setting allows the helper service to kill Blue Iris quickly the moment an update begins.

`Backup update files` -- (Default: `enabled`) If `enabled`, the helper service will automatically make backups of update.exe, saving them in the Blue Iris directory with names like "update64_4.5.3.3.exe" or "update32_4.5.3.3.exe".

`Backup registry before each update` -- (Default: `enabled`) If enabled, this program will make a backup of Blue Iris's registry settings at the moment when Blue Iris begins to download a new update. Registry backups are stored alongside BiUpdateHelper.

`Daily registry backup` -- (Default: `enabled`) If enabled, Blue Iris's registry settings will be backed up each day and stored alongside BiUpdateHelper.

`Log verbose` -- (Default: `disabled`) For debugging purposes, you may turn on verbose logging.  This will cause the service to write a lot of information to the log file, and should only be used if troubleshooting a problem.

## Recovering Registration Keys

Though it is not the primary purpose of this tool, clicking the "BI Registration Info" button will show you what registration key you used to activate Blue Iris 3 or Blue Iris 4.  This is helpful in case you've lost your registration key and you want to deactivate and reinstall on a new PC.

## Issues

Please report any defects using the project's Issues tab above.

## Notes

When this program backs up the registry, it exports `HKEY_LOCAL_MACHINE\SOFTWARE\Perspective Software\Blue Iris`.  On systems I have inspected, there is a second, much smaller set of registry entries `HKEY_CURRENT_USER\SOFTWARE\Perspective Software\Blue Iris` which is not backed up because I have not seen this used for anything more important than remembering the last window position.
