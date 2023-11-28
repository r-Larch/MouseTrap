$ErrorActionPreference = 'Stop';

$version       = '<version>'
$hash          = '<hash>'
$versionNumber = [Regex]::Replace($version, 'v(\d+\.\d+\.\d+).*', '$1')

$packageName   = 'mousetrap'
$toolsDir      = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"
$url           = "https://github.com/r-Larch/MouseTrap/releases/download/$version/MouseTrap-$versionNumber.zip"


$packageArgs = @{
  packageName   = $packageName
  unzipLocation = $toolsDir
  url           = $url
  checksum      = $hash
  checksumType  = 'SHA256'
}

Install-ChocolateyZipPackage @packageArgs


$mousetrap = "$toolsDir\$packageName";

# install
& $mousetrap "-i";
if (-not $?) {
	throw "error while install";
}

# start the app minimized as tray icon in taskbar.
& $mousetrap
