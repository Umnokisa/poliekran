Unicode true
ManifestDPIAware true
SetCompressor /SOLID lzma
RequestExecutionLevel user

!include "MUI2.nsh"
!include "LogicLib.nsh"
!include "x64.nsh"
!include "FileFunc.nsh"
!include "nsDialogs.nsh"

!define NAME "Полиэкран"
!define AUTHOR "Надежда $\"Umnokisa$\" Булочко"
!ifndef VER
  !define VER "1.2.0"
!endif
!ifndef VER4
  !define VER4 "${VER}.0"
!endif
!define PROGID "Poliekran.Connect"
!define CLSID "{CFE2C7D5-5DBE-4027-903C-A3FB8B5B3890}"
!define ASM "Poliekran, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
!define UNKEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\Poliekran"
!define ADDKEY "Software\Microsoft\Office\PowerPoint\Addins\${PROGID}"
!define LOGDIR "$LOCALAPPDATA\Poliekran"

Name "${NAME}"
OutFile "..\dist\Poliekran-${VER}-setup.exe"
InstallDir "$LOCALAPPDATA\Programs\Poliekran"
BrandingText "${NAME} ${VER}"

VIProductVersion "${VER4}"
VIAddVersionKey /LANG=1049 "ProductName" "${NAME}"
VIAddVersionKey /LANG=1049 "FileDescription" "Установка ${NAME}"
VIAddVersionKey /LANG=1049 "FileVersion" "${VER}"
VIAddVersionKey /LANG=1049 "ProductVersion" "${VER}"
VIAddVersionKey /LANG=1049 "LegalCopyright" "© 2026 ${AUTHOR}, MIT"
VIAddVersionKey /LANG=1049 "CompanyName" "${AUTHOR}"

Var Old
Var RbUp
Var RbRm

!define MUI_ICON "..\assets\app.ico"
!define MUI_UNICON "..\assets\app.ico"
!define MUI_WELCOMEFINISHPAGE_BITMAP "..\assets\wizard.bmp"
!define MUI_ABORTWARNING
!define MUI_WELCOMEPAGE_TEXT "Мастер установит надстройку ${NAME} ${VER} для Microsoft PowerPoint.$\r$\n$\r$\nОна запускает разные презентации и электронные расписания на разных мониторах одного компьютера и не даёт PowerPoint закрывать показ, когда окно теряет фокус.$\r$\n$\r$\nЕсли PowerPoint открыт, закройте его."
!define MUI_FINISHPAGE_TEXT "${NAME} установлен.$\r$\n$\r$\nВ PowerPoint откройте вкладку «Слайд-шоу» и нажмите кнопку «Полиэкран».$\r$\n$\r$\nУдалить программу можно в «Приложениях» Windows или повторным запуском этого установщика."
!define MUI_FINISHPAGE_RUN
!define MUI_FINISHPAGE_RUN_TEXT "Запустить PowerPoint"
!define MUI_FINISHPAGE_RUN_FUNCTION RunPP
!define MUI_FINISHPAGE_SHOWREADME "$INSTDIR\Poliekran-guide-ru.pdf"
!define MUI_FINISHPAGE_SHOWREADME_TEXT "Открыть инструкцию"
!define MUI_FINISHPAGE_SHOWREADME_NOTCHECKED

!insertmacro MUI_PAGE_WELCOME
Page custom MaintShow MaintLeave
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_LANGUAGE "Russian"

; Waits until PowerPoint is closed and the add-in DLL is not locked.
!macro WaitPP DIR
  ${Do}
    StrCpy $9 ""
    FindWindow $0 "PPTFrameClass"
    ${If} $0 <> 0
      StrCpy $9 "PowerPoint запущен. Закройте его и нажмите «Повторить»."
    ${ElseIf} ${FileExists} "${DIR}\Poliekran.dll"
      ClearErrors
      FileOpen $8 "${DIR}\Poliekran.dll" a
      ${If} ${Errors}
        StrCpy $9 "PowerPoint закрыт, но ещё работает в фоне и держит надстройку. Подождите немного или завершите POWERPNT.EXE в диспетчере задач, затем нажмите «Повторить»."
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
      MessageBox MB_OK|MB_ICONINFORMATION "${NAME} не установлен." /SD IDOK
      SetErrorLevel 0
      Quit
    ${EndIf}
    !insertmacro WaitPP "$Old"
    !insertmacro Remove "$Old"
    MessageBox MB_OK|MB_ICONINFORMATION "${NAME} удалён." /SD IDOK
    SetErrorLevel 0
    Quit
  ${EndIf}

  ReadRegDWORD $0 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full" "Release"
  ${If} $0 < 394802
    MessageBox MB_OK|MB_ICONSTOP "Нужен .NET Framework 4.6.2 или новее. В Windows 10 и 11 он уже есть. Для Windows 7 и 8.1 его нужно скачать с сайта Microsoft и установить." /SD IDOK
    SetErrorLevel 4
    Abort
  ${EndIf}

  StrCpy $0 $INSTDIR
  Call BadChars
  ${If} $0 = 1
    MessageBox MB_OK|MB_ICONSTOP "Путь к папке пользователя содержит символ # или %:$\r$\n$INSTDIR$\r$\n$\r$\nPowerPoint не сможет загрузить надстройку из такой папки." /SD IDOK
    SetErrorLevel 5
    Abort
  ${EndIf}
FunctionEnd

Function MaintShow
  ${If} $Old == ""
    Abort
  ${EndIf}
  !insertmacro MUI_HEADER_TEXT "${NAME} уже установлен" "Выберите, что сделать."
  nsDialogs::Create 1018
  Pop $0
  ${NSD_CreateLabel} 0 0 100% 24u "Надстройка уже стоит в папке:$\r$\n$Old"
  Pop $0
  ${NSD_CreateRadioButton} 0 34u 100% 12u "Обновить до версии ${VER} или переустановить"
  Pop $RbUp
  ${NSD_CreateRadioButton} 0 52u 100% 12u "Удалить ${NAME} с этого компьютера"
  Pop $RbRm
  ${NSD_Check} $RbUp
  nsDialogs::Show
FunctionEnd

Function MaintLeave
  ${NSD_GetState} $RbRm $0
  ${If} $0 == ${BST_CHECKED}
    !insertmacro WaitPP "$Old"
    !insertmacro Remove "$Old"
    MessageBox MB_OK|MB_ICONINFORMATION "${NAME} удалён."
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
    Delete "$Old\Poliekran-guide-ru.pdf"
    Delete "$Old\Poliekran-guide-en.pdf"
    Delete "$Old\LICENSE.txt"
    Delete "$Old\uninstall.exe"
    RMDir "$Old"
  ${EndIf}

  SetOutPath "$INSTDIR"
  File "..\out\Poliekran.dll"
  File /oname=Poliekran.ico "..\assets\app.ico"
  File "README.txt"
  File "..\docs\Poliekran-guide-ru.pdf"
  File "..\docs\Poliekran-guide-en.pdf"
  File /oname=LICENSE.txt "..\LICENSE"
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

  WriteRegStr HKCU "${ADDKEY}" "FriendlyName" "${NAME}"
  WriteRegStr HKCU "${ADDKEY}" "Description" "Разные презентации и расписания на разных мониторах"
  WriteRegDWORD HKCU "${ADDKEY}" "LoadBehavior" 3
  WriteRegDWORD HKCU "Software\Microsoft\Office\16.0\PowerPoint\Resiliency\DoNotDisableAddinList" "${PROGID}" 1
  WriteRegDWORD HKCU "Software\Microsoft\Office\15.0\PowerPoint\Resiliency\DoNotDisableAddinList" "${PROGID}" 1

  WriteRegStr HKCU "${UNKEY}" "DisplayName" "${NAME}"
  WriteRegStr HKCU "${UNKEY}" "DisplayVersion" "${VER}"
  WriteRegStr HKCU "${UNKEY}" "Publisher" "${AUTHOR}"
  WriteRegStr HKCU "${UNKEY}" "InstallLocation" "$INSTDIR"
  WriteRegStr HKCU "${UNKEY}" "DisplayIcon" "$INSTDIR\Poliekran.ico"
  WriteRegStr HKCU "${UNKEY}" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegStr HKCU "${UNKEY}" "QuietUninstallString" '"$INSTDIR\uninstall.exe" /S'
  WriteRegDWORD HKCU "${UNKEY}" "NoModify" 1
  WriteRegDWORD HKCU "${UNKEY}" "NoRepair" 1
  ${GetSize} "$INSTDIR" "/S=0K" $0 $1 $2
  WriteRegDWORD HKCU "${UNKEY}" "EstimatedSize" $0
SectionEnd

Section "Uninstall"
  !insertmacro WaitPP "$INSTDIR"
  !insertmacro Remove "$INSTDIR"
SectionEnd
