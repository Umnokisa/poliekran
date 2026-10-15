# Poliekran

**[English](#english) | [Русский](#russian)**

<a name="english"></a>

A Microsoft PowerPoint add-in that runs several slideshows at the same time on different monitors of one PC. Each show stays on its own screen and keeps running while you work in other apps.

Out of the box, PowerPoint runs only one full-screen show and may close it when you switch to another app. Poliekran is built for info screens, lobby displays, video walls and any setup where neighboring monitors have to show different slides.

## Features

- Different presentations on different monitors. The same file can also run on several screens.
- Full-screen and windowed modes.
- A show is not closed or minimized when focus moves to another application.
- If a show crashes anyway, it restarts from the same slide.
- Control panel: list of shows, next/previous slide, restart, stop.
- The mouse pointer stays visible over other windows while shows are running.
- Pressing Esc in a show stops it. For unattended screens you can turn this off, and then only the Stop button in the control panel stops a show.
- Your original files are never modified: every presentation opens as a read-only copy.

## Installation

1. Download `Poliekran-<version>-setup.exe` from the Releases page.
2. Close PowerPoint and run the installer. No administrator rights are needed.
3. Open PowerPoint. A "Полиэкран" button appears on the Slide Show tab.

The installer is not signed, so Windows SmartScreen may warn about an unknown publisher. Click "More info" → "Run anyway".

To uninstall: Windows Settings → Apps → Полиэкран → Uninstall.

## Usage

1. Slide Show → Полиэкран.
2. Pick a monitor and a mode, click "Запустить файл…" (Open file) and choose a presentation.
3. Repeat for the other monitors.

Slide timings are respected and shows loop.

The interface is currently in Russian only.

## Requirements

- Windows 7 SP1 or later.
- Microsoft PowerPoint 2010 or later, including Microsoft 365. Both 32-bit and 64-bit Office are supported (tested on 64-bit).
- .NET Framework 4.6.2 or later. Windows 10 and 11 already include it.

## Troubleshooting

**No button on the ribbon.** File → Options → Add-ins, at the bottom choose "Manage: COM Add-ins" → Go, and tick "Полиэкран". If the add-in is listed under Disabled Items, enable it there.

**A show behaves oddly.** The add-in writes a log to `%LOCALAPPDATA%\Poliekran\log.txt`. Attach it to your issue: it shows which window was found, where it was placed and why a show was restarted.

## How it works

Poliekran is a classic COM add-in written in C#, without VSTO or the Office PIAs. It talks to PowerPoint through late binding and handles the show windows through Win32.

- PowerPoint itself starts the show. The add-in then finds the show window, moves it to the chosen monitor and stretches it to the monitor bounds.
- The show window is subclassed: focus-loss messages never reach PowerPoint, and minimizing or moving the window is blocked.
- A watchdog timer checks every half second that the window is still alive and puts it back in place. If the window disappears without a user command, the show starts again from the last slide.

| File | Contents |
|---|---|
| `src/Connect.cs` | entry point (IDTExtensibility2) and the ribbon button |
| `src/ShowManager.cs` | starting shows, finding windows, watchdog, restarts |
| `src/Guard.cs` | show window message filter, pinning to the monitor |
| `src/ControlForm.cs` | control panel |
| `src/W.cs` | Win32 wrappers |
| `src/L.cs` | logging |
| `installer/setup.nsi` | NSIS installer |

## Building

DLL:

```
dotnet build src -c Release -o out
```

Requires .NET SDK 6 or later. The project also opens in Visual Studio.

Installer (NSIS 3):

```
makensis -INPUTCHARSET UTF8 installer\setup.nsi
```

Output: `dist\Poliekran-<version>-setup.exe`.

The installer puts the DLL into `%LOCALAPPDATA%\Programs\Poliekran` and registers the `Poliekran.Connect` COM class under HKCU in both the 32-bit and 64-bit registry views. It then adds the add-in to `HKCU\Software\Microsoft\Office\PowerPoint\Addins` and to `DoNotDisableAddinList`, so Office does not disable it for slow loading.

## Status

Early version. The main scenario (several full-screen shows on different monitors) works. Windowed mode is less reliable because PowerPoint resists resizing its own window.

Planned: the same presentation in sync on several monitors, spanning a show across a video wall, a ticker on top of a show, and launching a set of shows from the command line at Windows startup.

---


<a name="russian"></a>

# Полиэкран

**[English](#english) | [Русский](#russian)**

Надстройка для Microsoft PowerPoint, которая запускает несколько презентаций одновременно на разных мониторах одного компьютера. Каждый показ остаётся на своём экране и продолжает работать, пока вы заняты чем-то другим.

Обычно PowerPoint держит один полноэкранный показ, а если переключиться в другую программу, может его закрыть. «Полиэкран» решает это для информационных экранов, стоек в холле, видеостен и любых мест, где на соседних мониторах должны крутиться разные слайды.

## Возможности

- Разные презентации на разных мониторах. Один и тот же файл тоже можно запустить на нескольких экранах.
- Полноэкранный и оконный режим.
- Показ не закрывается и не сворачивается, когда фокус уходит в другое приложение.
- Если показ всё-таки упал, он перезапускается с того же слайда.
- Пульт управления: список показов, листание вперёд и назад, перезапуск, остановка.
- Курсор не пропадает над другими окнами, пока идут показы.
- Esc в показе останавливает его. Для экранов без присмотра это можно отключить, и тогда показ остановит только кнопка в пульте.
- Оригинальные файлы не меняются: каждая презентация открывается как копия только для чтения.

## Установка

1. Скачайте `Poliekran-<версия>-setup.exe` со страницы релизов.
2. Закройте PowerPoint и запустите установщик. Права администратора не нужны.
3. Откройте PowerPoint: на вкладке «Слайд-шоу» появится кнопка «Полиэкран».

Установщик не подписан, поэтому Windows SmartScreen может предупредить о неизвестном издателе. Нажмите «Подробнее» → «Выполнить в любом случае».

Удаление: «Параметры Windows → Приложения → Полиэкран → Удалить».

## Как пользоваться

1. «Слайд-шоу» → «Полиэкран».
2. Выберите монитор и режим, нажмите «Запустить файл…» и укажите презентацию.
3. Повторите для остальных мониторов.

Тайминги слайдов работают, показ зациклен.

## Требования

- Windows 7 SP1 и новее.
- Microsoft PowerPoint 2010 и новее, включая Microsoft 365. Подходит и 32-, и 64-битный Office (проверялось на 64-битном).
- .NET Framework 4.6.2 или новее. В Windows 10 и 11 он уже установлен.

## Если что-то не работает

**Кнопки нет на ленте.** «Файл → Параметры → Надстройки», внизу «Управление: Надстройки COM» → «Перейти», отметьте «Полиэкран». Если надстройка попала в «Отключённые объекты», включите её там.

**Показ ведёт себя странно.** Надстройка пишет журнал в `%LOCALAPPDATA%\Poliekran\log.txt`. Приложите его к issue: по нему видно, какое окно нашлось, куда оно поставлено и почему показ перезапускался.

## Как это устроено

Это классическая COM-надстройка на C#, без VSTO и без Office PIA. С PowerPoint она общается через позднее связывание, а с окнами показа работает через Win32.

- Показ запускается средствами PowerPoint. Затем надстройка находит его окно, переносит на выбранный монитор и растягивает по его границам.
- Окно показа подклассируется: сообщения о потере активности до PowerPoint не доходят, а сворачивание и перемещение окна блокируются.
- Сторожевой таймер раз в полсекунды проверяет, живо ли окно, и возвращает его на место. Если окно пропало без команды пользователя, показ запускается заново с последнего слайда.

| Файл | Что внутри |
|---|---|
| `src/Connect.cs` | точка входа (IDTExtensibility2) и кнопка на ленте |
| `src/ShowManager.cs` | запуск показов, поиск окна, сторож, перезапуск |
| `src/Guard.cs` | перехват сообщений окна показа, фиксация на мониторе |
| `src/ControlForm.cs` | пульт управления |
| `src/W.cs` | обёртки Win32 |
| `src/L.cs` | журнал |
| `installer/setup.nsi` | установщик NSIS |

## Сборка

DLL:

```
dotnet build src -c Release -o out
```

Нужен .NET SDK 6 или новее. Проект также открывается в Visual Studio.

Установщик (NSIS 3):

```
makensis -INPUTCHARSET UTF8 installer\setup.nsi
```

Результат: `dist\Poliekran-<версия>-setup.exe`.

Установщик кладёт DLL в `%LOCALAPPDATA%\Programs\Poliekran` и регистрирует COM-класс `Poliekran.Connect` в HKCU, в 32- и 64-битном представлении реестра. Затем он прописывает надстройку в `HKCU\Software\Microsoft\Office\PowerPoint\Addins` и вносит её в `DoNotDisableAddinList`, чтобы Office не отключал её за медленную загрузку.

## Статус

Ранняя версия. Основной сценарий (несколько полноэкранных показов на разных мониторах) работает. Оконный режим менее надёжен, потому что PowerPoint сопротивляется изменению размера своего окна.

Планы: синхронный показ одной презентации на нескольких мониторах, растягивание на видеостену, бегущая строка поверх показа, запуск набора показов из командной строки при старте Windows.
