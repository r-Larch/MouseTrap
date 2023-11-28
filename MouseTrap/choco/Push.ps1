
param (
	[string] $nuspec,
	[string] $zip,
	[string] $version,
    [switch] $debug = $false
)


function Main() {
	$dir  = Split-Path -parent $nuspec;

	$choco = @{
		version = $version
		nuspec = $nuspec
		dir = $dir
		installScript = [System.IO.Path]::Combine($dir, "chocolateyinstall.ps1")
		uninstallScript = [System.IO.Path]::Combine($dir, "chocolateyuninstall.ps1")
	}

	$hash = (Get-FileHash -Algorithm SHA256 -Path $zip).Hash.ToUpper()
	ChocoUpdateScript $choco $version $hash

	Push-Location $dir
	$previousTag = [Git]::GetVersionTags() | where { $_ -ne "v$version" } | sort -Descending | select -first 2 | select -last 1;
	$history = [Git]::GetHistorySince($previousTag);
	Pop-Location

	ChocoUpdateNuspec $choco $history "https://api.github.com/repos/r-Larch/MouseTrap"

	ChocoPublish $choco $debug

}


function ChocoUpdateScript([object] $choco, [string] $version, [string] $hash) {
    $script = Get-Content $choco.installScript -Encoding UTF8 -Raw
    $script = $script.replace('<version>', $version)
	$script = $script.replace('<hash>', $hash)
    $script | Set-Content $choco.installScript -Force -Encoding UTF8
}


function ChocoUpdateNuspec([object] $choco, [string] $history, [string] $githubRepoApi) {
	$repo = $(Invoke-Webrequest $githubRepoApi).Content | ConvertFrom-Json;
	$topics = $(Invoke-Webrequest $githubRepoApi/topics -Headers @{'Accept'='application/vnd.github.mercy-preview+json'}).Content | ConvertFrom-Json
	$xml = [xml] $(gc -Path $choco.nuspec -Encoding UTF8);
	$xml.package.metadata.version = [Regex]::Replace($version, '^v', '');
	$xml.package.metadata.summary = $repo.description;
	$xml.package.metadata.tags = [string]::Join(" ", $topics.names);
	$xml.package.metadata.copyright = "Copyright $([DateTime]::Now.Year) René Larch";
	$xml.package.metadata.releaseNotes = [string]::Join("`r`n", $history);
	$xml.Save($choco.nuspec);
}


function ChocoPublish([object] $choco, [bool] $debug = $false) {

	$chocoCommand = "choco";
	if (!$(Get-Command $chocoCommand -errorAction SilentlyContinue)) {
		Write-Host "Install chocolatey";
		iwr https://chocolatey.org/install.ps1 -UseBasicParsing | iex
		$env:Path += ";%ALLUSERSPROFILE%\chocolatey\bin";
	}

	# create the nuspec package
	& $chocoCommand pack $choco.nuspec --out $choco.dir

	$ver = [Regex]::Replace($choco.version, "^v(.*)", '$1');
	$nupkgName = $choco.nuspec.replace('.nuspec', ".$($ver).nupkg");

	if (!$debug) {
		# if token is given, we will publish the package to Chocolatey here
		if ($env:CHOCO_TOKEN) {
			& $chocoCommand apiKey -k $env:CHOCO_TOKEN -source https://push.chocolatey.org/
			& $chocoCommand push $nupkgName -source https://push.chocolatey.org/
		} else {
			Write-Warning "Chocolatey token was not set. Publication skipped."
		}
	} else {
		# For development/debugging purposes
		$script = Get-Content $choco.installScript -Encoding UTF8 -Raw
		Write-Host "=============== Choco Install Script ==============="
		Write-Host $script
		Write-Host "===================================================="

		$script = Get-Content $choco.uninstallScript -Encoding UTF8 -Raw
		Write-Host "============== Choco Uninstall Script =============="
		Write-Host $script
		Write-Host "===================================================="

		$nuspec = Get-Content $choco.nuspec -Encoding UTF8 -Raw
		Write-Host "================== Nuspec ==========================="
		Write-Host $nuspec
		Write-Host "===================================================="

		Write-Host "$chocoCommand pack " $choco.nuspec
		Write-Host "$chocoCommand apiKey -k $env:CHOCO_TOKEN -source https://push.chocolatey.org/"
		Write-Host "$chocoCommand push $nupkgName"
	}
}


class Git {
	static [string[]] GetTags() {
		return $(git tag --list).Split("`n");
	}
	static [string[]] GetVersionTags() {
		return [Git]::GetTags() | Select-String -pattern "v[\d+\.]+(-(alpha|beta))?";
	}
	static [string[]] GetHistorySince([string] $tagOrHash) {
        if ([string]::IsNullOrEmpty($tagOrHash)){
            return $(git log --oneline);
        }
		return $(git log "$tagOrHash..HEAD" --oneline);
	}
}


#########################
#		run main        #
#########################

Main;

#########################