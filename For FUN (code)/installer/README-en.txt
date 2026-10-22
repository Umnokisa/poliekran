Poliekran

PowerPoint add-in for running different presentations and electronic timetables
on different monitors of one computer. Shows remain open when PowerPoint loses focus.

Detailed guide: Poliekran-guide-en.pdf in this folder.
Russian guide: Poliekran-guide-ru.pdf.

How to use
1. PowerPoint -> Slide Show tab -> Poliekran button. The control panel opens.
2. Choose a monitor and a mode (full screen or window).
3. Choose how long slides without their own timing should stay on screen,
   and whether to restart the show when the file changes.
4. Click Start file… and choose a presentation.
5. Repeat for the other monitors.

The show loops continuously. When a timetable file is saved again, the add-in
opens the new version automatically, usually within 10 seconds. The show uses
a temporary copy, so the original file can be overwritten while it is running.
By default, slides without their own timing advance every 15 seconds.
For an ordinary talk, choose Do not advance.

Esc, the hyphen (-) key, Ctrl+Break or End Show in the show window stop it.
For unattended screens, clear Esc stops the show: then the show can only be
stopped with Stop or Stop all.

If the show window closes on its own, the show restarts from the same slide.
After five restarts within a minute it receives the failed status and tries
again after 60 seconds. Restart tries immediately.

If the button is missing
File -> Options -> Add-ins -> Manage: COM Add-ins -> Go.
Tick Poliekran. If it is not listed: Manage: Disabled Items -> Go -> select
Poliekran -> Enable. Do not run PowerPoint as administrator.

Log
%LOCALAPPDATA%\Poliekran\log.txt (paste this path into File Explorer).

Uninstall
Run the installer again and choose Uninstall,
or use Windows Settings -> Apps -> Poliekran -> Uninstall.

Copyright © 2026 Nadezhda "Umnokisa" Bulochko. MIT license, see LICENSE.txt.
