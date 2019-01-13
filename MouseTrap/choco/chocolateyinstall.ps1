
$ErrorActionPreference = 'Stop';

$packageName= 'mousetrap.exe'
$toolsDir   = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"

$mousetrap = "$toolsDir\$packageName";
# install
& $mousetrap "-i";
if (-not $?) {
	throw "error while install";
}
