# biupdatehelper

This tool solves two common problems with Blue Iris updates.

1) Reverting to previous versions.  Often, people install updates through the Blue Iris user interface (or allow Blue Iris to update itself automatically), and later discover they wish to revert to a previous update.  *But they don't have the old update files!*  This tool will automatically make backups of update files that Blue Iris has downloaded, so you may run the previous update files at any time.

2) Ensuring that Blue Iris closes itself for the update.  In some large systems with many cameras, Blue Iris fails to shut down gracefully during the update process.  This tool will automatically kill blueiris.exe whenever Blue Iris updates itself, or when you run any `update*.exe` within the same directory as BlueIris.exe.

## Installation

1) Download the latest release from the [releases section](https://github.com/bp2008/biupdatehelper/releases). **NOT the green `Clone or download` button**
2) Extract to a directory of your choice.
3) Run `BiUpdateHelper.exe`.
4) Click `Install Service`.  The service will now auto-start when your computer boots.
5) To start the service without rebooting, click `Start Service`.

![BiUpdateHelper Service Manager](http://i.imgur.com/In5oKdQ.png)

## Configuration

Clicking `Edit Service Settings` in the BiUpdateHelper Service Manager interface will open a dialog where you can edit 3 settings.

![BiUpdateHelper Settings](http://i.imgur.com/QaYxylK.png)

There are 3 settings:

`killBlueIrisProcessesDuringUpdate` -- (Default: `true`) On some systems with many cameras, Blue Iris cannot stop itself gracefully, and it maxes out the CPU while it shuts down over several minutes.  This can cause updates to fail.  This setting allows the helper service to kill Blue Iris quickly the moment an update begins.

`backupUpdateFiles` -- (Default: `true`) If `true`, the helper service will automatically make backups of update.exe, saving them in the Blue Iris directory with names like "update64_4.5.3.3.exe" or "update32_4.5.3.3.exe".

`logVerbose` -- (Default: `false`) For debugging purposes, you may turn on verbose logging.  This will cause the service to write a lot of information to the log file, and should only be used if troubleshooting a problem.

## Issues

Please report any defects using the project's Issues tab above.
