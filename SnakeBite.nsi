;NSIS Modern User Interface
;Basic Install Script

;--------------------------------
;Include Modern UI

  !include "MUI2.nsh"

;--------------------------------
;General

  ;Name and file
  Name "SnakeBite"
  OutFile "SnakeBite Installer.exe"

  ;Default installation folder
  InstallDir "$LOCALAPPDATA\SnakeBite"
  
  ;Get installation folder from registry if available
  InstallDirRegKey HKCU "Software\SnakeBite" ""

  ;Request application privileges for Windows Vista
  RequestExecutionLevel user

;--------------------------------
;Variables

  Var StartMenuFolder

;--------------------------------
;Interface Settings

  !define MUI_ABORTWARNING

;--------------------------------
;Pages

  !insertmacro MUI_PAGE_LICENSE "license.txt"
  !insertmacro MUI_PAGE_COMPONENTS
  !insertmacro MUI_PAGE_DIRECTORY

  ;Start Menu Folder Page Configuration
  !define MUI_STARTMENUPAGE_REGISTRY_ROOT "HKCU" 
  !define MUI_STARTMENUPAGE_REGISTRY_KEY "Software\SnakeBite" 
  !define MUI_STARTMENUPAGE_REGISTRY_VALUENAME "Start Menu Folder"
    
  !define SHCNE_ASSOCCHANGED 0x08000000
  !define SHCNF_IDLIST 0
 
  Function RefreshShellIcons
    ; By jerome tremblay - april 2003
    System::Call 'shell32.dll::SHChangeNotify(i, i, i, i) v \
    (${SHCNE_ASSOCCHANGED}, ${SHCNF_IDLIST}, 0, 0)'
  FunctionEnd
  
  !insertmacro MUI_PAGE_STARTMENU Application $StartMenuFolder

  !insertmacro MUI_PAGE_INSTFILES
  
  !insertmacro MUI_UNPAGE_CONFIRM
  !insertmacro MUI_UNPAGE_INSTFILES
  
;--------------------------------
;Languages
 
  !insertmacro MUI_LANGUAGE "English"

;--------------------------------
;Installer Sections

Section "SnakeBite" SecMain

  SetOutPath "$INSTDIR"
  
  ;ADD YOUR OWN FILES HERE...
  File "SnakeBite.exe"
  File "MakeBite.exe"
  File "CityHash.dll"
  File "GzsTool.Core.dll"
  File "ICSharpCode.SharpZipLib.dll"
  File "Zlib.Portable.dll"
  File "fpk_dictionary.txt"
  File "qar_dictionary.txt"
  File "README.md"
  File "ChangeLog.txt"
  File "mgsvfile.ico"
  File "mgsvpreset.ico"
  
  ;Store installation folder
  WriteRegStr HKCU "Software\SnakeBite" "" $INSTDIR
  
  ;Add .mgsv registry additions
  WriteRegStr HKCR ".mgsv" "" "mgsv_auto_file"
  WriteRegStr HKCR "mgsv_auto_file" "" ""
  WriteRegStr HKCR "mgsv_auto_file" "ContentType" "application/x-zip-compressed"
  WriteRegStr HKCR "mgsv_auto_file" "PercievedType" "compressed"
  WriteRegStr HKCR "mgsv_auto_file\OpenWithProgids" "" ""
  WriteRegStr HKCR "mgsv_auto_file\OpenWithProgids" "CompressedFolder" ""
  WriteRegDWORD HKCR "mgsv_auto_file" "EditFlags" 0
  WriteRegDWORD HKCR "mgsv_auto_file" "BrowserFlags" 8
  WriteRegStr HKCR "mgsv_auto_file\DefaultIcon" "" "$INSTDIR\mgsvfile.ico"
  
  ;Add .mgsv registry additions
  WriteRegStr HKCR ".mgsvpreset" "" "mgsvpreset_auto_file"
  WriteRegStr HKCR "mgsvpreset_auto_file" "" ""
  WriteRegStr HKCR "mgsvpreset_auto_file" "ContentType" "application/x-zip-compressed"
  WriteRegStr HKCR "mgsvpreset_auto_file" "PercievedType" "compressed"
  WriteRegStr HKCR "mgsvpreset_auto_file\OpenWithProgids" "" ""
  WriteRegStr HKCR "mgsvpreset_auto_file\OpenWithProgids" "CompressedFolder" ""
  WriteRegDWORD HKCR "mgsvpreset_auto_file" "EditFlags" 0
  WriteRegDWORD HKCR "mgsvpreset_auto_file" "BrowserFlags" 8
  WriteRegStr HKCR "mgsvpreset_auto_file\DefaultIcon" "" "$INSTDIR\mgsvpreset.ico"
  
  ;Refresh icons
  Call RefreshShellIcons
  
  ;Create uninstaller
  WriteUninstaller "$INSTDIR\Uninstall.exe"

  !insertmacro MUI_STARTMENU_WRITE_BEGIN Application
	
	;Create shortcuts
	CreateDirectory "$SMPROGRAMS\$StartMenuFolder"
	CreateShortcut "$SMPROGRAMS\$StartMenuFolder\SnakeBite.lnk" "$INSTDIR\SnakeBite.exe"
	CreateShortcut "$SMPROGRAMS\$StartMenuFolder\MakeBite.lnk" "$INSTDIR\MakeBite.exe"
	CreateShortcut "$SMPROGRAMS\$StartMenuFolder\Uninstall.lnk" "$INSTDIR\Uninstall.exe"
	CreateShortCut "$DESKTOP\SnakeBite.lnk" "$INSTDIR\SnakeBite.exe"
  
  !insertmacro MUI_STARTMENU_WRITE_END  

SectionEnd

;--------------------------------
;Descriptions

  ;Language strings
  LangString DESC_SecMain ${LANG_ENGLISH} "The main components required for SnakeBite."

  ;Assign language strings to sections
  !insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
	!insertmacro MUI_DESCRIPTION_TEXT ${SecMain} $(DESC_SecMain)
  !insertmacro MUI_FUNCTION_DESCRIPTION_END

;--------------------------------
;Uninstaller Section

Section "Uninstall"

  ;ADD YOUR OWN FILES HERE...
  Delete "$INSTDIR\SnakeBite.exe"
  Delete "$INSTDIR\MakeBite.exe"
  Delete "$INSTDIR\CityHash.dll"
  Delete "$INSTDIR\GzsTool.Core.dll"
  Delete "$INSTDIR\ICSharpCode.SharpZipLib.dll"
  Delete "$INSTDIR\Zlib.Portable.dll"
  Delete "$INSTDIR\fpk_dictionary.txt"
  Delete "$INSTDIR\qar_dictionary.txt"
  Delete "$INSTDIR\sbupdater.exe"

  Delete "$INSTDIR\Uninstall.exe"

  RMDir "$INSTDIR"

 !insertmacro MUI_STARTMENU_GETFOLDER Application $StartMenuFolder
  Delete "$DESKTOP\SnakeBite.lnk"
  Delete "$DESKTOP\MakeBite.lnk"
  Delete "$SMPROGRAMS\$StartMenuFolder\SnakeBite.lnk"
  Delete "$SMPROGRAMS\$StartMenuFolder\Uninstall.lnk"
  RMDir "$SMPROGRAMS\$StartMenuFolder"

  DeleteRegKey /ifempty HKCU "Software\SnakeBite"

SectionEnd