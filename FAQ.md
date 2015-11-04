# (English) KanColleViewer! FAQ
This document will attempt to answer some of the questions that keep popping up in random places.
Any additions are welcome.

## First things you must always do before reporting any issues:

1. If you are using **Windows 7** and have not updated it to **Service Pack 1** yet, do it.
2. Update your Microsoft Internet Explorer to *the latest available version*. That means **Internet Explorer 11** as of December 15th, 2014.
3. Update your **.NET Framework** to the latest version (**4.5.2** as of this writing: use either [Offline installer](http://www.microsoft.com/en-us/download/details.aspx?id=42642) or [Web Installer](http://www.microsoft.com/en-us/download/details.aspx?id=42643)).
4. Update KCV to its latest version. Pay attention to the release notes.
5. If your question is related to translations, update translations to the latest version.
6. **Do not put KCV under Program Files.** Current English builds store translation XML files inside the program's directory and when located inside Program Files these files cannot be properly saved (because elevation is required). This will be fixed in the future.
7. **Any modifications to the .exe.config file are not officially supported.** This file is **not** read by KCV and it has no control over its parsing. The config file is generated at compile time and is read by the .NET Framework. **If KCV crashes with a configuration error in .exe.config, this means you, and only you, are to blame.** KCV ships with a correct configuration file.

## Frequently Asked Questions
#### How do I use the region cookie method?
Do not spam-click the button; instead do the following:

1. Open KCV.
2. Wait for DMM.com to finish loading; it will most probably be in English.
3. Click **Set region cookie**.
4. Click **日本語** in the top right to switch to the Japanese language version of DMM.com.
5. Either close and restart KCV or proceed to **オンラインゲーム** and then **艦隊これくしょん**.

#### KCV does not translate anything inside the game.
KCV will never do that as that requires changing the data received from the server before passing it to the game client. This can be easily detected by DMM and is definitely grounds for a ban.

#### KCV shows ships being constructed as “???”.
This is the default. Tick the **Reveal ship names** checkbox in the **Shipyard** tab to see the names.

#### Is there a way to reload the game without going to **Settings** → **Browser**?
**Press F5** just like you do in your favourite web browser.

#### I pinned KCV to my taskbar and translations aren't working.
Right click the pinned button on the taskbar. A context menu will appear with KanColleViewer at the top. Right click that line, it'll open another context menu. Choose Properties in there:

![Taskbar Screenshot](http://i.koumakan.jp/2015-01-20/1421749535.png)

Explorer will open the standard shortcut properties window:

![Properties Screenshot](http://i.koumakan.jp/2015-01-20/1421749669.png)

Make sure the **Start in:** folder is set to the folder where KCV is.

#### I made a shortcut for KCV and translations aren't working.
See the answer above, but instead of the taskbar button's shortcut properties you'll need to right-click your shortcut and choose Properties in that context menu. Look for **Start in:** and make sure it points to the folder with the KCV executable.

#### Translations aren't working even though KCV is not pinned or has **Start in** set correctly.
Try closing KCV, removing the XML files in the translations folder and restarting KCV; it should download translations automatically. If you have that option disabled, download them manually. If this still does not help, you might have permission issues.
Another approach: open **Windows File Explorer** (NOT Internet Explorer; to start File Explorer you can use the following keyboard shortcut: Windows Logo Key + E) and insert `%APPDATA%\grabacr.net\KanColleViewer\` into its address bar. Delete **Settings.xml** there. Note: this will reset your KCV settings.

#### Every time I start KanColle, it seems to re-download every asset (CG, BGM, etc.)
Make sure your system time is correct. Enable time synchronisation if your BIOS RTC is faulty (**Internet Time** tab in the **Date and Time** applet).
*If you are running Windows 10 Technical Preview:* this is a known issue; as a workaround you can use a local caching proxy server ([polipo-win32](http://www.pps.univ-paris-diderot.fr/~jch/software/files/polipo/) should do the trick but setting it up is outside the scope of this document so I won't provide any instructions; you can also use Squid/win32 or any other HTTP proxy server you prefer).

#### KCV shows the game but the bottom (or right, for the horizontal version) pane remains in the *Waiting for KanColle to start* state.
You probably launched a second copy of KCV. As KCV uses FiddlerCore's proxying functionality, only the first instance of KCV is able to set up Fiddler to listen on the port defined in the configuration file. Any other instances will get their data processed by that first instance's Fiddler.
If you need to run two copies at once, copy the KCV .exe file and its .exe.config file under a new name (say, KanColleViewer2.exe and KanColleViewer2.exe.config) and edit
```xml
<setting name="LocalProxyPort" serializeAs="String">
  <value>37564</value>
</setting>
```
replacing `37564` with some other value (recommended to be above `10000`; also, port number cannot exceed `65534`). If you start the renamed executable after that, it'll use its own port for its Fiddler instance.

#### KCV shows the game wrong (I'm missing ship CG, map selection is wrong, etc.)
Try clearing the cache (either using the button on the KCV start screen or in Internet Explorer's Internet Options). If you are using API links, refresh your API token.

#### Game hangs on the loading screen.
Check if the game works in your system's Internet Explorer and try clearing your cache.

#### Instead of showing the game, KCV tells me to download something.
Install [Adobe Flash Player](http://get.adobe.com/flashplayer/).

#### Instead of the game I only see a small red cross in the top left corner.
Most probably you have Flash disabled in your Add-on settings in Internet Explorer. Click the gear icon on the toolbar (it's on the very right) and choose **Manage Add-ons**; check if Adobe Flash is disabled under the **Toolbars and extensions** category.
*Important note:* Adobe Flash might not be present in the extension list because Internet Explorer has its own Flash plug-in. If that is the case, try installing Adobe's version.

#### Instead of the game I only see white.
You may have ActiveX filtered or disabled in your Internet Explorer settings. Adobe Flash uses ActiveX; disable filtering and/or enable ActiveX in Internet Explorer settings.

#### How to make KCV load my API link automatically?
Edit `KanColleViewer.exe.config` (or the appropriate configuration file for the horizontal build).
Change `http://www.dmm.com/netgame/social/application/-/detail/=/app_id=854854/` in
```xml
<setting name="KanColleUrl" serializeAs="String">
  <value>http://www.dmm.com/netgame/social/application/-/detail/=/app_id=854854/</value>
</setting>
```
to your API link URL, but *make sure you replace `&` with `&amp;`* (this is called an *HTML escape*; `&` has a special meaning in XML and KCV will probably crash if you don't properly escape it).

The end result should look kind of like this:

```xml
<setting name="KanColleUrl" serializeAs="String">
  <value>http://ip.octets.go.here/kcs/mainD2.swf?api_token=█████████████████&amp;api_starttime=█████████████</value>
</setting>
```

There *must be* no quotation marks of any kind. The URL *must be* an absolute URL (in other words, it *must* start with `http://`).

#### I use API link and the Flash quality settings don't work.
You must use the game with the DMM site for them to work, or you can make up your own HTML page that embeds the Flash object and make a script for it.

#### KCV crashes randomly on my overclocked PC.
Some users reported crashes when running in an overclocked environment. Try disabling overclocking.

#### How to report an issue?
You'll need a GitHub account. If the issue in question is not related to translations of quests, ship names, equipment and other in-game strings, use [KCV's issue tracker](https://github.com/Yuubari/KanColleViewer/issues). For everything related to translations of in-game strings use [KCV Localisation issue tracker](https://github.com/KCV-Localisation/KanColleViewer-Translations/issues); this is also the place for submitting new quests although now this isn't needed as they are submitted automatically.

KanColleViewer! automatically generates a crash log file, `error.log`, in its folder. In cases of crashes please attach this file to your report.
