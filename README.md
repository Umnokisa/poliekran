<a name="english"></a>

<p align="center"><img src="For%20FUN%20%28code%29/assets/icon.png" width="128" alt="Poliekran icon"></p>

# Poliekran

**[English](#english) | [Русский](#russian)**

**User guide:** [English](docs/guide-en.md) ([PDF](docs/Poliekran-guide-en.pdf)) · [Русский](docs/guide-ru.md) ([PDF](docs/Poliekran-guide-ru.pdf))

A Microsoft PowerPoint add-in that runs several shows at once on different monitors of a single PC. One screen can loop a presentation and the next an electronic timetable, while both continue running when someone works in other apps.

By itself, PowerPoint puts a full-screen show on one monitor and may close it when you switch to another program. That makes it unsuitable for information screens, lobby displays and timetable boards, where neighboring monitors show different content throughout the day.

## Features

- A different presentation on each monitor. The same file can also run on several screens.
- Synchronous shows: choose one running show as the master and make other shows follow its slide changes. This is useful for different language versions on separate monitors.
- Two modes: «Full screen» (a normal full-screen show) and «Window» (PowerPoint's windowed show, stretched over the entire monitor with its frame removed).
- A show is not closed or minimized when focus moves to another application.
- Shows can loop continuously. Slides without their own timing can advance automatically after 5–60 seconds, so a multi-page timetable keeps moving.
- When the file changes on disk, the show restarts with the new content. Edit the timetable, save it, and the screen updates by itself.
- If a show window closes on its own, the show starts again from the slide it was on.
- Control panel with a list of shows, slide numbers and statuses, plus previous/next, restart and stop controls for one or all shows.
- The mouse pointer stays visible over other windows while shows are running.
- Your original files are never modified: every presentation opens as a read-only copy.

## Installation

1. Download `Poliekran-<version>-setup.exe` from [Releases](../../releases/latest).
2. Close PowerPoint and run the installer. No administrator rights are needed.
3. Open PowerPoint. A «Poliekran» button appears on the Slide Show tab.

The installer is not signed, so Windows SmartScreen may warn about an unknown publisher. Click More info → Run anyway to continue.

The file `Poliekran-1.4.0-setup.exe` is a Windows installer, not a portable standalone EXE. It installs the PowerPoint COM add-in for the current user, registers it in the per-user registry and copies the user guide. It does not require administrator rights. Close PowerPoint before installing or updating.

**Updating.** Run the new installer. It notices the existing copy and offers to update it.

**Uninstalling.** Any of these works:

- run the installer again and choose «Uninstall»;
- Windows 11: Settings → Apps → Installed apps → ⋯ next to Poliekran → Uninstall; Windows 10: Settings → Apps → Apps & features;
- from a script: `Poliekran-<version>-setup.exe /uninstall /S`.

The uninstaller removes the files, the registry entries and the log.

## Usage

1. Slide Show → Poliekran. The control panel opens.
2. Pick a monitor and a mode.
3. Choose how long slides without their own timing stay on screen. The default is 15 seconds, which suits information screens and timetables. For an ordinary talk with manually advanced slides, choose «do not advance». You can also choose whether the show should restart when the file changes.
4. Click «Start file…» and choose a file (.pptx, .pptm, .ppt, .ppsx, .ppsm, .pps).
5. Repeat for the other monitors.

To synchronize shows, start the master presentation first. Before starting each follower, choose the master in «Synchronize with». Followers use manual advance and follow the master's current slide; the master controls the group. A follower can have fewer slides: when the master's slide number does not exist, it shows its first slide.

The navigation, restart and stop buttons act on the shows selected in the list; when only one show is running, no selection is needed. After connecting a monitor, click «Refresh monitors».

Closing the control panel does not stop the shows; the ribbon button brings it back. Closing PowerPoint stops all of them.

Each show plays every slide in a loop with Presenter View off. The Slide Show settings saved in the file itself are ignored.

The interface can be English or Russian. Choose the language in the installer; the add-in uses the same language when PowerPoint starts.

## Electronic timetables

Keep the timetable as a regular PowerPoint file, locally or on a network share, and start it on the screen with «Restart the show when the file changes» selected. The add-in checks the file every 5 seconds. Once a new version has been saved and is no longer changing, the add-in opens it and only then replaces the running show, usually within 10 seconds. If the new file cannot be opened yet, the old timetable stays on screen and the add-in tries again later. If the timetable now has fewer pages, the show starts from the first one.

The show plays a temporary copy of the file, so PowerPoint never locks the original: you can overwrite it from another computer while the show is running.

A timetable of several pages turns them over by the time chosen in the control panel. A one-page timetable just stays on screen. Do not protect the file with a password: PowerPoint would stop and ask for it.

## Stopping and restarts

Esc, the hyphen (-) key, Ctrl+Break or End Show in the right-click menu stop a show completely. On screens nobody should touch, clear «Esc stops the show» in the control panel. A show closed with one of these keyboard commands then comes back within a few seconds; only «Stop» or «Stop all» stop it permanently.

If a show window disappears without a user command, the show starts again from the same slide. After five such restarts within a minute, the add-in pauses that show: the list says «упал, повтор через N с», and after 60 seconds it tries again. «Перезапустить» retries immediately. If PowerPoint itself crashes, all shows stop with it.

## Requirements

- Windows 10 or 11. Windows 7 SP1 and 8.1 with .NET Framework 4.6.2 should work but are untested.
- PowerPoint 2013 or later, including Microsoft 365, 32- or 64-bit (tested on 64-bit Office). PowerPoint 2010 may work but is untested.
- .NET Framework 4.6.2 or later. Windows 10 (since version 1607) and Windows 11 include it; the installer checks.

## Troubleshooting

**No button on the ribbon.** Go to File → Options → Add-ins. At the bottom, set Manage to COM Add-ins, click Go and tick Poliekran. If it is not listed there, set Manage to Disabled Items, click Go, select Poliekran, click Enable and restart PowerPoint. Do not run PowerPoint as administrator: an elevated PowerPoint may ignore per-user add-ins.

**A show behaves oddly.** The add-in writes a log to `%LOCALAPPDATA%\Poliekran\log.txt` (cleared once it passes 1 MB). It records which window was found, where it was placed and why a show restarted. Attach it to a new [issue](../../issues) together with your Windows and Office versions. The log contains full paths to your files, so remove anything private first.

## How it works

It is a classic COM add-in in C#, with no VSTO or Office PIAs. It reaches PowerPoint through late binding; everything related to the show windows uses the Win32 API directly.

- PowerPoint starts the show. The add-in finds the new show window, moves it to the chosen monitor and stretches it over the whole screen, taking the monitor's DPI into account.
- The add-in replaces the window procedure of the show window (subclassing). Focus-loss messages never reach PowerPoint, and the window cannot be minimized, maximized, moved or closed with Alt+F4.
- A watchdog timer checks every half second that the window is alive and every two seconds puts it back if something moved it. Every five seconds it also checks the file for changes.

| File | Contents |
|---|---|
| `For FUN (code)/src/Connect.cs` | entry point (IDTExtensibility2) and the ribbon button |
| `For FUN (code)/src/ShowManager.cs` | starting shows, finding windows, watchdog, restarts, file reload |
| `For FUN (code)/src/Guard.cs` | show window message filter, pinning to the monitor |
| `For FUN (code)/src/Hook.cs` | catches Esc and right clicks inside the show |
| `For FUN (code)/src/ControlForm.cs` | control panel |
| `For FUN (code)/src/W.cs` | Win32 wrappers |
| `For FUN (code)/src/L.cs` | logging |
| `For FUN (code)/installer/setup.nsi` | NSIS installer |
| `For FUN (code)/assets/` | icon and installer artwork |
| `docs/` | user guides (Markdown and PDF), `build-pdf.sh` to rebuild the PDFs |
| `For FUN (code)/build.cmd` | local build |
| `.github/workflows/build.yml` | CI build and releases |

## Building

### On your machine (Windows)

You need the .NET SDK 8 or later and [NSIS 3](https://nsis.sourceforge.io/). Then run:

```bat
For FUN (code)\build.cmd 1.2.3
```

The script builds `For FUN (code)\out\Poliekran.dll` and `For FUN (code)\dist\Poliekran-1.2.3-setup.exe`. The installer uses the guide PDFs already in `docs/`; after editing a guide, rebuild them with `docs/build-pdf.sh` (this requires pandoc and Chromium). Without an argument, the script takes `<Version>` from `For FUN (code)\src\Poliekran.csproj`. The version must contain three numbers. The project also opens in Visual Studio.

### For people who want to modify the code

The complete C# source code is included in the `For FUN (code)/` folder. You can change the add-in yourself—for example, adjust the control panel, show behavior or window handling—and build your own installer. The main implementation is in `For FUN (code)/src/ShowManager.cs`, the control panel is in `For FUN (code)/src/ControlForm.cs`, and the installer definition is in `For FUN (code)/installer/setup.nsi`.

After making a change, run `For FUN (code)\build.cmd 1.4.0` on Windows. The compiled DLL is written to `For FUN (code)\out\Poliekran.dll`, and the installer is written to `For FUN (code)\dist\Poliekran-1.4.0-setup.exe`. Use a new three-part version number for a release that you distribute. The GitHub workflow in `.github/workflows/build.yml` performs the same build automatically for pushes, pull requests and version tags.

### On GitHub

`.github/workflows/build.yml` builds the DLL and the installer on every push to `main`/`master`, on pull requests and on manual runs. Each run provides two separate artifacts: a Windows installer EXE and a source-code artifact with the folder tree and build files shown separately.

To publish a release, push a tag:

```sh
git tag v1.2.3
git push origin v1.2.3
```

The tag must be `v` plus exactly three numbers. The workflow builds the installer for that version and attaches only the EXE installer to a new GitHub release with generated notes. The source-code folder tree remains available in the workflow artifacts under `for funcode`.

### What the installer does

It puts the DLL into `%LOCALAPPDATA%\Programs\Poliekran` and registers the `Poliekran.Connect` COM class under HKCU in both the 32-bit and 64-bit registry views. It then adds the add-in to `HKCU\Software\Microsoft\Office\PowerPoint\Addins` and to `DoNotDisableAddinList`, so Office does not disable it because of slow loading. Silent mode (`/S`) never shows dialogs. Exit codes: 0 means success, 3 means PowerPoint is running, 4 means .NET Framework 4.6.2 is missing, and 5 means the user folder path contains # or %.

`AssemblyVersion` is fixed at 1.0.0.0 and repeated in `For FUN (code)/installer/setup.nsi` (`ASM` and the `InprocServer32\1.0.0.0` key). If you ever change it, change it there too. The product version is `<Version>`.

## Status

This is an early version. Several full-screen shows on different monitors have been tested on the author's machine. Windowed mode is less reliable because PowerPoint resists resizing its own window.

Planned: spanning a show across a video wall, a ticker on top of a show, and starting a set of shows automatically at Windows startup.

Bugs and ideas go to [Issues](../../issues). Pull requests are welcome.

## License

[MIT](LICENSE) © 2026 Надежда "Umnokisa" Булочко

---

<a name="russian"></a>

<p align="center"><img src="For%20FUN%20%28code%29/assets/icon.png" width="128" alt="Иконка Полиэкрана"></p>

# Полиэкран

**[English](#english) | [Русский](#russian)**

**Инструкция пользователя:** [русская](docs/guide-ru.md) ([PDF](docs/Poliekran-guide-ru.pdf)) · [английская](docs/guide-en.md) ([PDF](docs/Poliekran-guide-en.pdf))

Надстройка для Microsoft PowerPoint, которая запускает несколько показов одновременно на разных мониторах одного компьютера. На одном экране может непрерывно показываться презентация, на соседнем — электронное расписание, и оба продолжают работать, пока пользователь занимается другими делами.

Сам PowerPoint выводит полноэкранный показ на один монитор и может закрыть его при переключении на другую программу. Для информационных экранов, табло в холле и расписаний это неудобно: соседние мониторы должны весь день показывать разный контент.

## Возможности

- На каждом мониторе своя презентация. Один и тот же файл тоже можно запустить на нескольких экранах.
- Синхронные показы: один запущенный показ можно назначить ведущим, а остальные — ведомыми. Это удобно, например, для одновременного показа разных языковых версий на отдельных мониторах.
- Два режима: «Полный экран» (обычный полноэкранный показ) и «Окно» (оконный показ PowerPoint, растянутый на весь монитор без рамки).
- Показ не закрывается и не сворачивается, когда фокус уходит в другое приложение.
- Показ может идти по кругу. Слайды без собственного времени смены можно листать автоматически через 5–60 секунд, чтобы многостраничное расписание постоянно обновлялось.
- Когда файл на диске меняется, показ перезапускается с новым содержимым. После изменения и сохранения расписания экран обновляется автоматически.
- Если окно показа закрылось само, показ запускается заново с того же слайда.
- Панель управления: список показов с номерами слайдов и статусами, листание, перезапуск и остановка одного или всех показов.
- Курсор мыши не пропадает над другими окнами, пока идут показы.
- Исходные файлы не меняются: каждая презентация открывается как копия только для чтения.

## Установка

1. Скачайте `Poliekran-<версия>-setup.exe` со страницы [Releases](../../releases/latest).
2. Закройте PowerPoint и запустите установщик. Права администратора не нужны.
3. Откройте PowerPoint: на вкладке «Слайд-шоу» появится кнопка «Полиэкран».

Установщик не подписан, поэтому Windows SmartScreen может предупредить о неизвестном издателе. Нажмите «Подробнее» → «Выполнить в любом случае».

Файл `Poliekran-1.4.0-setup.exe` — это установщик Windows, а не переносимая самостоятельная программа. Он устанавливает COM-надстройку PowerPoint для текущего пользователя, регистрирует её в пользовательской части реестра и копирует инструкцию. Права администратора не нужны. Перед установкой или обновлением закройте PowerPoint.

**Обновление.** Запустите новый установщик. Он увидит установленную версию и предложит её обновить.

**Удаление.** Подойдёт любой способ:

- снова запустить установщик и выбрать «Удалить»;
- Windows 11: «Параметры» → «Приложения» → «Установленные приложения» → «⋯» рядом с «Полиэкран» → «Удалить»; Windows 10: «Параметры» → «Приложения» → «Приложения и возможности»;
- из скрипта: `Poliekran-<версия>-setup.exe /uninstall /S`.

При удалении стираются файлы, записи в реестре и журнал.

## Как пользоваться

1. «Слайд-шоу» → «Полиэкран». Откроется панель управления.
2. Выберите монитор и режим.
3. Выберите, через сколько листать слайды без собственного времени смены. По умолчанию задано 15 секунд: это подходит для информационных экранов и расписаний. Для обычного доклада, где слайды листают вручную, выберите «не листать». Там же решите, нужно ли перезапускать показ после изменения файла.
4. Нажмите «Запустить файл…» и укажите файл (.pptx, .pptm, .ppt, .ppsx, .ppsm, .pps).
5. Повторите для остальных мониторов.

Чтобы синхронизировать показы, сначала запустите ведущую презентацию. Перед запуском каждого ведомого выберите ведущий показ в поле «Синхронизировать с». Ведомые показы листаются вручную и следуют за текущим слайдом ведущего; ведущий управляет всей группой. Если у ведомой презентации меньше слайдов, при отсутствии номера ведущего она показывает первый слайд.

Кнопки листания, перезапуска и остановки действуют на показы, выделенные в списке. Если запущен только один показ, выделять его не нужно. После подключения монитора нажмите «Обновить мониторы».

Если закрыть панель управления, показы продолжат идти; открыть её снова можно той же кнопкой на ленте. При закрытии PowerPoint все показы останавливаются.

Показ всегда идёт по кругу, со всеми слайдами, режим докладчика выключен. Настройки показа, сохранённые в самом файле, не учитываются.

## Электронное расписание

Храните расписание как обычный файл PowerPoint, на компьютере или в сетевой папке, и запустите его на экране с включённой настройкой «Перезапускать показ, когда файл изменился». Надстройка проверяет файл раз в 5 секунд. Когда новая версия сохранена и перестала меняться, надстройка открывает её и только после этого заменяет идущий показ, обычно в пределах 10 секунд. Если новый файл пока не открывается, на экране остаётся старое расписание, а надстройка попробует позже. Если страниц стало меньше, показ начинается с первой.

Показ идёт по временной копии файла, поэтому PowerPoint не блокирует оригинал: его можно перезаписать с другого компьютера прямо во время показа.

Многостраничное расписание листается с интервалом, выбранным в панели управления. Одностраничное просто остаётся на экране. Не устанавливайте для файла пароль: PowerPoint остановит показ и запросит его.

## Остановка и перезапуски

Esc, «-», Ctrl+Break или «Завершить показ» в контекстном меню полностью останавливают показ. На экранах, которые никто не должен трогать, снимите в панели управления галочку «Esc в показе останавливает его». Тогда показ, закрытый одной из этих клавиш, через несколько секунд запустится снова, а остановить его окончательно можно только кнопками «Остановить» и «Остановить все».

Если окно показа пропало без команды пользователя, показ запускается заново с того же слайда. После пяти таких перезапусков за минуту надстройка ставит показ на паузу: в списке появляется «упал, повтор через N с», и через 60 секунд она пробует снова. Кнопка «Перезапустить» запускает новую попытку сразу. Если падает сам PowerPoint, вместе с ним останавливаются все показы.

## Требования

- Windows 10 или 11. На Windows 7 SP1 и 8.1 с .NET Framework 4.6.2 должно работать, но не проверялось.
- PowerPoint 2013 и новее, включая Microsoft 365, 32- или 64-битный (проверялось на 64-битном Office). На PowerPoint 2010 может заработать, но не проверялось.
- .NET Framework 4.6.2 или новее. Он входит в Windows 10 начиная с версии 1607 и в Windows 11; установщик это проверяет.

## Если что-то не работает

**Кнопки нет на ленте.** Откройте «Файл» → «Параметры» → «Надстройки». Внизу в поле «Управление» выберите «Надстройки COM», нажмите «Перейти» и отметьте «Полиэкран». Если надстройки в этом списке нет, выберите в том же поле «Отключенные объекты», нажмите «Перейти», выделите «Полиэкран», нажмите «Включить» и перезапустите PowerPoint. Не запускайте PowerPoint от имени администратора: в таком режиме он может не видеть надстройки, установленные для пользователя.

**Показ ведёт себя странно.** Надстройка пишет журнал в `%LOCALAPPDATA%\Poliekran\log.txt` (он очищается, когда перерастает 1 МБ). В нём видно, какое окно нашлось, куда оно поставлено и почему показ перезапускался. Приложите журнал к сообщению в [Issues](../../issues) и укажите версии Windows и Office. В журнале есть полные пути к вашим файлам, так что перед отправкой уберите лишнее.

## Как это устроено

Это классическая COM-надстройка на C#, без VSTO и Office PIA. К PowerPoint она обращается через позднее связывание. Всё, что касается окон показа, сделано непосредственно через Win32 API.

- Показ запускает сам PowerPoint. Надстройка находит новое окно показа, переносит его на выбранный монитор и растягивает на весь экран с учётом масштаба этого монитора.
- Надстройка подменяет оконную процедуру окна показа (subclassing). Сообщения о потере фокуса до PowerPoint не доходят, а свернуть, развернуть, сдвинуть или закрыть окно через Alt+F4 нельзя.
- Сторожевой таймер раз в полсекунды проверяет, живо ли окно, а раз в две секунды возвращает его на место, если его сдвинули. Раз в пять секунд он заодно смотрит, не изменился ли файл.

| Файл | Что внутри |
|---|---|
| `For FUN (code)/src/Connect.cs` | точка входа (IDTExtensibility2) и кнопка на ленте |
| `For FUN (code)/src/ShowManager.cs` | запуск показов, поиск окна, сторож, перезапуск, обновление файла |
| `For FUN (code)/src/Guard.cs` | перехват сообщений окна показа, фиксация на мониторе |
| `For FUN (code)/src/Hook.cs` | ловит Esc и правый щелчок внутри показа |
| `For FUN (code)/src/ControlForm.cs` | панель управления |
| `For FUN (code)/src/W.cs` | обёртки Win32 |
| `For FUN (code)/src/L.cs` | журнал |
| `For FUN (code)/installer/setup.nsi` | установщик NSIS |
| `For FUN (code)/assets/` | иконка и картинка для установщика |
| `docs/` | инструкции пользователя (Markdown и PDF), `build-pdf.sh` пересобирает PDF |
| `For FUN (code)/build.cmd` | локальная сборка |
| `.github/workflows/build.yml` | сборка на GitHub и релизы |

## Сборка

### На своём компьютере (Windows)

Нужны .NET SDK 8 или новее и [NSIS 3](https://nsis.sourceforge.io/). Затем:

```bat
For FUN (code)\build.cmd 1.2.3
```

Скрипт соберёт `For FUN (code)\out\Poliekran.dll` и `For FUN (code)\dist\Poliekran-1.2.3-setup.exe`. PDF-инструкции установщик берёт из `docs/` готовыми; после правки инструкции пересоберите их скриптом `docs/build-pdf.sh` (нужны pandoc и Chromium). Без аргумента версия берётся из `<Version>` в `For FUN (code)\src\Poliekran.csproj`. Версия должна состоять из трёх чисел. Проект также открывается в Visual Studio.

### Для тех, кто хочет доработать код

Полный исходный код на C# находится в папке `For FUN (code)/`. Его можно менять самостоятельно: например, настроить панель управления, поведение показов или работу с окнами, а затем собрать собственный установщик. Основная логика находится в `For FUN (code)/src/ShowManager.cs`, панель управления — в `For FUN (code)/src/ControlForm.cs`, а описание установщика — в `For FUN (code)/installer/setup.nsi`.

После изменений запустите в Windows `For FUN (code)\build.cmd 1.4.0`. Собранная DLL появится в `For FUN (code)\out\Poliekran.dll`, а установщик — в `For FUN (code)\dist\Poliekran-1.4.0-setup.exe`. Для распространяемого выпуска используйте новый номер версии из трёх чисел. Workflow в `.github/workflows/build.yml` выполняет ту же сборку автоматически при push, pull request и отправке тега версии.

### На GitHub

`.github/workflows/build.yml` собирает DLL и установщик при push в `main`/`master`, на pull request и при ручном запуске. Каждый запуск даёт два отдельных артефакта: установщик EXE для Windows и артефакт с исходным кодом, где дерево папок и файлы сборки видны отдельно.

Чтобы выпустить релиз, отправьте тег:

```sh
git tag v1.2.3
git push origin v1.2.3
```

Тег должен состоять из `v` и ровно трёх чисел. Workflow соберёт установщик этой версии и прикрепит только EXE к новому релизу на GitHub вместе с автоматически собранными заметками. Дерево исходного кода останется доступно среди артефактов запуска workflow в папке `for funcode`.

### Что делает установщик

Кладёт DLL в `%LOCALAPPDATA%\Programs\Poliekran` и регистрирует COM-класс `Poliekran.Connect` в HKCU, в 32- и 64-битном представлении реестра. Затем прописывает надстройку в `HKCU\Software\Microsoft\Office\PowerPoint\Addins` и вносит её в `DoNotDisableAddinList`, чтобы Office не отключал её за медленную загрузку. В тихом режиме (`/S`) никаких окон не появляется. Коды выхода: 0 — успех, 3 — запущен PowerPoint, 4 — нет .NET Framework 4.6.2, 5 — в пути к папке пользователя есть # или %.

`AssemblyVersion` закреплена на 1.0.0.0 и продублирована в `For FUN (code)/installer/setup.nsi` (`ASM` и ключ `InprocServer32\1.0.0.0`). Если меняете её, меняйте и там. Версия продукта задаётся в `<Version>`.

## Статус

Ранняя версия. Несколько полноэкранных показов на разных мониторах работают на компьютере автора. Оконный режим менее надёжен, потому что PowerPoint сопротивляется изменению размера своего окна.

Планы: растягивание показа на видеостену, бегущая строка поверх показа, автоматический запуск набора показов при старте Windows.

Ошибки и идеи — в [Issues](../../issues). Пул-реквесты приветствуются.

## Лицензия

[MIT](LICENSE) © 2026 Надежда "Umnokisa" Булочко
