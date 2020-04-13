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
  
  ;;;;; .MGSV
  
  ; Add .mgsv registry additions
  WriteRegStr HKCR ".mgsv" "" "MGSV Mod File"
  WriteRegStr HKCR ".mgsv" "Content Type" "application/x-zip-compressed"
  WriteRegStr HKCR ".mgsv" "DontCompressInPackage" ""
  WriteRegStr HKCR ".mgsv" "PerceivedType" "compressed"
  
  ; CLSID for CompressedFolder
  WriteRegStr HKCR ".mgsv\CLSID" "" "{E88DCCE0-B7B3-11d1-A9F0-00AA0060FA31}"
  
  ; .MGSV Icon
  WriteRegStr HKCR ".mgsv\DefaultIcon" "" "$INSTDIR\mgsvfile.ico"
  
  ; pointless?
  ;WriteRegExpandStr HKCR ".mgsv" "AppUserModelID" "Microsoft.Windows.Explorer"
  ;WriteRegDWORD HKCR ".mgsv" "EditFlags" 2097152
  ;WriteRegExpandStr HKCR ".mgsv" "FriendlyTypeName" "@%SystemRoot%\system32\zipfldr.dll,-10195"
  
  ; Finds explorer I guess?
  ;WriteRegStr HKCR ".mgsv\Shell\find" "" ""
  ;WriteRegStr HKCR ".mgsv\Shell\find" "LegacyDisable" ""
  ;WriteRegDWORD HKCR ".mgsv\Shell\find" "SuppressionPolicy" 128
  ;WriteRegExpandStr HKCR ".mgsv\Shell\find\command" "" "%SystemRoot%\Explorer.exe"
  ;WriteRegStr HKCR ".mgsv\Shell\find\command" "DelegateExecute" "{a015411a-f97d-4ef3-8425-8a38d022aebc}"
  
  ; Open on double-click
  WriteRegStr HKCR ".mgsv\Shell\Open" "MultiSelectModel" "Document"
  WriteRegExpandStr HKCR ".mgsv\Shell\Open\Command" "" "%SystemRoot%\Explorer.exe /idlist,%I,%L"
  WriteRegStr HKCR ".mgsv\Shell\Open\Command" "DelegateExecute" "{11dbb47c-a525-400b-9e80-a54615a090c0}"
  
  ; Adds 'Extract All...' to right-click context menu 
  WriteRegStr HKCR ".mgsv\ShellEx\ContextMenuHandlers\{b8cdcb65-b1bf-4b42-9428-1dfdb7ee92af}" "" "Compressed (zipped) Folder Menu"
  WriteRegStr HKCR ".mgsv\ShellEx\DropHandler" "" "{ed9d80b9-d157-457b-9192-0e7280313bf0}"
  WriteRegStr HKCR ".mgsv\ShellEx\StorageHandler" "" "{E88DCCE0-B7B3-11d1-A9F0-00AA0060FA31}"
  
  ;;;;;; .MGSVPRESET
  
  ; Add .mgsvpreset registry additions
  WriteRegStr HKCR ".mgsvpreset" "" "MGSV Mod Preset File"
  WriteRegStr HKCR ".mgsvpreset" "Content Type" "application/x-zip-compressed"
  WriteRegStr HKCR ".mgsvpreset" "DontCompressInPackage" ""
  WriteRegStr HKCR ".mgsvpreset" "PerceivedType" "compressed"
  
  ; CLSID for CompressedFolder
  WriteRegStr HKCR ".mgsvpreset\CLSID" "" "{E88DCCE0-B7B3-11d1-A9F0-00AA0060FA31}"
  
  ; .MGSVPreset Icon
  WriteRegStr HKCR ".mgsvpreset\DefaultIcon" "" "$INSTDIR\mgsvpreset.ico"
    
  ; Open on double-click
  WriteRegStr HKCR ".mgsvpreset\Shell\Open" "MultiSelectModel" "Document"
  WriteRegExpandStr HKCR ".mgsvpreset\Shell\Open\Command" "" "%SystemRoot%\Explorer.exe /idlist,%I,%L"
  WriteRegStr HKCR ".mgsvpreset\Shell\Open\Command" "DelegateExecute" "{11dbb47c-a525-400b-9e80-a54615a090c0}"
  
  ; Adds 'Extract All...' to right-click context menu 
  WriteRegStr HKCR ".mgsvpreset\ShellEx\ContextMenuHandlers\{b8cdcb65-b1bf-4b42-9428-1dfdb7ee92af}" "" "Compressed (zipped) Folder Menu"
  WriteRegStr HKCR ".mgsvpreset\ShellEx\DropHandler" "" "{ed9d80b9-d157-457b-9192-0e7280313bf0}"
  WriteRegStr HKCR ".mgsvpreset\ShellEx\StorageHandler" "" "{E88DCCE0-B7B3-11d1-A9F0-00AA0060FA31}"
  
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
  
  ; Delete .mgsv registry additions
  DeleteRegKey /ifempty HKCU ".mgsv"
  DeleteRegKey /ifempty HKCU ".mgsvpreset"

SectionEnd