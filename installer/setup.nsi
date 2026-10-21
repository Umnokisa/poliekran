Unicode true
ManifestDPIAware true
SetCompressor /SOLID lzma
RequestExecutionLevel user

!include "MUI2.nsh"
!include "LogicLib.nsh"
!include "x64.nsh"
!include "FileFunc.nsh"
!include "nsDialogs.nsh"

!ifndef VER
  !define VER "1.4.0"
!endif
!ifndef VER4
  !define VER4 "${VER}.0"
!endif

LangString NAME 1033 "Poliekran"
LangString NAME 1049 "Полиэкран"
LangString AUTHOR 1033 "Nadezhda $\"Umnokisa$\" Bulochko"
LangString AUTHOR 1049 "Надежда $\"Umnokisa$\" Булочко"
LangString DESCRIPTION 1033 "Different presentations and timetables on different monitors"
LangString DESCRIPTION 1049 "Разные презентации и расписания на разных мониторах"
LangString WELCOME_TEXT 1033 "This wizard will install the Poliekran ${VER} add-in for Microsoft PowerPoint.$\r$\n$\r$\nIt runs different presentations and electronic timetables on different monitors of one computer and keeps the show running when its window loses focus.$\r$\n$\r$\nIf PowerPoint is open, close it first."
LangString WELCOME_TEXT 1049 "Мастер установит надстройку Полиэкран ${VER} для Microsoft PowerPoint.$\r$\n$\r$\nОна запускает разные презентации и электронные расписания на разных мониторах одного компьютера и не даёт PowerPoint закрывать показ, когда окно теряет фокус.$\r$\n$\r$\nЕсли PowerPoint открыт, закройте его."
LangString FINISH_TEXT 1033 "Poliekran has been installed.$\r$\n$\r$\nIn PowerPoint, open the Slide Show tab and click the Poliekran button.$\r$\n$\r$\nYou can uninstall the program from Windows Settings or by running this installer again."
LangString FINISH_TEXT 1049 "Полиэкран установлен.$\r$\n$\r$\nВ PowerPoint откройте вкладку «Слайд-шоу» и нажмите кнопку «Полиэкран».$\r$\n$\r$\nУдалить программу можно в «Приложениях» Windows или повторным запуском этого установщика."
LangString RUN_PP 1033 "Launch PowerPoint"
LangString RUN_PP 1049 "Запустить PowerPoint"
LangString OPEN_GUIDE 1033 "Open the user guide"
LangString OPEN_GUIDE 1049 "Открыть инструкцию"
LangString README_FILE 1033 "Poliekran-guide-en.pdf"
LangString README_FILE 1049 "Poliekran-guide-ru.pdf"
LangString ALREADY_INSTALLED 1033 "Poliekran is already installed"
LangString ALREADY_INSTALLED 1049 "Полиэкран уже установлен"
LangString CHOOSE_ACTION 1033 "Choose what to do."
LangString CHOOSE_ACTION 1049 "Выберите, что сделать."
LangString INSTALLED_AT 1033 "The add-in is installed in:$\r$\n$Old"
LangString INSTALLED_AT 1049 "Надстройка уже стоит в папке:$\r$\n$Old"
LangString UPDATE_OPTION 1033 "Update to version ${VER} or reinstall"
LangString UPDATE_OPTION 1049 "Обновить до версии ${VER} или переустановить"
LangString REMOVE_OPTION 1033 "Uninstall Poliekran"
LangString REMOVE_OPTION 1049 "Удалить Полиэкран с этого компьютера"
LangString NOT_INSTALLED 1033 "Poliekran is not installed."
LangString NOT_INSTALLED 1049 "Полиэкран не установлен."
LangString UNINSTALLED 1033 "Poliekran has been uninstalled."
LangString UNINSTALLED 1049 "Полиэкран удалён."
LangString NET_REQUIRED 1033 ".NET Framework 4.6.2 or later is required. It is already included in Windows 10 and 11. On Windows 7 and 8.1, download and install it from Microsoft."
LangString NET_REQUIRED 1049 "Нужен .NET Framework 4.6.2 или новее. В Windows 10 и 11 он уже есть. Для Windows 7 и 8.1 его нужно скачать с сайта Microsoft и установить."
LangString BAD_PATH 1033 "The user folder path contains # or %:$\r$\n$INSTDIR$\r$\n$\r$\nPowerPoint cannot load the add-in from a folder with these characters."
LangString BAD_PATH 1049 "Путь к папке пользователя содержит символ # или %:$\r$\n$INSTDIR$\r$\n$\r$\nPowerPoint не сможет загрузить надстройку из такой папки."
LangString WAIT_POWERPOINT 1033 "PowerPoint is running. Close it and click Retry."
LangString WAIT_POWERPOINT 1049 "PowerPoint запущен. Закройте его и нажмите «Повторить»."
LangString WAIT_LOCKED 1033 "PowerPoint is closed, but it is still running in the background and has locked the add-in. Wait a little or end POWERPNT.EXE in Task Manager, then click Retry."
LangString WAIT_LOCKED 1049 "PowerPoint закрыт, но ещё работает в фоне и держит надстройку. Подождите немного или завершите POWERPNT.EXE в диспетчере задач, затем нажмите «Повторить»."
!define PROGID "Poliekran.Connect"
!define CLSID "{CFE2C7D5-5DBE-4027-903C-A3FB8B5B3890}"
!define ASM "Poliekran, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
!define UNKEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\Poliekran"
!define ADDKEY "Software\Microsoft\Office\PowerPoint\Addins\${PROGID}"
!define LOGDIR "$LOCALAPPDATA\Poliekran"

Name "$(NAME)"
OutFile "..\dist\Poliekran-${VER}-setup.exe"
InstallDir "$LOCALAPPDATA\Programs\Poliekran"
BrandingText "$(NAME) ${VER}"

VIProductVersion "${VER4}"
VIAddVersionKey /LANG=1033 "ProductName" "Poliekran"
VIAddVersionKey /LANG=1033 "FileDescription" "Poliekran setup"
VIAddVersionKey /LANG=1049 "ProductName" "Полиэкран"
VIAddVersionKey /LANG=1049 "FileDescription" "Установка Полиэкрана"
VIAddVersionKey /LANG=1033 "FileVersion" "${VER}"
VIAddVersionKey /LANG=1033 "ProductVersion" "${VER}"
VIAddVersionKey /LANG=1049 "FileVersion" "${VER}"
VIAddVersionKey /LANG=1049 "ProductVersion" "${VER}"
VIAddVersionKey /LANG=1033 "LegalCopyright" "Copyright © 2026 Nadezhda $\"Umnokisa$\" Bulochko, MIT"
VIAddVersionKey /LANG=1033 "CompanyName" "Nadezhda $\"Umnokisa$\" Bulochko"
VIAddVersionKey /LANG=1049 "LegalCopyright" "© 2026 Надежда $\"Umnokisa$\" Булочко, MIT"
VIAddVersionKey /LANG=1049 "CompanyName" "Надежда $\"Umnokisa$\" Булочко"

Var Old
Var RbUp
Var RbRm

!define MUI_ICON "..\assets\app.ico"
!define MUI_UNICON "..\assets\app.ico"
!define MUI_WELCOMEFINISHPAGE_BITMAP "..\assets\wizard.bmp"
!define MUI_ABORTWARNING
!define MUI_WELCOMEPAGE_TEXT "$(WELCOME_TEXT)"
!define MUI_FINISHPAGE_TEXT "$(FINISH_TEXT)"
!define MUI_FINISHPAGE_RUN
!define MUI_FINISHPAGE_RUN_TEXT "$(RUN_PP)"
!define MUI_FINISHPAGE_RUN_FUNCTION RunPP
!define MUI_FINISHPAGE_SHOWREADME "$INSTDIR\$(README_FILE)"
!define MUI_FINISHPAGE_SHOWREADME_TEXT "$(OPEN_GUIDE)"
!define MUI_FINISHPAGE_SHOWREADME_NOTCHECKED

!insertmacro MUI_PAGE_WELCOME
Page custom MaintShow MaintLeave
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_LANGUAGE "English"
!insertmacro MUI_LANGUAGE "Russian"

; Waits until PowerPoint is closed and the add-in DLL is not locked.
!macro WaitPP DIR
  ${Do}
    StrCpy $9 ""
    FindWindow $0 "PPTFrameClass"
    ${If} $0 <> 0
      StrCpy $9 "$(WAIT_POWERPOINT)"
    ${ElseIf} ${FileExists} "${DIR}\Poliekran.dll"
      ClearErrors
      FileOpen $8 "${DIR}\Poliekran.dll" a
      ${If} ${Errors}
        StrCpy $9 "$(WAIT_LOCKED)"
      ${Else}
        FileClose $8
      ${EndIf}
    ${EndIf}
    ${If} $9 == ""
      ${Break}
    ${EndIf}
    MessageBox MB_RETRYCANCEL|MB_ICONEXCLAMATION "$9" /SD IDCANCEL IDRETRY +3
    SetErrorLevel 3
    Abort
  ${Loop}
!macroend

!macro RegCom
  WriteRegStr HKCU "Software\Classes\${PROGID}" "" "${PROGID}"
  WriteRegStr HKCU "Software\Classes\${PROGID}\CLSID" "" "${CLSID}"
  WriteRegStr HKCU "Software\Classes\CLSID\${CLSID}" "" "${PROGID}"
  WriteRegStr HKCU "Software\Classes\CLSID\${CLSID}\ProgId" "" "${PROGID}"
  WriteRegStr HKCU "Software\Classes\CLSID\${CLSID}\Implemented Categories\{62C8FE65-4EBB-45e7-B440-6E39B2CDBF29}" "" ""
  WriteRegStr HKCU "Software\Classes\CLSID\${CLSID}\InprocServer32" "" "mscoree.dll"
  WriteRegStr HKCU "Software\Classes\CLSID\${CLSID}\InprocServer32" "ThreadingModel" "Both"
  WriteRegStr HKCU "Software\Classes\CLSID\${CLSID}\InprocServer32" "Class" "Poliekran.Connect"
  WriteRegStr HKCU "Software\Classes\CLSID\${CLSID}\InprocServer32" "Assembly" "${ASM}"
  WriteRegStr HKCU "Software\Classes\CLSID\${CLSID}\InprocServer32" "RuntimeVersion" "v4.0.30319"
  WriteRegStr HKCU "Software\Classes\CLSID\${CLSID}\InprocServer32" "CodeBase" "$1"
  WriteRegStr HKCU "Software\Classes\CLSID\${CLSID}\InprocServer32\1.0.0.0" "Class" "Poliekran.Connect"
  WriteRegStr HKCU "Software\Classes\CLSID\${CLSID}\InprocServer32\1.0.0.0" "Assembly" "${ASM}"
  WriteRegStr HKCU "Software\Classes\CLSID\${CLSID}\InprocServer32\1.0.0.0" "RuntimeVersion" "v4.0.30319"
  WriteRegStr HKCU "Software\Classes\CLSID\${CLSID}\InprocServer32\1.0.0.0" "CodeBase" "$1"
!macroend

!macro UnregCom
  DeleteRegKey HKCU "Software\Classes\${PROGID}"
  DeleteRegKey HKCU "Software\Classes\CLSID\${CLSID}"
!macroend

; Removes everything the installer created. DIR is the install folder.
!macro Remove DIR
  SetRegView 32
  !insertmacro UnregCom
  ${If} ${RunningX64}
    SetRegView 64
    !insertmacro UnregCom
    SetRegView 32
  ${EndIf}
  DeleteRegKey HKCU "${ADDKEY}"
  DeleteRegValue HKCU "Software\Microsoft\Office\16.0\PowerPoint\Resiliency\DoNotDisableAddinList" "${PROGID}"
  DeleteRegValue HKCU "Software\Microsoft\Office\15.0\PowerPoint\Resiliency\DoNotDisableAddinList" "${PROGID}"
  DeleteRegValue HKCU "Software\Microsoft\Office\16.0\PowerPoint\AddInLoadTimes" "${PROGID}"
  DeleteRegValue HKCU "Software\Microsoft\Office\15.0\PowerPoint\AddInLoadTimes" "${PROGID}"
  DeleteRegKey HKCU "${UNKEY}"
  Delete /REBOOTOK "${DIR}\Poliekran.dll"
  Delete "${DIR}\Poliekran.ico"
  Delete "${DIR}\README.txt"
  Delete "${DIR}\README-en.txt"
  Delete "${DIR}\README-ru.txt"
  Delete "${DIR}\Poliekran-guide-ru.pdf"
  Delete "${DIR}\Poliekran-guide-en.pdf"
  Delete "${DIR}\LICENSE.txt"
  Delete "${DIR}\uninstall.exe"
  RMDir "${DIR}"
  Delete "${LOGDIR}\log.txt"
  RMDir "${LOGDIR}"
!macroend

Function RunPP
  ExecShell "open" "powerpnt.exe"
FunctionEnd

Function FileUri
  StrCpy $1 "file:///"
  StrCpy $2 0
  ${Do}
    StrCpy $3 $0 1 $2
    ${If} $3 == ""
      ${Break}
    ${EndIf}
    ${If} $3 == "\"
      StrCpy $3 "/"
    ${EndIf}
    StrCpy $1 "$1$3"
    IntOp $2 $2 + 1
  ${Loop}
FunctionEnd

; $0 = path; returns $0 = 1 if it contains # or %
Function BadChars
  StrCpy $2 0
  StrCpy $3 $0
  StrCpy $0 0
  ${Do}
    StrCpy $4 $3 1 $2
    ${If} $4 == ""
      ${Break}
    ${EndIf}
    ${If} $4 == "#"
    ${OrIf} $4 == "%"
      StrCpy $0 1
      ${Break}
    ${EndIf}
    IntOp $2 $2 + 1
  ${Loop}
FunctionEnd

Function .onInit
  !insertmacro MUI_LANGDLL_DISPLAY
  ReadRegStr $Old HKCU "${UNKEY}" "InstallLocation"
  ${IfNot} ${FileExists} "$Old\Poliekran.dll"
  ${AndIfNot} ${FileExists} "$Old\uninstall.exe"
    StrCpy $Old ""
  ${EndIf}

  ${GetParameters} $R0
  ClearErrors
  ${GetOptions} $R0 "/uninstall" $R1
  ${IfNot} ${Errors}
    ${If} $Old == ""
      MessageBox MB_OK|MB_ICONINFORMATION "$(NOT_INSTALLED)" /SD IDOK
      SetErrorLevel 0
      Quit
    ${EndIf}
    !insertmacro WaitPP "$Old"
    !insertmacro Remove "$Old"
    MessageBox MB_OK|MB_ICONINFORMATION "$(UNINSTALLED)" /SD IDOK
    SetErrorLevel 0
    Quit
  ${EndIf}

  ReadRegDWORD $0 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full" "Release"
  ${If} $0 < 394802
    MessageBox MB_OK|MB_ICONSTOP "$(NET_REQUIRED)" /SD IDOK
    SetErrorLevel 4
    Abort
  ${EndIf}

  StrCpy $0 $INSTDIR
  Call BadChars
  ${If} $0 = 1
    MessageBox MB_OK|MB_ICONSTOP "$(BAD_PATH)" /SD IDOK
    SetErrorLevel 5
    Abort
  ${EndIf}
FunctionEnd

Function MaintShow
  ${If} $Old == ""
    Abort
  ${EndIf}
  !insertmacro MUI_HEADER_TEXT "$(ALREADY_INSTALLED)" "$(CHOOSE_ACTION)"
  nsDialogs::Create 1018
  Pop $0
  ${NSD_CreateLabel} 0 0 100% 24u "$(INSTALLED_AT)"
  Pop $0
  ${NSD_CreateRadioButton} 0 34u 100% 12u "$(UPDATE_OPTION)"
  Pop $RbUp
  ${NSD_CreateRadioButton} 0 52u 100% 12u "$(REMOVE_OPTION)"
  Pop $RbRm
  ${NSD_Check} $RbUp
  nsDialogs::Show
FunctionEnd

Function MaintLeave
  ${NSD_GetState} $RbRm $0
  ${If} $0 == ${BST_CHECKED}
    !insertmacro WaitPP "$Old"
    !insertmacro Remove "$Old"
    MessageBox MB_OK|MB_ICONINFORMATION "$(UNINSTALLED)"
    SetErrorLevel 0
    Quit
  ${EndIf}
FunctionEnd

Section "Install"
  SetErrorLevel 0
  !insertmacro WaitPP "$INSTDIR"

  ${If} $Old != ""
  ${AndIf} $Old != $INSTDIR
    Delete "$Old\Poliekran.dll"
    Delete "$Old\Poliekran.ico"
    Delete "$Old\README.txt"
    Delete "$Old\README-en.txt"
    Delete "$Old\README-ru.txt"
    Delete "$Old\Poliekran-guide-ru.pdf"
    Delete "$Old\Poliekran-guide-en.pdf"
    Delete "$Old\LICENSE.txt"
    Delete "$Old\uninstall.exe"
    RMDir "$Old"
  ${EndIf}

  SetOutPath "$INSTDIR"
  File "..\out\Poliekran.dll"
  File /oname=Poliekran.ico "..\assets\app.ico"
  File /oname=README-en.txt "README-en.txt"
  File /oname=README-ru.txt "README.txt"
  File "..\docs\Poliekran-guide-ru.pdf"
  File "..\docs\Poliekran-guide-en.pdf"
  File /oname=LICENSE.txt "..\LICENSE"
  ${If} $LANGUAGE == 1033
    Delete "$INSTDIR\README-ru.txt"
    Rename "$INSTDIR\README-en.txt" "$INSTDIR\README.txt"
  ${Else}
    Delete "$INSTDIR\README-en.txt"
    Rename "$INSTDIR\README-ru.txt" "$INSTDIR\README.txt"
  ${EndIf}
  WriteUninstaller "$INSTDIR\uninstall.exe"

  StrCpy $0 "$INSTDIR\Poliekran.dll"
  Call FileUri

  SetRegView 32
  !insertmacro UnregCom
  !insertmacro RegCom
  ${If} ${RunningX64}
    SetRegView 64
    !insertmacro UnregCom
    !insertmacro RegCom
    SetRegView 32
  ${EndIf}

  WriteRegStr HKCU "${ADDKEY}" "FriendlyName" "$(NAME)"
  WriteRegStr HKCU "${ADDKEY}" "Description" "$(DESCRIPTION)"
  WriteRegDWORD HKCU "${ADDKEY}" "LoadBehavior" 3
  WriteRegDWORD HKCU "Software\Microsoft\Office\16.0\PowerPoint\Resiliency\DoNotDisableAddinList" "${PROGID}" 1
  WriteRegDWORD HKCU "Software\Microsoft\Office\15.0\PowerPoint\Resiliency\DoNotDisableAddinList" "${PROGID}" 1

  WriteRegStr HKCU "${UNKEY}" "DisplayName" "$(NAME)"
  WriteRegStr HKCU "${UNKEY}" "DisplayVersion" "${VER}"
  WriteRegStr HKCU "${UNKEY}" "Publisher" "$(AUTHOR)"
  WriteRegStr HKCU "${UNKEY}" "InstallLocation" "$INSTDIR"
  WriteRegStr HKCU "${UNKEY}" "DisplayIcon" "$INSTDIR\Poliekran.ico"
  WriteRegStr HKCU "${UNKEY}" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegStr HKCU "${UNKEY}" "QuietUninstallString" '"$INSTDIR\uninstall.exe" /S'
  ${If} $LANGUAGE == 1033
    WriteRegStr HKCU "${UNKEY}" "Language" "en"
  ${Else}
    WriteRegStr HKCU "${UNKEY}" "Language" "ru"
  ${EndIf}
  WriteRegDWORD HKCU "${UNKEY}" "NoModify" 1
  WriteRegDWORD HKCU "${UNKEY}" "NoRepair" 1
  ${GetSize} "$INSTDIR" "/S=0K" $0 $1 $2
  WriteRegDWORD HKCU "${UNKEY}" "EstimatedSize" $0
SectionEnd

Section "Uninstall"
  !insertmacro WaitPP "$INSTDIR"
  !insertmacro Remove "$INSTDIR"
SectionEnd
