$ErrorActionPreference = 'Stop';

$version       = '<version>'
$hash          = '<hash>'

$packageName   = 'mousetrap'
$toolsDir      = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"
$url           = "https://github.com/r-Larch/MouseTrap/releases/download/v$version-beta/MouseTrap-$version.zip"


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
