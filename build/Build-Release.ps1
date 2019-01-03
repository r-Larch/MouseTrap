
param (
    [Parameter(Mandatory=$false)]
    [ValidatePattern("^\d+\.\d+\.\d+$")]
    [string]
    $ReleaseVersionNumber,

	[Parameter(Mandatory=$false)]
	[switch]
	$Alpha = $false,

	[Parameter(Mandatory=$false)]
	[switch]
	$Beta = $false
)


$Root = Split-path -parent $(Split-Path -parent $MyInvocation.MyCommand.Path)

$SolutionFile = [System.IO.Path]::Combine($Root, "MouseTrap.sln");
$DistFolder   = [System.IO.Path]::Combine($Root, "dist");
$TempFolder   = [System.IO.Path]::Combine($Root, "temp");
$ReadmeFile   = [System.IO.Path]::Combine($Root, "README.md");
$ProjectRoot  = [System.IO.Path]::Combine($Root, "MouseTrap");
$OutputDir    = [System.IO.Path]::Combine($ProjectRoot, "bin");
$chocoNuspec  = [System.IO.Path]::Combine($ProjectRoot, "MouseTrap.nuspec");
$AssemblyInfo = [System.IO.Path]::Combine($ProjectRoot, "Properties/AssemblyInfo.cs");

$GithubGetRepoApi = "https://api.github.com/repos/r-Larch/MouseTrap";

function Main() {
	
	# auto commit ? =============================

	if ([Git]::HasStagedChanges()) {
		[Git]::ShowStaged();
		if ([UI]::Comfirm("you have staged changes! Do you wand to auto commit them?") -eq $false) {
			[Git]::Discard($AssemblyInfo);
			[Git]::Discard($chocoNuspec);
			[UI]::ThrowError("Please commit your staged changes first");
		}
	}

	if ([Git]::HasUnstagedChanges()) {
		[Git]::ShowUnstaged();
		if ([UI]::Comfirm("you have unstaged changes! Do you wand to auto commit them?") -eq $false) {
			[UI]::ThrowError("Please commit your unstaged changes first");
		}
		else {
			[Git]::Stage("*");
		}
	}


	# version number =============================

	[Git]::Stage($AssemblyInfo); # backup
	if ([string]::IsNullOrEmpty($ReleaseVersionNumber)) {
		$ReleaseVersionNumber = GetVersion $AssemblyInfo;
	}
	else {
		SetVersion $AssemblyInfo $ReleaseVersionNumber;
	}


	# Git tags and history =============================

	$tag = [Git]::CreateVersionTag($ReleaseVersionNumber, $Alpha, $Beta);
	$previousTag = [Git]::GetVersionTags() | where { $_ -ne $tag } | sort -Descending | select -first 1;
	$history = [Git]::GetHistorySince($previousTag);


	# update .nuspec =============================

	[Git]::Stage($chocoNuspec); # backup
	$repo = $(Invoke-Webrequest $GithubGetRepoApi).Content | ConvertFrom-Json;
	$topics = $(Invoke-Webrequest $GithubGetRepoApi/topics -Headers @{'Accept'='application/vnd.github.mercy-preview+json'}).Content | ConvertFrom-Json
	$xml = [xml] $(gc -Path $chocoNuspec -Encoding UTF8);
	$xml.package.metadata.version = $ReleaseVersionNumber;
	$xml.package.metadata.summary = $repo.description;
	$xml.package.metadata.tags = [string]::Join(" ", $topics.names);
	$xml.package.metadata.description = [string]::Join("`r`n", $(gc -Path $ReadmeFile));
	$xml.package.metadata.copyright = "Copyright $([DateTime]::Now.Year) René Larch";
	$xml.package.metadata.releaseNotes = [string]::Join("`r`n", $history);
	# $xml.package.metadata.licenseUrl = ;
	# $xml.package.metadata.projectUrl = $repo.html_url;
	# $xml.package.metadata.projectSourceUrl = $repo.html_url;
	# $xml.package.metadata.bugTrackerUrl = ;
	# $xml.package.metadata.docsUrl = ;
	# $xml.package.metadata.iconUrl = ;
	$xml.Save($chocoNuspec);
	

	# commit changes =============================

	[Git]::Stage($AssemblyInfo);
	[Git]::Stage($chocoNuspec);

	if ([Git]::HasTag($tag)) {
		if ([Git]::HasStagedChanges()) {
			[Git]::RemoveTag($tag);
			[Git]::Commit("Update $tag");
			[Git]::AddTag($tag);
		}
	}
	else {
		if ([Git]::HasStagedChanges()) {
			[Git]::Commit("Release $tag");
		}
		[Git]::AddTag($tag);
	}


	# Build =============================

	BuildSolution $SolutionFile "Release" $OutputDir;
	$exacutable = [System.IO.Path]::Combine($OutputDir, "MouseTrap.exe")
	# SignExecutable $exacutable "renelarch@gmail.com";
	Copy-Item $exacutable "$DistFolder\MouseTrap.$ReleaseVersionNumber.exe";
	# Zip $OutputDir "$DistFolder\MouseTrap.$ReleaseVersionNumber.zip";


	# choco =============================

	ChocoPack $chocoNuspec $DistFolder;


	# commit results =============================

	[Git]::Stage("$DistFolder\MouseTrap.$ReleaseVersionNumber.exe");
	[Git]::Stage("$DistFolder\MouseTrap.$ReleaseVersionNumber.nupkg");
	[Git]::Commit("$tag binarys");


	# cleanup =============================

	CleanDir $TempFolder;
}


function ChocoPack([string] $nuspec, [string] $nupkg) {
	
	if (!$(Get-Command choco -errorAction SilentlyContinue)) {
		Write-Host "Install chocolatey";
		iwr https://chocolatey.org/install.ps1 -UseBasicParsing | iex
		$env:Path += ";%ALLUSERSPROFILE%\chocolatey\bin";
	}

	& choco pack "$nuspec" --outputdirectory "$nupkg";
	if (-not $?) {
		throw "The choco pack process returned an error code."
	}

	$xml = [xml] $(gc -Path $chocoNuspec -Encoding UTF8);
	$name = $xml.package.metadata.id + "." + $xml.package.metadata.version + ".nupkg";
	$file = [System.IO.Path]::Combine($nupkg, $name);

	if ([UI]::Ask("Enter 'choco push' to publish '$name':", "choco push")) {
		& choco push $file;
		if (-not $?) {
			throw "The publish process returned an error code."
		}
	}
}


class Git {
	static [string[]] GetTags() {
		return $(git tag --list).Split("`n");
	}
	static [string[]] GetVersionTags() {
		return [Git]::GetTags() | Select-String -pattern "v[\d+\.]+(-(alpha|beta))?";
	}
	static [string] GetLatastVersionTag() {
		return [Git]::GetVersionTags() | sort -Descending | select -first 1;
	}
	static [string[]] GetHistorySince([string] $tagOrHash) {
        if ([string]::IsNullOrEmpty($tagOrHash)){
            return $(git log --oneline);
        }
		return $(git log "$tagOrHash..HEAD" --oneline);
	}
	static AddTag([string] $tag) {
		git tag -a "$tag" -m "$tag" | Write-Host -ForegroundColor Green;
	}
	static RemoveTag([string] $tag) {
		git tag -d "$tag" | Write-Host -ForegroundColor Magenta;
	}
	static [string] CreateVersionTag([string] $version, [bool] $alpha = $false, [bool] $beta = $false) {
		$tag = "v$version";
		if ($alpha) {
			$tag += "-alpha";
		}
		elseif ($beta) {
			$tag += "-beta";
		}
		return $tag;
	}
	static [bool] HasTag([string] $tag) {
		return [Git]::GetTags().Contains($tag);
	}

	static [bool] HasUncommittedChanges() {
		return [string]::IsNullOrEmpty($(git diff HEAD --stat)) -eq $false;
	}
	static [bool] HasUnstagedChanges() {
		return [string]::IsNullOrEmpty($(git diff --stat)) -eq $false;
	}
	static [bool] HasStagedChanges() {
		return [string]::IsNullOrEmpty($(git diff --staged --stat)) -eq $false;
	}
	static ShowUnstaged() {
		Write-Host "`n Unstaged changes:`n" -ForegroundColor DarkGreen;
		git diff --stat | Write-Host -ForegroundColor Green;
	}
	static ShowStaged() {
		Write-Host "`n Staged changes:`n" -ForegroundColor DarkGreen;
		git diff --staged --stat | Write-Host -ForegroundColor Green;
	}

	static Stage([string] $file) {
		git add $file | Write-Host -ForegroundColor Green;
	}
	static Unstage([string] $file) {
		git reset HEAD "$file" | Write-Host -ForegroundColor DarkYellow;
	}
	static Discard([string] $file) {
		git checkout -- "$file" | Write-Host -ForegroundColor Magenta;
	}
	static FullReset([string] $file) {
		[Git]::Unstage($file);
		[Git]::Discard($file);
	}
	static Commit([string] $msg) {
		git commit -m $msg | Write-Host -ForegroundColor Green;
	}
}


class UI {
	static [bool] Comfirm([string] $message) {

		$choices = [Collections.ObjectModel.Collection[Management.Automation.Host.ChoiceDescription]]::new();
		$choices.Add((New-Object Management.Automation.Host.ChoiceDescription -ArgumentList '&Yes'))
		$choices.Add((New-Object Management.Automation.Host.ChoiceDescription -ArgumentList '&No'))

		Write-Host "";
		$decision = $global:Host.UI.PromptForChoice($null, $message, $choices, 1)
		if ($decision -eq 0) {
			return $true;
		} else {
			return $false;
		}
	}
	static [bool] Ask([string] $msg, [string] $answare) {
		$res = "";
		do {
			Write-Host "`n`n $msg (<empty> to skip)" -ForegroundColor Green;
			Write-Host " " -NoNewline;

			$res = Read-Host
			if ($res -eq $answare) {
				return $true;
			}
		}
		while (![string]::IsNullOrEmpty($res));

		return $false;
	}
	static ThrowError($error) {
		Write-Host "`n $error" -ForegroundColor Red;
		Write-Host "";
		exit;
	}
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
		& $SignTool sign /n "$subjectName" "$executable" #/tr http://timestamp.comodoca.com/authenticode
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