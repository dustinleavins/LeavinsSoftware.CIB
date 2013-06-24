; Copyright (c) 2013 Dustin Leavins
; See the file 'LICENSE.txt' for copying permission.

; This file combines the normal Installer and
; the bootstrapper into one exe.

;Definitions
!define AppName "CIB Collection Manager"
!define CompanyName "Leavins Software"
!ifndef ProgramVersion
  !define ProgramVersion "0.1"
!endif

;General
  SilentInstall silent
  Name "${AppName} Installer"
  OutFile "CIB ${ProgramVersion}.exe"
  RequestExecutionLevel user

;Installer Sections
Section Main
  SetOutPath "$TEMP\${CompanyName}\${AppName}"
  File "setup.exe"
  File "main.exe"
  ExecWait '"$TEMP\${CompanyName}\${AppName}\setup.exe"'
  SetOutPath $TEMP
  RMDir /r "$TEMP\${CompanyName}"
SectionEnd

