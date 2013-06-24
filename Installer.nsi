; Copyright (c) 2013 Dustin Leavins
; See the file 'LICENSE.txt' for copying permission.

; Main installer for CIB Collection Manager

;Definitions
!define AppName "CIB Collection Manager"
!define CompanyName "Leavins Software"
!define UninstallRegSubkey "Software\Microsoft\Windows\CurrentVersion\Uninstall\${AppName}"

!ifndef ProgramVersion
  !define ProgramVersion "0.1"
!endif

!include "MUI2.nsh"

;General
  Name "${AppName}"
  OutFile "main.exe"
  InstallDir "$LOCALAPPDATA\${CompanyName}\${AppName}"
  InstallDirRegKey HKCU "Software\${CompanyName}\${AppName}" ""
  RequestExecutionLevel user

;Interface Settings

  !define MUI_ABORTWARNING

;Pages

  !insertmacro MUI_PAGE_LICENSE "License.txt"
  !insertmacro MUI_PAGE_COMPONENTS
  !insertmacro MUI_PAGE_DIRECTORY
  !insertmacro MUI_PAGE_INSTFILES
  
  !insertmacro MUI_UNPAGE_CONFIRM
  !insertmacro MUI_UNPAGE_INSTFILES
  
;Languages
 
  !insertmacro MUI_LANGUAGE "English"

;Installer Sections
Section "Main Program" SecDummy
  SetOutPath "$INSTDIR"
  File /r "Files\*.*"
  WriteRegStr HKCU "Software\${CompanyName}\${AppName}" "" $INSTDIR
  WriteUninstaller "$INSTDIR\Uninstall.exe"
  CreateShortcut "$STARTMENU\Programs\${AppName}.lnk" "$INSTDIR\app.exe" "" "$INSTDIR\icon2.ico"
  CreateShortcut "$DESKTOP\${AppName}.lnk" "$INSTDIR\app.exe" "" "$INSTDIR\icon2.ico"

  WriteRegStr HKCU "${UninstallRegSubkey}" "DisplayName" "${AppName}"
  WriteRegStr HKCU "${UninstallRegSubkey}" "DisplayIcon" "$\"$INSTDIR\icon2.ico$\""
  WriteRegStr HKCU "${UninstallRegSubkey}" "Publisher" "${CompanyName}"
  WriteRegStr HKCU "${UninstallRegSubkey}" "DisplayVersion" "${ProgramVersion}"
  WriteRegStr HKCU "${UninstallRegSubkey}" "InstallLocation" "$\"$INSTDIR$\""
  WriteRegStr HKCU "${UninstallRegSubkey}" "InstallSource" "$\"$INSTDIR$\""
  WriteRegDWord HKCU "${UninstallRegSubkey}" "NoModify" 1
  WriteRegDWord HKCU "${UninstallRegSubkey}" "NoRepair" 1
  WriteRegStr HKCU "${UninstallRegSubkey}" "UninstallString" "$\"$INSTDIR\Uninstall.exe$\""
  WriteRegStr HKCU "${UninstallRegSubkey}" "Comments" "Uninstalls ${AppName}."
SectionEnd

;Descriptions

  LangString DESC_SecDummy ${LANG_ENGLISH} "Test Description"

  !insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
    !insertmacro MUI_DESCRIPTION_TEXT ${SecDummy} $(DESC_SecDummy)
  !insertmacro MUI_FUNCTION_DESCRIPTION_END

;Uninstaller Section

Section "Uninstall"
  RMDir /REBOOTOK /r "$INSTDIR"
  DeleteRegKey /ifempty HKCU "Software\${CompanyName}\${AppName}"
  DeleteRegKey /ifempty HKCU "Software\${CompanyName}"
  DeleteRegKey HKCU "${UninstallRegSubkey}"
  Delete "$STARTMENU\Programs\${AppName}.lnk"
  Delete "$DESKTOP\${AppName}.lnk"
SectionEnd

