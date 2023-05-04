; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "MiniserverUpdater"
#define MyAppVersion "0.9.9"
#define MyAppPublisher "Musat"
#define MyAppExeName "WPFConfigUpdater.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application. Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{8259765F-8364-4436-8A49-3FD08C22125D}
AppName={#MyAppName}
AppVersion={#MyAppVersion} 
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\MiniserverUpdater 
DisableProgramGroupPage=yes
; Uncomment the following line to run in non administrative install mode (install for current user only.)
;PrivilegesRequired=lowest
OutputDir=C:\Users\musatbe\source\repos\WPFConfigUpdater\InnoSetup
OutputBaseFilename=MiniserverUpdater_{#MyAppVersion}
SetupIconFile=C:\Users\musatbe\source\repos\WPFConfigUpdater\resources\Icon.ico
Compression=lzma
SolidCompression=yes
WizardStyle=modern
;DisableDirPage=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "C:\Users\musatbe\source\repos\WPFConfigUpdater\bin\Release\net6.0-windows\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\musatbe\source\repos\WPFConfigUpdater\bin\Release\net6.0-windows\Changelog.txt"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\musatbe\source\repos\WPFConfigUpdater\bin\Release\net6.0-windows\EmptyProject.Loxone"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\musatbe\source\repos\WPFConfigUpdater\bin\Release\net6.0-windows\Newtonsoft.Json.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\musatbe\source\repos\WPFConfigUpdater\bin\Release\net6.0-windows\RestSharp.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\musatbe\source\repos\WPFConfigUpdater\bin\Release\net6.0-windows\WPFConfigUpdater.deps.json"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\musatbe\source\repos\WPFConfigUpdater\bin\Release\net6.0-windows\WPFConfigUpdater.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\musatbe\source\repos\WPFConfigUpdater\bin\Release\net6.0-windows\WPFConfigUpdater.pdb"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\musatbe\source\repos\WPFConfigUpdater\bin\Release\net6.0-windows\WPFConfigUpdater.runtimeconfig.json"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\musatbe\source\repos\WPFConfigUpdater\bin\Release\net6.0-windows\resources\*"; DestDir: "{app}\resources"; Flags: ignoreversion recursesubdirs createallsubdirs
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent
