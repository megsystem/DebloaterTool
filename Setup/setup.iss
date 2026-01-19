; DebloaterTool Online Installer
; Non-commercial use only
#include "C:\Program Files (x86)\Inno Download Plugin\idp.iss"

#define MyAppName "DebloaterTool"
#define MyAppVersion "1.1"
#define MyAppPublisher "megsystem"
#define MyAppURL "https://github.com/megsystem"
#define MyAppExeName "DebloaterTool.exe"
#define MyAppIcon "DebloaterTool.ico" ; icon file in script folder

[Setup]
; AppId requires double braces format; make sure GUID is stable between versions
AppId={{84C3FD7D-2128-40E9-90BF-F9054235C7C3}}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName=C:\{#MyAppName}
DefaultGroupName={#MyAppName}
OutputDir=.
OutputBaseFilename=DebloaterToolSetup
Compression=lzma
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=admin
UninstallDisplayIcon={app}\{#MyAppIcon}
LicenseFile=LICENSE.txt
AllowNoIcons=no
DisableProgramGroupPage=no
MinVersion=6.1

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
; Keep the ICO and README packaged with the installer
Source: "{#MyAppIcon}"; DestDir: "{app}"; Flags: ignoreversion
Source: "README.txt"; DestDir: "{app}"; Flags: ignoreversion
Source: "LICENSE.txt"; DestDir: "{app}"; Flags: ignoreversion
; The EXE is downloaded at runtime by the Inno Download Plugin
Source: "{tmp}\{#MyAppExeName}"; DestDir: "{app}"; Flags: external ignoreversion

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: startmenuicon
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{group}\Uninstall"; Filename: "{uninstallexe}"; IconFilename: "{app}\{#MyAppIcon}"
Name: "{group}\README.txt"; Filename: "{app}\README.txt"

[Tasks]
Name: "desktopicon"; Description: "Create a &desktop icon"; GroupDescription: "Additional icons:";
Name: "startmenuicon"; Description: "Create a &Start Menu shortcut"; GroupDescription: "Additional icons:";

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Launch {#MyAppName}"; Flags: nowait postinstall skipifsilent

[Code]
var
  ReadmePage: TWizardPage;
  ReadmeMemo: TMemo;

procedure InitializeWizard;
var
  TempReadme: string;
begin
  ExtractTemporaryFile('README.txt');
  ExtractTemporaryFile('LICENSE.txt');

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

function NextButtonClick(CurPageID: Integer): Boolean;
begin
  Result := True;
end;