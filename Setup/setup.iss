; DebloaterTool Online Installer
; Non-commercial use only
#include "C:\Program Files (x86)\Inno Download Plugin\idp.iss"

#define MyAppName "DebloaterTool"
#define MyAppVersion "1.0"
#define MyAppPublisher "megsystem"
#define MyAppURL "https://github.com/megsystem"
#define MyAppExeName "DebloaterTool.exe"
#define MyAppIcon "DebloaterTool.ico" ; <-- Use your icon file here

[Setup]
AppId={{84C3FD7D-2128-40E9-90BF-F9054235C7C3}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}

DefaultDirName={autopf}\DebloaterTool
DefaultGroupName=DebloaterTool
AllowNoIcons=yes
OutputBaseFilename=DebloaterToolSetup
Compression=lzma
SolidCompression=yes
WizardStyle=modern dark windows11
PrivilegesRequired=admin

; --- Set Control Panel icon ---
UninstallDisplayIcon={app}\{#MyAppIcon}

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
; Include the ICO in the installer
Source: "{#MyAppIcon}"; DestDir: "{app}"; Flags: ignoreversion

; Include README.txt in the installer
Source: "README.txt"; DestDir: "{app}"; Flags: ignoreversion

; Download the EXE at runtime using Inno Download Plugin
Source: "{tmp}\{#MyAppExeName}"; DestDir: "{app}"; Flags: external ignoreversion

[Icons]
Name: "{group}\DebloaterTool"; Filename: "{app}\{#MyAppExeName}"
Name: "{commondesktop}\DebloaterTool"; Filename: "{app}\{#MyAppExeName}"

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Launch DebloaterTool"; Flags: nowait postinstall skipifsilent

[Code]

var
  ReadmePage: TWizardPage;
  ReadmeMemo: TMemo;

procedure InitializeWizard;
var
  TempReadme: string;
begin
  ExtractTemporaryFile('README.txt'); 
  TempReadme := ExpandConstant('{tmp}\README.txt');

  ReadmePage := CreateCustomPage(wpWelcome, 'README / Information', 'Please read the following before continuing:');
  ReadmeMemo := TMemo.Create(WizardForm);
  ReadmeMemo.Parent := ReadmePage.Surface;
  ReadmeMemo.Align := alClient;
  ReadmeMemo.ReadOnly := True;
  ReadmeMemo.ScrollBars := ssVertical;
  ReadmeMemo.WordWrap := True;

  if FileExists(TempReadme) then
    ReadmeMemo.Lines.LoadFromFile(TempReadme)
  else
    ReadmeMemo.Lines.Text := 'README.txt not found!';

  idpAddFile(
    'https://raw.githubusercontent.com/megsystem/DebloaterTool/main/DebloaterTool.exe',
    ExpandConstant('{tmp}\{#MyAppExeName}')
  );

  idpDownloadAfter(wpReady);
end;

