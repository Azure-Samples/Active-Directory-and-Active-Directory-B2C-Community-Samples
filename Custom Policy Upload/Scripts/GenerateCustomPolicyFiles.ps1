param( 
    [Parameter(Mandatory=$true)] [string]$CPConfigurationFilePath,
	[Parameter(Mandatory=$true)] [string]$PolicyFilesFolder,
	[Parameter(Mandatory=$true)] [string]$DestinationFolder
)

Import-Module "~\CustomPolicyFileProvider\CustomPolicyFileSvc\bin\Debug\CustomPolicyFileSvc.dll"
Invoke-GenerateCustomPolicyFiles -CPConfigurationFilePath $CPConfigurationFilePath -PolicyFilesFolder $PolicyFilesFolder -DestinationFolder $DestinationFolder