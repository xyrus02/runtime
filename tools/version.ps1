add-type -path "$PSScriptRoot\cake\XyrusWorx.Management.dll"

function Update-AssemblyInfo ([string] $path, [XyrusWorx.Management.SemanticVersion] $version) {
    $assemblyVersionPattern = 'AssemblyVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)'
    $fileVersionPattern = 'AssemblyFileVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)'
    $informationalVersionPattern = 'AssemblyInformationalVersion\(".*?"\)'
    $assemblyVersion = 'AssemblyVersion("' + $Version.DeclareFinal().WithoutMetadata().ToString() + '.0")';
    $fileVersion = 'AssemblyFileVersion("' + $Version.DeclareFinal().WithoutMetadata().ToString() + '.0")';
    $informationalVersion = 'AssemblyInformationalVersion("' + $Version.ToString() + '")';
    
	$file = [IO.FileInfo] $path
	
	(Get-Content $path) | ForEach-Object {
		% {$_ -replace $informationalVersionPattern, $informationalVersion } |
		% {$_ -replace $assemblyVersionPattern, $assemblyVersion } |
		% {$_ -replace $fileVersionPattern, $fileVersion }
	} | Set-Content $path
}

function Get-SolutionPackageVersion {
    param([Parameter(Mandatory = $true, ValueFromPipeline = $true, Position = 0)] [System.Xml.XmlElement] $Node)

    if ($Node.LocalName -eq "PackageVersion") {
        return [XyrusWorx.Management.SemanticVersion]::Parse($Node.InnerText)
    }

    foreach($childNode in @($Node.ChildNodes)) {
        if ($childNode -is "System.Xml.XmlElement") {
            $childResult = Get-SolutionPackageVersion $childNode
            if ($childResult -ne $null) {
                return $childResult
            }
        }
    }

    return $null;
}
function Set-SolutionPackageVersion {
    param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true, Position = 0)] [System.Xml.XmlElement] $Node,
        [Parameter(Mandatory = $true, Position = 1)] [XyrusWorx.Management.SemanticVersion] $Version)

    if ($Node.LocalName -eq "PackageVersion") {
        Write-Verbose "Writing package version property: $($Version.ToString())"
        $Node.InnerText = $Version.ToString()
    }

    if ($Node.LocalName -eq "PackageBaseVersion") {
        Write-Verbose "Writing package version property: $($Version.DeclareFinal().WithoutMetadata().ToString())"
        $Node.InnerText = $Version.DeclareFinal().WithoutMetadata().ToString()
    }

    foreach($childNode in @($Node.ChildNodes)) {
        if ($childNode -is "System.Xml.XmlElement") {
            Set-SolutionPackageVersion $childNode $Version
        }
    }
}

function global:Get-CurrentVersion {
    Process {
        $xml =  [xml](Get-Content "$PSScriptRoot\..\src\package.props")
        $text = [string]($xml.Project.PropertyGroup.PackageVersion)

        return $text.Trim()
    }
}

function global:Update-Version {
    [CmdletBinding(SupportsShouldProcess=$True)]
    param(
        [alias("p")] [switch] $Patch,
        [alias("u")] [switch] $Feature,
        [alias("r")] [switch] $Release,
        [alias("v")] [string] $Version = $null,

        [alias("pre")]  [string] $PreRelease = $null,
        [alias("step")] [int]    $Increment = 1
    )

    Process {

        if ($Increment -lt 0) { 
            Write-Error -Message "Decrements are not allowed" -Category InvalidArgument
            Return
        }

        if (-not $Patch -and -not $Feature -and -not $Release -and ($Version -eq $null -or $Version -eq "")) {
            Write-Warning -Message "No action requested. Exiting."
            Return
        }

        $_BuildProperties = [xml](gc "$PSScriptRoot\..\src\package.props")
        $_CurrentVersion = Get-SolutionPackageVersion $_BuildProperties.Project

        if ($_CurrentVersion -eq $null) {
            $_CurrentVersion = [XyrusWorx.Management.SemanticVersion]::new(0,0,0)
        }

        for($i = 0; $i -lt $Increment; $i++) {
            $_NextVersion = $_CurrentVersion

            if ($Version -ne $null -and $Version -ne "") {
                try {
                    $_NextVersion = [XyrusWorx.Management.SemanticVersion]::Parse($Version)
                }
                catch {
                    Write-Error -Message "The version string ""$Version"" is invalid." -Category InvalidArgument
                    Return
                }
                break
            }

            elseif ($Release) { $_NextVersion = $_NextVersion.RaiseMajor() }
            elseif ($Feature) { $_NextVersion = $_NextVersion.RaiseMinor() }
            elseif ($Patch) { $_NextVersion = $_NextVersion.Patch() }
        }

        if ($PreRelease -ne $null -and $PreRelease.Trim() -ne "") {
            $_NextVersion = $_NextVersion.DeclarePreRelease($PreRelease)
        }

        Write-Information "$($_CurrentVersion.ToString()) -> $($_NextVersion.ToString())"
        Set-SolutionPackageVersion $_BuildProperties.Project -Version $_NextVersion

        if ($WhatIf -or $WhatIfPreference) {
            Write-Information "Skipping output because -WhatIf was set."
            Return
        }

        try {
        
            if (Test-Path -Path "$PSScriptRoot\..\src\Package.cs" -PathType Leaf) {
                Update-AssemblyInfo -Path "$PSScriptRoot\..\src\Package.cs" -Version $_NextVersion
            }
        
            $_BuildProperties.Save("$PSScriptRoot\..\src\package.props")
        }
        catch {
            Write-Error -Message "Failed to write to ""$ProjectPath"": $_"
            Return
        }

    }
}

set-alias "ver" "Get-CurrentVersion"
set-alias "update" "Update-Version"