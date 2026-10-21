# Poliekran — User Guide

Version 1.3. [Русская версия](guide-ru.md)

Poliekran is an add-in for Microsoft PowerPoint. It runs several shows at once on different monitors of one PC: a presentation on one screen, an electronic timetable on another, something else on a third. The shows keep running while people work in other programs on the same computer.

The interface can be English or Russian. Choose the language in the installer; the add-in uses the same language when PowerPoint starts.

## 1. What you need

- A PC with Windows 10 or 11 and two or more monitors (TVs connected as monitors work too).
- Microsoft PowerPoint 2013 or later, including Microsoft 365.
- .NET Framework 4.6.2 or later. Windows 10 and 11 already have it.

No administrator rights are needed.

## 2. Installing

1. Close PowerPoint.
2. Run `Poliekran-1.3.0-setup.exe`.
3. If Windows shows "Windows protected your PC", click **More info** → **Run anyway**. The installer has no digital signature, so Windows does not recognize it.
4. Choose English in the installer. The wizard, its buttons and the final page will then be in English. You can tick **Launch PowerPoint** on the last page.

The program goes into `%LOCALAPPDATA%\Programs\Poliekran`. This guide and a short note in the selected language, `README.txt`, are placed there too.

## 3. Finding the button

Open PowerPoint and go to the **Slide Show** tab. On the right you will see a group and a button, both called **Poliekran**. Click the button to open the control panel.

If the button is missing, see section 10.

## 4. The control panel

From top to bottom:

| Label on screen | Meaning | What it does |
|---|---|---|
| (monitor list) | — | Picks the screen for the show. Lines look like “2: 1920×1080, position 1920,0”. The main monitor is marked “primary”. |
| Full screen / Window | Full screen / Window | Show mode. Use Full screen. Window is a fallback and less reliable. |
| Start file… | Start file… | Choose a presentation and start it on the selected monitor. |
| Refresh monitors | Refresh monitors | Re-read the monitor list after connecting or disconnecting a screen. |
| Advance slides without timing after | Advance slides without timing after | How long to keep a slide that has no timing of its own. Default 15 s. Do not advance waits for a manual click. |
| Restart the show when the file changes | Restart the show when the file changes | Watch the file and show the new version when it is saved again. Needed for timetables. |
| (show table) | File, Monitor, Slide, Status, Settings | One row per running show. |
| «< Назад», «Вперёд >» | Back, Forward | Previous or next slide in the selected show. |
| «Перезапустить» | Restart | Start the selected show again. |
| Stop, Stop all | Stop, Stop all | Stop the selected show or all of them. |
| Esc stops the show | Esc stops the show | Allow stopping a show with Esc on the screen itself. Clear it for screens nobody should touch. |

Back, Forward, Restart and Stop act on the shows selected in the table. With one show running, no selection is needed.

You can close the control panel with the X button: the shows keep running. The same ribbon button brings the panel back.

## 5. Starting a show on a monitor

1. Pick the screen in the monitor list.
2. Leave the mode at Full screen.
3. Choose the time for slides without their own timing:
   - info screen or timetable: 10–30 seconds;
   - an ordinary talk where someone clicks through the slides: Do not advance.
4. If the file will be updated, keep Restart the show when the file changes ticked.
5. Click Start file… and choose the presentation.

The show opens full screen on the chosen monitor. Repeat for the other monitors. The same file can run on several screens.

A show always plays every slide in a loop. Your original file is never changed: the program works with a copy.

## 6. Electronic timetables

### Preparing the file

- Make the timetable an ordinary PowerPoint presentation, one page per slide.
- Several pages can turn automatically. Either set the timing in the presentation itself (Transitions → Timing → After), or pick an interval in the control panel when you start the show.
- Keep the file wherever it is convenient to edit: on the same PC or on a network share.
- Do not protect the file with a password: PowerPoint would stop and ask for it.

### Updating

Open the file on any computer, edit it and save. There is no need to close it. In about 10 seconds the screen shows the new version by itself.

How it works: the program looks at the file every 5 seconds. When it sees a new version and the file has stopped changing, it opens the new version first and only then replaces the running show. If the new file cannot be opened yet (for example, it is still being copied), the old timetable stays on screen and the program tries again later. If the new timetable has fewer pages, the show starts from the first one.

While a show is running, the original file is not locked: you can overwrite it, rename it or replace it with another file of the same name.

## 7. Managing running shows

### Statuses in the table

| Status | Meaning |
|---|---|
| «идёт» | Running. |
| «перезапуск» | Starting or restarting; takes a few seconds. |
| «упал, повтор через N с» | Crashed, retry in N s: the show closed several times in a row, so the program paused it and will try again in N seconds. |

### Stopping

To stop a show for good:

- click Stop or Stop all in the control panel;
- on the screen itself, press Esc, the hyphen key, Ctrl+Break, or choose End Show in the right-click menu (only if Esc stops the show is ticked);
- close PowerPoint: all shows stop.

### Automatic restart

If a show window closes on its own (a glitch, another program interfering), the show starts again from the same slide. If this happens five times within a minute, the program pauses that show for 60 seconds and then tries again. «Перезапустить» (Restart) tries at once.

## 8. Unattended screens

If the timetable PC stands in a lobby and nobody comes near it:

- Clear Esc stops the show in the control panel. An accidental Esc will then not blank the screen: the show comes back within a few seconds.
- Stop Windows from turning off the display or going to sleep. Windows 11: Settings → System → Power (Power & battery on laptops) → Screen and sleep. Windows 10: Settings → System → Power & sleep. Set everything to Never.
- Turn off the screen saver: Settings → Personalization → Lock screen → Screen saver, choose (None).
- Windows may restart for updates. After a restart the shows have to be started again: there is no automatic start yet.

## 9. Updating and uninstalling

**Updating.** Close PowerPoint and run the new installer. It notices the installed copy and offers Update to version … or reinstall. Click Next.

**Uninstalling**, any of these:

- run the installer again and choose Uninstall Poliekran;
- Windows 11: Settings → Apps → Installed apps → ⋯ next to Poliekran → Uninstall;
- Windows 10: Settings → Apps → Apps & features → Poliekran → Uninstall.

Close PowerPoint first. The program files, registry entries and the log are removed.

For administrators: silent install `Poliekran-1.3.0-setup.exe /S`, silent uninstall `Poliekran-1.3.0-setup.exe /uninstall /S`. Exit codes: 0 success, 3 PowerPoint is running, 4 no .NET Framework 4.6.2, 5 the user folder path contains # or %.

## 10. Troubleshooting

**No Poliekran button on the ribbon.**
File → Options → Add-ins. At the bottom, set Manage to COM Add-ins, click Go and tick Poliekran. If it is not in that list, set Manage to Disabled Items, click Go, select Poliekran, click Enable and restart PowerPoint.

**PowerPoint asked whether to disable the add-in.**
Click No and send the log (see below).

**The show opened on the wrong monitor.**
Click Refresh monitors, pick the screen again, stop the show and start it again.

**The timetable does not update.**
Check that Restart the show when the file changes was ticked when the show started (the Settings column says watching file), and that people save the same file that is running, not a copy with another name. For a file on a network share, check that the share is reachable from this PC.

**Slides do not advance.**
The Settings column says Do not advance and the slides have no timing of their own. Stop the show and start it again with an interval, for example 15 s.

**The installer says PowerPoint is still running.**
Close all PowerPoint windows. If the message stays, PowerPoint is stuck in the background: open Task Manager (Ctrl+Shift+Esc), find Microsoft PowerPoint, click End task, then «Повторить» (Retry).

**Do not run PowerPoint as administrator.** In that mode it may not see the add-in.

### The log

The program writes a log to `%LOCALAPPDATA%\Poliekran\log.txt`. To find it, paste this path into the File Explorer address bar. The log is cleared once it passes 1 MB.

When asking for help, attach the log, give your Windows and Office versions and describe what happened. The log contains full paths to your files, so remove anything private before sending it.

## 11. Language selection

Choose English or Russian in the installer. The selected language applies to the installer, the PowerPoint ribbon button, the control panel, status messages and the installed `README.txt`. If no installer setting is found, the add-in follows the Windows display language.

The Russian labels are:

| English | Russian |
|---|---|
| Full screen / Window | Полный экран / Окно |
| Start file… | Запустить файл… |
| Refresh monitors | Обновить мониторы |
| Advance slides without timing after | Слайды без времени смены листать через |
| Do not advance | не листать |
| Restart the show when the file changes | Перезапускать показ, когда файл изменился |
| File, Monitor, Slide, Status, Settings | Файл, Монитор, Слайд, Статус, Настройки |
| running / restarting / failed, retry in N s | идёт / перезапуск / упал, повтор через N с |
| < Previous, Next > | < Назад, Вперёд > |
| Restart | Перезапустить |
| Stop, Stop all | Остановить, Остановить все |
| Esc stops the show | Esc в показе останавливает его |
| Select a show in the list. | Выберите показ в списке. |
| Could not start the show | Не удалось запустить |
| The previous show is still starting… | Предыдущий показ ещё запускается… |
| Next, Finish, Cancel, Retry | Далее, Готово, Отмена, Повторить |
| Update to version … or reinstall | Обновить до версии … или переустановить |
| Uninstall Poliekran | Удалить Полиэкран с этого компьютера |
| PowerPoint is running. Close it… | PowerPoint запущен. Закройте его… |

---

© 2026 Надежда "Umnokisa" Булочко. MIT License.
