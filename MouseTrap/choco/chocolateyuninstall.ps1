
$ErrorActionPreference = 'Stop';

$packageName= 'mousetrap.exe'
$toolsDir   = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"

$mousetrap = "$toolsDir\$packageName";

# uninstall
& $mousetrap "-u";
if (-not $?) {
	throw "error while uninstall";
}
