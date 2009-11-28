!include "MUI2.nsh"
!include "checkDotNet3.nsh"

!define MIN_FRA_MAJOR "3"
!define MIN_FRA_MINOR "5"
!define MIN_FRA_BUILD "*"


; The name of the installer
Name "digiTweet Snarl Edition"

; The file to write
OutFile "Setup digiTweet Snarl Edition.exe"





; The default installation directory
InstallDir "$PROGRAMFILES\Tlhan Ghun\digiTweetSnarlEdition"

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM "Software\TlhanGhun\digiTweetSnarlEdition" "Install_Dir"

; Request application privileges for Windows Vista
RequestExecutionLevel admin


 


;--------------------------------

  !define MUI_ABORTWARNING



!define MUI_HEADERIMAGE
!define MUI_HEADERIMAGE_BITMAP "digiTweetSetupSmall.bmp"
!define MUI_WELCOMEFINISHPAGE_BITMAP "digiTweetSetupBig.bmp"
!define MUI_WELCOMEPAGE_TITLE "digiTweet Snarl Edition"
!define MUI_WELCOMEPAGE_TEXT "A Twitter client using the general notification system Snarl."
!define MUI_STARTMENUPAGE_DEFAULTFOLDER "Tlhan Ghun\digiTweetSnarlEdition"
!define MUI_ICON "Images\tray.ico"
!define MUI_UNICON "uninstall.ico"


Var StartMenuFolder
; Pages

  !insertmacro MUI_PAGE_WELCOME
  !insertmacro MUI_PAGE_LICENSE "License.txt"
  !insertmacro MUI_PAGE_COMPONENTS
  !insertmacro MUI_PAGE_DIRECTORY

  !define MUI_STARTMENUPAGE_REGISTRY_ROOT "HKCU" 
  !define MUI_STARTMENUPAGE_REGISTRY_KEY "Software\TlhanGhun\digiTweetSnarlEdition" 
  !define MUI_STARTMENUPAGE_REGISTRY_VALUENAME "Start Menu Folder"
  !insertmacro MUI_PAGE_STARTMENU Application $StartMenuFolder

  !insertmacro MUI_PAGE_INSTFILES
  !define MUI_FINISHPAGE_RUN "DigiTweet.exe"
  !insertmacro MUI_PAGE_FINISH




  !insertmacro MUI_UNPAGE_WELCOME
  !insertmacro MUI_UNPAGE_CONFIRM
  !insertmacro MUI_UNPAGE_INSTFILES
  !insertmacro MUI_UNPAGE_FINISH





;--------------------------------




!insertmacro MUI_LANGUAGE "English"

; LoadLanguageFile "${NSISDIR}\Contrib\Language files\English.nlf"
;--------------------------------
;Version Information

  VIProductVersion "1.0.0.0"
  VIAddVersionKey /LANG=${LANG_ENGLISH} "ProductName" "digiTweetSnarlEdition"
  VIAddVersionKey /LANG=${LANG_ENGLISH} "CompanyName" "Tlhan Ghun"
  VIAddVersionKey /LANG=${LANG_ENGLISH} "LegalCopyright" "© Tlhan Ghun GPL v.3"
  VIAddVersionKey /LANG=${LANG_ENGLISH} "FileDescription" "digiTweet Snarl Edition"
  VIAddVersionKey /LANG=${LANG_ENGLISH} "FileVersion" "1.0"







Function un.UninstallDirs
    Exch $R0 ;input string
    Exch
    Exch $R1 ;maximum number of dirs to check for
    Push $R2
    Push $R3
    Push $R4
    Push $R5
       IfFileExists "$R0\*.*" 0 +2
       RMDir "$R0"
     StrCpy $R5 0
    top:
     StrCpy $R2 0
     StrLen $R4 $R0
    loop:
     IntOp $R2 $R2 + 1
      StrCpy $R3 $R0 1 -$R2
     StrCmp $R2 $R4 exit
     StrCmp $R3 "\" 0 loop
      StrCpy $R0 $R0 -$R2
       IfFileExists "$R0\*.*" 0 +2
       RMDir "$R0"
     IntOp $R5 $R5 + 1
     StrCmp $R5 $R1 exit top
    exit:
    Pop $R5
    Pop $R4
    Pop $R3
    Pop $R2
    Pop $R1
    Pop $R0
FunctionEnd









; The stuff to install
Section "digiTweetSnarlEdition"

  SectionIn RO
  
  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  !insertmacro AbortIfBadFramework

  ; Put file there
  File "Blacklight.WPFControls.dll"
  File "Blacklight.WPFControls.pdb"
  File "ColorPicker.dll"
  File "DigiFlare.DigiTweet.DataAccess.dll"
  File "DigiFlare.DigiTweet.DataAccess.pdb"
  File "DigiFlare.DigiTweet.dll"
  File "DigiFlare.DigiTweet.pdb"
  File "DigiTweet.application"
  File "DigiTweet.exe"
  File "DigiTweet.exe.config"
  File "DigiTweet.exe.manifest"
  File "DigiTweet.pdb"
  File "LICENSE.txt"
  File "uninstall.ico"
  File "WPFToolkit.dll"

  SetOutPath "$INSTDIR\\Images"
  File "Images\\tray.ico"
  File "Images\\Logo.png"
  
  SetOutPath $INSTDIR
  
  ; Write the installation path into the registry
  WriteRegStr HKLM SOFTWARE\TlhanGhun\digiTweetSnarlEdition "Install_Dir" "$INSTDIR"
  
  ; Write the uninstall keys for Windows
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\digiTweetSnarlEdition" "DisplayName" "digiTweet Snarl Edition"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\digiTweetSnarlEdition" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\digiTweetSnarlEdition" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\digiTweetSnarlEdition" "NoRepair" 1
  WriteUninstaller "uninstall.exe"



  
SectionEnd

; Optional section (can be disabled by the user)
Section "Start Menu Shortcuts"

!insertmacro MUI_STARTMENU_WRITE_BEGIN Application

  CreateDirectory "$SMPROGRAMS\$StartMenuFolder"
  CreateShortCut "$SMPROGRAMS\$StartMenuFolder\\digiTweet.lnk" "$INSTDIR\DigiTweet.exe" "" "$INSTDIR\DigiTweet.exe" 0
;  CreateShortCut "$SMPROGRAMS\$StartMenuFolder\\Documentation.lnk" "$INSTDIR\Documentation.URL" "" $INSTDIR\Documentation.ico" 0
  CreateShortCut "$SMPROGRAMS\$StartMenuFolder\\Uninstall.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
  
!insertmacro MUI_STARTMENU_WRITE_END

  
SectionEnd


;--------------------------------

; Uninstaller

Section "Uninstall"

  
  ; Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\digiTweetSnarlEdition"
  DeleteRegKey HKLM "Software\TlhanGhun\digiTweetSnarlEdition"
  ; Remove files and uninstaller
  Delete $INSTDIR\*.*

  ; Remove shortcuts, if any
  !insertmacro MUI_STARTMENU_GETFOLDER Application $StartMenuFolder
    
  Delete "$SMPROGRAMS\$StartMenuFolder\\*.*"
  


  DeleteRegKey HKCU "Software\TlhanGhun\digiTweetSnarlEdition"


  ; Remove directories used
   ; RMDir "$SMPROGRAMS\$StartMenuFolder"
Push 10 #maximum amount of directories to remove
  Push "$SMPROGRAMS\$StartMenuFolder" #input string
    Call un.UninstallDirs

   
  ; RMDir "$INSTDIR"
  
  Push 10 #maximum amount of directories to remove
  Push $INSTDIR #input string
    Call un.UninstallDirs


SectionEnd
