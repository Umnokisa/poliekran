<a name="english"></a>

<p align="center"><img src="assets/icon.png" width="128" alt="Poliekran icon"></p>

# Poliekran

**[English](#english) | [Русский](#russian)**

A Microsoft PowerPoint add-in that runs several shows at once on different monitors of one PC. One screen can loop a presentation, the next one an electronic timetable, and each keeps running while someone works in other apps.

On its own, PowerPoint puts a full-screen show on one monitor and may close it when you click into another program. That rules it out for info screens, lobby displays and timetable boards, where neighboring monitors show different content all day.

## Features

- A different presentation on each monitor. The same file can also run on several screens.
- Two modes: «Полный экран» (a normal full-screen show) and «Окно» (PowerPoint's windowed show, also stretched over the whole monitor with the frame removed).
- A show is not closed or minimized when focus moves to another application.
- Shows loop. Slides without their own timing can advance automatically after 5–60 seconds, so a multi-page timetable keeps turning.
- When the file changes on disk, the show restarts with the new content. Edit the timetable, save it, and the screen updates by itself.
- If a show window closes on its own, the show starts again from the slide it was on.
- Control panel: list of shows with slide number and status, previous/next slide, restart, stop one or all.
- The mouse pointer stays visible over other windows while shows are running.
- Your original files are never modified: every presentation opens as a read-only copy.

## Installation

1. Download `Poliekran-<version>-setup.exe` from [Releases](../../releases/latest).
2. Close PowerPoint and run the installer. No administrator rights are needed.
3. Open PowerPoint. A «Полиэкран» button appears on the Slide Show tab.

The installer is not signed, so Windows SmartScreen may warn about an unknown publisher. Click More info → Run anyway.

**Updating.** Run the new installer. It notices the existing copy and offers to update it.

**Uninstalling.** Any of these works:

- run the installer again and choose «Удалить»;
- Windows 11: Settings → Apps → Installed apps → ⋯ next to Полиэкран → Uninstall; Windows 10: Settings → Apps → Apps & features;
- from a script: `Poliekran-<version>-setup.exe /uninstall /S`.

The uninstaller removes the files, the registry entries and the log.

## Usage

1. Slide Show → Полиэкран. The control panel opens.
2. Pick a monitor and a mode.
3. Choose how long slides without their own timing stay on screen. The default is 15 seconds, which suits info screens and timetables; for an ordinary talk with click-through slides pick «не листать». Also choose whether the show should restart when the file changes.
4. Click «Запустить файл…» and choose a file (.pptx, .pptm, .ppt, .ppsx, .ppsm, .pps).
5. Repeat for the other monitors.

The navigation, restart and stop buttons act on the shows selected in the list; with one show running, no selection is needed. After plugging in a monitor, click «Обновить мониторы».

Closing the control panel does not stop the shows; the ribbon button brings it back. Closing PowerPoint stops all of them.

Each show plays every slide in a loop with Presenter View off. The Slide Show settings saved in the file itself are ignored.

The interface is in Russian only for now.

## Electronic timetables

Keep the timetable as a regular PowerPoint file, locally or on a network share, and start it on the screen with «Перезапускать показ, когда файл изменился» ticked. The add-in checks the file every 5 seconds. Once a new version has been saved and stopped changing, the add-in opens it, and only then replaces the running show, usually within 10 seconds. If the new file cannot be opened yet, the old timetable stays on screen and the add-in tries again later. If the timetable now has fewer pages, the show starts from the first one.

The show plays a temporary copy of the file, so PowerPoint never locks the original: you can overwrite it from another computer while the show is running.

A timetable of several pages turns them over by the time chosen in the control panel. A one-page timetable just stays on screen. Do not protect the file with a password: PowerPoint would stop and ask for it.

## Stopping and restarts

Esc, the hyphen key, Ctrl+Break or End Show in the right-click menu stop a show for good. On screens nobody should touch, clear «Esc в показе останавливает его» in the control panel. A show closed from the keyboard then comes back within a few seconds, and only «Остановить» or «Остановить все» stop it.

If a show window disappears without a user command, the show starts again from the same slide. After five such restarts within a minute the add-in pauses that show: the list says «упал, повтор через N с», and after 60 seconds it tries again. «Перезапустить» retries at once. If PowerPoint itself crashes, all shows go down with it.

## Requirements

- Windows 10 or 11. Windows 7 SP1 and 8.1 with .NET Framework 4.6.2 should work but are untested.
- PowerPoint 2013 or later, including Microsoft 365, 32- or 64-bit (tested on 64-bit Office). PowerPoint 2010 may work but is untested.
- .NET Framework 4.6.2 or later. Windows 10 (since version 1607) and Windows 11 include it; the installer checks.

## Troubleshooting

**No button on the ribbon.** Go to File → Options → Add-ins. At the bottom, set Manage to COM Add-ins, click Go and tick Полиэкран. If it is not listed there, set Manage to Disabled Items, click Go, select Полиэкран, click Enable and restart PowerPoint. Do not run PowerPoint as administrator: an elevated PowerPoint may ignore per-user add-ins.

**A show behaves oddly.** The add-in writes a log to `%LOCALAPPDATA%\Poliekran\log.txt` (cleared once it passes 1 MB). It records which window was found, where it was placed and why a show restarted. Attach it to a new [issue](../../issues) together with your Windows and Office versions. The log contains full paths to your files, so remove anything private first.

## How it works

It is a classic COM add-in in C#, with no VSTO or Office PIAs. It reaches PowerPoint through late binding; everything to do with the show windows is plain Win32.

- PowerPoint starts the show. The add-in finds the new show window, moves it to the chosen monitor and stretches it over the whole screen, taking the monitor's DPI into account.
- The add-in replaces the window procedure of the show window (subclassing). Focus-loss messages never reach PowerPoint, and the window cannot be minimized, maximized, moved or closed with Alt+F4.
- A watchdog timer checks every half second that the window is alive and every two seconds puts it back if something moved it. Every five seconds it also looks at the file.

| File | Contents |
|---|---|
| `src/Connect.cs` | entry point (IDTExtensibility2) and the ribbon button |
| `src/ShowManager.cs` | starting shows, finding windows, watchdog, restarts, file reload |
| `src/Guard.cs` | show window message filter, pinning to the monitor |
| `src/Hook.cs` | catches Esc and right clicks inside the show |
| `src/ControlForm.cs` | control panel |
| `src/W.cs` | Win32 wrappers |
| `src/L.cs` | logging |
| `installer/setup.nsi` | NSIS installer |
| `assets/` | icon and installer artwork |
| `build.cmd` | local build |
| `.github/workflows/build.yml` | CI build and releases |

## Building

### On your machine (Windows)

You need the .NET SDK 8 or later and [NSIS 3](https://nsis.sourceforge.io/). Then run:

```bat
build.cmd 1.2.3
```

The script builds `out\Poliekran.dll` and `dist\Poliekran-1.2.3-setup.exe`. Without an argument it takes `<Version>` from `src/Poliekran.csproj`. The version must be three numbers. The project also opens in Visual Studio.

### On GitHub

`.github/workflows/build.yml` builds the DLL and the installer on every push to `main`/`master`, on pull requests and on manual runs. The result is attached to the run as an artifact.

To publish a release, push a tag:

```sh
git tag v1.2.3
git push origin v1.2.3
```

The tag must be `v` plus exactly three numbers. The workflow builds the installer for that version and attaches it to a new GitHub release with generated notes.

### What the installer does

It puts the DLL into `%LOCALAPPDATA%\Programs\Poliekran` and registers the `Poliekran.Connect` COM class under HKCU in both the 32-bit and 64-bit registry views. It then adds the add-in to `HKCU\Software\Microsoft\Office\PowerPoint\Addins` and to `DoNotDisableAddinList`, so Office does not disable it for slow loading. Silent mode (`/S`) never shows dialogs. Exit codes: 0 success, 3 PowerPoint is running, 4 no .NET Framework 4.6.2, 5 the user folder path contains # or %.

`AssemblyVersion` is fixed at 1.0.0.0 and repeated in `installer/setup.nsi` (`ASM` and the `InprocServer32\1.0.0.0` key). If you ever change it, change it there too. The product version is `<Version>`.

## Status

Early version. Several full-screen shows on different monitors work on the author's machine. Windowed mode is less reliable because PowerPoint resists resizing its own window.

Planned: the same presentation in sync on several monitors, spanning a show across a video wall, a ticker on top of a show, and starting a set of shows automatically at Windows startup.

Bugs and ideas go to [Issues](../../issues). Pull requests are welcome.

## License

[MIT](LICENSE) © 2026 Надежда "Umnokisa" Булочко

---

<a name="russian"></a>

<p align="center"><img src="assets/icon.png" width="128" alt="Иконка Полиэкрана"></p>

# Полиэкран

**[English](#english) | [Русский](#russian)**

Надстройка для Microsoft PowerPoint, которая запускает несколько показов одновременно на разных мониторах одного компьютера. На одном экране может крутиться презентация, на соседнем электронное расписание, и оба продолжают работать, пока за компьютером занимаются чем-то другим.

Сам PowerPoint выводит полноэкранный показ на один монитор и может закрыть его, если щёлкнуть в другую программу. Для информационных экранов, табло в холле и расписаний это не годится: там соседние мониторы весь день показывают разное.

## Возможности

- На каждом мониторе своя презентация. Один и тот же файл тоже можно запустить на нескольких экранах.
- Два режима: «Полный экран» (обычный полноэкранный показ) и «Окно» (оконный показ PowerPoint, который тоже растягивается на весь монитор без рамки).
- Показ не закрывается и не сворачивается, когда фокус уходит в другое приложение.
- Показ идёт по кругу. Слайды без своего времени смены можно листать автоматически через 5–60 секунд, чтобы многостраничное расписание не стояло на месте.
- Когда файл на диске меняется, показ перезапускается с новым содержимым. Поправили расписание, сохранили, и экран обновился сам.
- Если окно показа закрылось само, показ запускается заново с того же слайда.
- Пульт: список показов с номером слайда и статусом, листание, перезапуск, остановка одного или всех.
- Курсор мыши не пропадает над другими окнами, пока идут показы.
- Исходные файлы не меняются: каждая презентация открывается как копия только для чтения.

## Установка

1. Скачайте `Poliekran-<версия>-setup.exe` со страницы [Releases](../../releases/latest).
2. Закройте PowerPoint и запустите установщик. Права администратора не нужны.
3. Откройте PowerPoint: на вкладке «Слайд-шоу» появится кнопка «Полиэкран».

Установщик не подписан, поэтому Windows SmartScreen может предупредить о неизвестном издателе. Нажмите «Подробнее» → «Выполнить в любом случае».

**Обновление.** Запустите новый установщик. Он увидит установленную версию и предложит её обновить.

**Удаление.** Подойдёт любой способ:

- снова запустить установщик и выбрать «Удалить»;
- Windows 11: «Параметры» → «Приложения» → «Установленные приложения» → «⋯» рядом с «Полиэкран» → «Удалить»; Windows 10: «Параметры» → «Приложения» → «Приложения и возможности»;
- из скрипта: `Poliekran-<версия>-setup.exe /uninstall /S`.

При удалении стираются файлы, записи в реестре и журнал.

## Как пользоваться

1. «Слайд-шоу» → «Полиэкран». Откроется пульт.
2. Выберите монитор и режим.
3. Выберите, через сколько листать слайды без своего времени смены. По умолчанию 15 секунд: это подходит для информационных экранов и расписаний. Для обычного доклада, где слайды листают вручную, выберите «не листать». Там же решите, перезапускать ли показ, когда файл изменился.
4. Нажмите «Запустить файл…» и укажите файл (.pptx, .pptm, .ppt, .ppsx, .ppsm, .pps).
5. Повторите для остальных мониторов.

Кнопки листания, перезапуска и остановки действуют на показы, выделенные в списке. Если показ один, выделять его не нужно. Подключили монитор — нажмите «Обновить мониторы».

Если закрыть пульт, показы продолжат идти; вернуть его можно той же кнопкой на ленте. При закрытии PowerPoint останавливаются все показы.

Показ всегда идёт по кругу, со всеми слайдами, режим докладчика выключен. Настройки показа, сохранённые в самом файле, не учитываются.

## Электронное расписание

Храните расписание как обычный файл PowerPoint, на компьютере или в сетевой папке, и запустите его на экране с галочкой «Перезапускать показ, когда файл изменился». Надстройка проверяет файл раз в 5 секунд. Когда новая версия сохранена и перестала меняться, надстройка открывает её и только после этого заменяет идущий показ, обычно в пределах 10 секунд. Если новый файл пока не открывается, на экране остаётся старое расписание, а надстройка попробует позже. Если страниц стало меньше, показ начинается с первой.

Показ идёт по временной копии файла, поэтому PowerPoint не блокирует оригинал: его можно перезаписать с другого компьютера прямо во время показа.

Многостраничное расписание листается с интервалом, выбранным в пульте. Одностраничное просто стоит на экране. Не ставьте на файл пароль: PowerPoint остановится и будет его спрашивать.

## Остановка и перезапуски

Esc, «-», Ctrl+Break или «Завершить показ» в контекстном меню останавливают показ совсем. На экранах, которые никто не должен трогать, снимите в пульте галочку «Esc в показе останавливает его». Тогда показ, закрытый с клавиатуры, через несколько секунд запустится снова, а остановить его можно только кнопками «Остановить» и «Остановить все».

Если окно показа пропало без команды пользователя, показ запускается заново с того же слайда. После пяти таких перезапусков за минуту надстройка ставит показ на паузу: в списке появляется «упал, повтор через N с», и через 60 секунд она пробует снова. Кнопка «Перезапустить» пробует сразу. Если падает сам PowerPoint, вместе с ним пропадают все показы.

## Требования

- Windows 10 или 11. На Windows 7 SP1 и 8.1 с .NET Framework 4.6.2 должно работать, но не проверялось.
- PowerPoint 2013 и новее, включая Microsoft 365, 32- или 64-битный (проверялось на 64-битном Office). На PowerPoint 2010 может заработать, но не проверялось.
- .NET Framework 4.6.2 или новее. Он входит в Windows 10 начиная с версии 1607 и в Windows 11; установщик это проверяет.

## Если что-то не работает

**Кнопки нет на ленте.** Откройте «Файл» → «Параметры» → «Надстройки». Внизу в поле «Управление» выберите «Надстройки COM», нажмите «Перейти» и отметьте «Полиэкран». Если надстройки в этом списке нет, выберите в том же поле «Отключенные объекты», нажмите «Перейти», выделите «Полиэкран», нажмите «Включить» и перезапустите PowerPoint. Не запускайте PowerPoint от имени администратора: в таком режиме он может не видеть надстройки, установленные для пользователя.

**Показ ведёт себя странно.** Надстройка пишет журнал в `%LOCALAPPDATA%\Poliekran\log.txt` (он очищается, когда перерастает 1 МБ). В нём видно, какое окно нашлось, куда оно поставлено и почему показ перезапускался. Приложите журнал к сообщению в [Issues](../../issues) и укажите версии Windows и Office. В журнале есть полные пути к вашим файлам, так что перед отправкой уберите лишнее.

## Как это устроено

Это классическая COM-надстройка на C#, без VSTO и Office PIA. К PowerPoint она обращается через позднее связывание. Всё, что касается окон показа, сделано на чистом Win32.

- Показ запускает сам PowerPoint. Надстройка находит новое окно показа, переносит его на выбранный монитор и растягивает на весь экран с учётом масштаба этого монитора.
- Надстройка подменяет оконную процедуру окна показа (subclassing). Сообщения о потере фокуса до PowerPoint не доходят, а свернуть, развернуть, сдвинуть или закрыть окно через Alt+F4 нельзя.
- Сторожевой таймер раз в полсекунды проверяет, живо ли окно, а раз в две секунды возвращает его на место, если его сдвинули. Раз в пять секунд он заодно смотрит, не изменился ли файл.

| Файл | Что внутри |
|---|---|
| `src/Connect.cs` | точка входа (IDTExtensibility2) и кнопка на ленте |
| `src/ShowManager.cs` | запуск показов, поиск окна, сторож, перезапуск, обновление файла |
| `src/Guard.cs` | перехват сообщений окна показа, фиксация на мониторе |
| `src/Hook.cs` | ловит Esc и правый щелчок внутри показа |
| `src/ControlForm.cs` | пульт |
| `src/W.cs` | обёртки Win32 |
| `src/L.cs` | журнал |
| `installer/setup.nsi` | установщик NSIS |
| `assets/` | иконка и картинка для установщика |
| `build.cmd` | локальная сборка |
| `.github/workflows/build.yml` | сборка на GitHub и релизы |

## Сборка

### На своём компьютере (Windows)

Нужны .NET SDK 8 или новее и [NSIS 3](https://nsis.sourceforge.io/). Затем:

```bat
build.cmd 1.2.3
```

Скрипт соберёт `out\Poliekran.dll` и `dist\Poliekran-1.2.3-setup.exe`. Без аргумента версия берётся из `<Version>` в `src/Poliekran.csproj`. Версия должна состоять из трёх чисел. Проект также открывается в Visual Studio.

### На GitHub

`.github/workflows/build.yml` собирает DLL и установщик при пуше в `main`/`master`, на пул-реквестах и при ручном запуске. Результат прикрепляется к запуску как артефакт.

Чтобы выпустить релиз, отправьте тег:

```sh
git tag v1.2.3
git push origin v1.2.3
```

Тег должен состоять из `v` и ровно трёх чисел. Workflow соберёт установщик этой версии и прикрепит его к новому релизу на GitHub вместе с автоматически собранными заметками.

### Что делает установщик

Кладёт DLL в `%LOCALAPPDATA%\Programs\Poliekran` и регистрирует COM-класс `Poliekran.Connect` в HKCU, в 32- и 64-битном представлении реестра. Затем прописывает надстройку в `HKCU\Software\Microsoft\Office\PowerPoint\Addins` и вносит её в `DoNotDisableAddinList`, чтобы Office не отключал её за медленную загрузку. В тихом режиме (`/S`) никаких окон не появляется. Коды выхода: 0 — успех, 3 — запущен PowerPoint, 4 — нет .NET Framework 4.6.2, 5 — в пути к папке пользователя есть # или %.

`AssemblyVersion` закреплена на 1.0.0.0 и продублирована в `installer/setup.nsi` (`ASM` и ключ `InprocServer32\1.0.0.0`). Если меняете её, меняйте и там. Версия продукта задаётся в `<Version>`.

## Статус

Ранняя версия. Несколько полноэкранных показов на разных мониторах работают на компьютере автора. Оконный режим менее надёжен, потому что PowerPoint сопротивляется изменению размера своего окна.

Планы: синхронный показ одной презентации на нескольких мониторах, растягивание на видеостену, бегущая строка поверх показа, автоматический запуск набора показов при старте Windows.

Ошибки и идеи — в [Issues](../../issues). Пул-реквесты приветствуются.

## Лицензия

[MIT](LICENSE) © 2026 Надежда "Umnokisa" Булочко
