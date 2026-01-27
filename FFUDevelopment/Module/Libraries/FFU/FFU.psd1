@{
RootModule = 'FFU.psm1'
ModuleVersion = '0.1.0'
CompatiblePSEditions = @('Core')
GUID = 'e387272a-d5d0-4b3d-93ba-4321c3e5135f'
Author = 'rbalsley, dodexahedron'
CompanyName = 'FFU'
Copyright = '(c) rbalsley, dodexahedron. All rights reserved. Provided under the terms of the MIT license.'
Description = 'A tool to help automate Windows image creation and deployment, using FFU images for fast imaging.'
PowerShellVersion = '7.5'
ProcessorArchitecture = 'None'
RequiredModules = @('Dism', 'Hyper-V', 'Storage')
RequiredAssemblies = 'bin\FFU.Core.dll'
NestedModules = @()
FunctionsToExport = @()
CmdletsToExport = @()
VariablesToExport = @()
AliasesToExport = @()
PrivateData = @{
  PSData = @{
    Tags = @('FFU','Deployment','Imaging')
    # LicenseUri = ''
    ProjectUri = 'https://github.com/rbalsleyMSFT/FFU'
    # IconUri = ''
    ReleaseNotes = 'Porting in progress. Not ready for production use.'
    Prerelease = 'alpha1'
  }
}
# HelpInfoURI = ''
# DefaultCommandPrefix = ''
}
