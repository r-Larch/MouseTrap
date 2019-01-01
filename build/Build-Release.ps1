
param (
    [Parameter(Mandatory=$false)]
    [ValidatePattern("^\d+\.\d+\.\d+$")]
    [string]
    $ReleaseVersionNumber
)


$Root = Split-path -parent $(Split-Path -parent $MyInvocation.MyCommand.Path)

$SolutionFile = [System.IO.Path]::Combine($Root, "MouseTrap.sln");
$DistFolder   = [System.IO.Path]::Combine($Root, "dist");
$TempFolder   = [System.IO.Path]::Combine($Root, "temp");
$ProjectRoot  = [System.IO.Path]::Combine($Root, "MouseTrap");
$OutputDir    = [System.IO.Path]::Combine($ProjectRoot, "bin");
$chocoNuspec  = [System.IO.Path]::Combine($ProjectRoot, "MouseTrap.nuspec");
$AssemblyInfo = [System.IO.Path]::Combine($ProjectRoot, "Properties/AssemblyInfo.cs");


function Main() {
	
	if ([string]::IsNullOrEmpty($ReleaseVersionNumber)) {
		$ReleaseVersionNumber = GetVersion $AssemblyInfo;
	}
	else {
		SetVersion $AssemblyInfo $ReleaseVersionNumber;
	}

	BuildSolution $SolutionFile "Release" $OutputDir;

	$exacutable = [System.IO.Path]::Combine($OutputDir, "MouseTrap.exe")

	# SignExecutable $exacutable "My Certificate Subject";

	Copy-Item $exacutable "$DistFolder\MouseTrap.$ReleaseVersionNumber.exe";
	# Zip $OutputDir "$DistFolder\MouseTrap.$ReleaseVersionNumber.zip";

	# ChocoPack $chocoNuspec "$DistFolder\MouseTrap.$ReleaseVersionNumber.nupkg";

	# GitAddTag "v$ReleaseVersionNumber" $alpha $beta

	CleanDir $TempFolder;
}


function SignExecutable([string] $executable, [string] $subjectName) {
	# Code signing, works on my machine but probably not very portable
	# Use the following command to create a self-signed cert to build a signed version of the WACS executable 
	# New-SelfSignedCertificate `
	#     -CertStoreLocation cert:\currentuser\my `
	#     -Subject "CN=WACS" `
	#     -KeyUsage DigitalSignature `
	#     -Type CodeSigning

	$from = "C:\Program Files (x86)\Windows Kits\";
	$SignTool = $(Get-ChildItem $from -Recurse -File `
		| Where {$_.Name -eq "signtool.exe"} `
		| where {$_.FullName -like "*\x86\*" -or $_.FullName -like "*\x64\*" -or $_.FullName -like "*\App Certification Kit\*" } `
		| select -first 1 `
		).FullName;

	if (Test-Path $SignTool) {
		& $SignTool sign /n "$subjectName" "$executable"
	}
	else {
		throw 'signtool.exe Not found! Please open "Visual Studio Installer" and install "ClickOnce Publication Tools"';
	}
}



function BuildSolution([string] $solutionFile, [string] $configuration, [string] $outputDir) {
	$NuGet = GetTool "nuget.exe" "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe";
	$NuGetFolder = Join-Path -Path $([System.IO.Path]::GetDirectoryName($solutionFile)) "packages";

	# Restore NuGet packages
	cmd.exe /c "$NuGet restore $solutionFile -NonInteractive -PackagesDirectory $NuGetFolder"

	$MSBuild = GetMSBuildPath;

	# Clean solution
	& $MSBuild "$solutionFile" "/p:OutDir=$outputDir;Configuration=$configuration" /maxcpucount /t:Clean
	if (-not $?) {
		throw "The MSBuild process returned an error code."
	}

	# Build
	& $MSBuild "$solutionFile" "/p:OutDir=$outputDir;Configuration=$configuration" /maxcpucount
	if (-not $?) {
		throw "The MSBuild process returned an error code."
	}
}

function GetTool([string] $fileName, [string] $downloadUrl = $null) {
	$tool = "$env:HOMEDRIVE\Tools\nuget.exe"
	If ($(Test-Path $tool) -eq $false) {
		if ([string]::IsNullOrEmpty($downloadUrl)) {
			throw [System.IO.FileNotFoundException]::new("$tool not found", $tool);
		}
		else {
			Invoke-WebRequest $downloadUrl -OutFile $tool
		}
	}
	return $tool;
}

function Zip([string] $directoryPath, [string] $destinationZipFile, [bool] $overwrite = $true) {
	if (Test-Path $destinationZipFile) {
	    if ($overwrite) {
			Remove-Item $destinationZipFile;
		}
		else {
			throw "Target File allready exists: '$destinationZipFile'";
		}
	}
	Add-Type -assembly "system.io.compression.filesystem";
	[io.compression.zipfile]::CreateFromDirectory($directoryPath, $destinationZipFile);
}

function GetVersion([string] $assemblyInfo) {
	return $(Select-String -path $assemblyInfo -pattern 'AssemblyFileVersion\("([^"]+)"\)').Matches.Groups[1].Value;
}

function SetVersion([string] $assemblyInfo, [string] $version) {
	$versionParts = $version.Split(".")
	$NewVersion = 'AssemblyVersion("' + $versionParts[0] + '.' + $versionParts[1] + '.' + $versionParts[2] + '")'
	$NewFileVersion = 'AssemblyFileVersion("' + $version + '")'

	(gc -Path $assemblyInfo) `
		-replace 'AssemblyVersion\("[0-9\.*]+"\)', $NewVersion |
		sc -Path $assemblyInfo -Encoding UTF8
	(gc -Path $assemblyInfo) `
		-replace 'AssemblyFileVersion\("[0-9\.]+"\)', "$NewFileVersion" |
		sc -Path $assemblyInfo -Encoding UTF8
}

function GetMSBuildPath() {
    $vs14key = "HKLM:\SOFTWARE\Microsoft\MSBuild\ToolsVersions\14.0"
    $vs15key = "HKLM:\SOFTWARE\wow6432node\Microsoft\VisualStudio\SxS\VS7"
    $msbuildPath = ""

    if (Test-Path $vs14key) {
        $key = Get-ItemProperty $vs14key
        $subkey = $key.MSBuildToolsPath
        if ($subkey) {
            $msbuildPath = Join-Path $subkey "msbuild.exe"
        }
    }

    if (Test-Path $vs15key) {
        $key = Get-ItemProperty $vs15key
        $subkey = $key."15.0"
        if ($subkey) {
            $msbuildPath = Join-Path $subkey "MSBuild\15.0\bin\amd64\msbuild.exe"
        }
    }

    return $msbuildPath
}

function CleanDir([string] $dir) {
	if (Test-Path $dir) {
	    Remove-Item $dir -Recurse
	}
	New-Item $dir -Type Directory
}



#########################
#		run main        #
#########################

Main;

#########################