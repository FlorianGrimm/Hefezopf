Set-StrictMode -Version Latest
#
Set-Variable -Name "BuildVerbose"  -Value $False -Scope Global -Option ReadOnly -Force
#
Set-Variable -Name "SolutionFile"  -Value "Hefezopf2016.sln" -Scope Global -Option ReadOnly -Force
#
function Get-Script-Directory
{
    $scriptInvocation = (Get-Variable MyInvocation -Scope 1).Value
    return Split-Path $scriptInvocation.MyCommand.Path
}
if ((Get-Variable -Name "buildDir" -ValueOnly -ErrorAction SilentlyContinue) -eq $null) {
    #$buildDir = ""
    Set-Variable -Name "buildDir" -Value "" -Scope Global -Option ReadOnly -Force
}
if ($buildDir -eq "") {
    #Write-Host "BuildDir is empty 1."
    #$buildDir=$PSScriptRoot
    # if ($buildDir -eq $null) {
    #     $buildDir = ""
    # }
    if ($buildDir -eq "") {
        #Write-Host "BuildDir is empty 2."
        #$buildDir = Get-Script-Directory
        Set-Variable -Name "buildDir" -Value (Get-Script-Directory) -Scope Global -Option ReadOnly -Force
    }
} else {
    Write-Host "BuildDir: $buildDir old"
}
Write-Host "Hint: BuildDir: $buildDir"
#
#$rootDir = [System.IO.Path]::GetDirectoryName($buildDir)
Set-Variable -Name "rootDir" -Value ([System.IO.Path]::GetDirectoryName($buildDir)) -Scope Global -Option ReadOnly -Force
Write-Host "Hint: rootDir: $rootDir"
#
#$slnDir = [System.IO.Path]::Combine($rootDir, "source")
Set-Variable -Name "slnDir" -Value ([System.IO.Path]::GetFullPath($rootDir)) -Scope Global -Option ReadOnly -Force
Write-Host "Hint: slnDir: $slnDir"
#
Set-Variable -Name "libDir" -Value ([System.IO.Path]::Combine($rootDir, "Hefezopf_Build")) -Scope Global -Option ReadOnly -Force
Write-Host "Hint: libDir: $libDir"
#
function Build-RestartShell(){
    $exe = "C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe"
    if ($host.Version.Major -eq 2){
        Start-Process $exe -ArgumentList  @('-Version 3.0', '-NoExit', '-ExecutionPolicy Unrestricted', '-File "'+$buildDir+'\build.ps1"')
    } else {
        Start-Process $exe -ArgumentList @('-Version 3.0', '-NoExit', '-ExecutionPolicy Unrestricted', '-File "'+$buildDir+'\build.ps1"')
    }
    (Get-Host).SetShouldExit(0)
}
#
function Get-Bits() {
    if ([System.IntPtr]::Size -eq 4) {
        "32-bit"
    } else{
        "64-bit"
    }
}
#
function Build-SetConfiguration(){
<#
.SYNOPSIS
    Set the configuration and special variables.

.DESCRIPTION
    Build-Compile needs this.

.PARAMETER Configuration
    Debug
    -or-
    Release
#>
param(
    [string] $Configuration="",
    [Switch] $Debug,
    [Switch] $Release
    )
    if($Debug){$Configuration="Debug"}
    if($Release){$Configuration="Release"}
    if([string]::IsNullOrEmpty($Configuration)){$Configuration="Debug"}
    Set-Variable -Name "BuildConfiguration" -Value $Configuration -Scope Global -Option ReadOnly -Force
    #
    $Major=0
    $ass=[System.AppDomain]::CurrentDomain.GetAssemblies() | ?{$_.FullName -like 'Microsoft.SharePoint.Library, *'}
    if ($ass -ne $null){
        $Major = $ass.GetName().Version.Major
    } else {
        if (test-path 'C:\Program Files\Common Files\microsoft shared\Web Server Extensions\15\bin\PSCONFIG.EXE'){
            $Major = 15
        }
        if (test-path 'C:\Program Files\Common Files\microsoft shared\Web Server Extensions\16\bin\PSCONFIG.EXE'){
            $Major = 16
        }
    }
    Set-Variable -Name "SPVersion"  -Value $Major -Scope Global -Option ReadOnly -Force

    Set-Variable -Name "GACDlls" -Value @(
            @{Name="Gsaelzbrot.Library"; Path="$($slnDir)\..\Gsaelzbrot\GsaelzbrotLibrary\bin\$($Configuration)\Gsaelzbrot.Library.dll"},
            @{Name="Hefezopf.Contracts"; Path="$($slnDir)\HefezopfContractsy\bin\$($Configuration)\Hefezopf.Contracts.dll"},
            @{Name="Hefezopf.Fundament"; Path="$($slnDir)\HefezopfFundament\bin\$($Configuration)\Hefezopf.Fundament.dll"},
            @{Name="Hefezopf.Library"; Path="$($slnDir)\HefezopfFundament\bin\$($Configuration)\Hefezopf.Library.dll"}
        ) -Scope Global -Option ReadOnly -Force

    Set-Variable -Name "Application_csproj"  -Value "$($slnDir)\HefezopfSharePointApplication2016\HefezopfSharePointApplication2016.csproj" -Scope Global -Option ReadOnly -Force
    Set-Variable -Name "Application_WspName"  -Value "HefezopfSharePointApplication.wsp" -Scope Global -Option ReadOnly -Force
    Set-Variable -Name "Application_WspPath"  -Value "$($slnDir)\HefezopfSharePointApplication2016\bin\$($Configuration)\HefezopfSharePointApplication.wsp" -Scope Global -Option ReadOnly -Force
    #
    Set-Variable -Name "Frontend_csproj"  -Value "$($slnDir)\HefezopfSharePointFrontend2016\HefezopfSharePointFrontend2016.csproj" -Scope Global -Option ReadOnly -Force
    Set-Variable -Name "Frontend_WspName"  -Value "HefezopfSharePointFrontend.wsp" -Scope Global -Option ReadOnly -Force
    Set-Variable -Name "Frontend_WspPath"  -Value "$($slnDir)\HefezopfSharePointFrontend2016\bin\$($Configuration)\HefezopfSharePointFrontend.wsp" -Scope Global -Option ReadOnly -Force
    #
    Write-Host "BuildConfiguration is $($BuildConfiguration)."
}
#
Function Build-WaitForJobDone([string] $Name){
    Write-Host -NoNewline "Waiting for deployment job to complete" $Name "."
    Start-Sleep -Seconds 2
    $wspSol = Get-SPSolution $Name -ErrorAction SilentlyContinue
    if ($wspSol -eq $null) {
        Start-Sleep -Seconds 2
        $wspSol = Get-SPSolution $Name -ErrorAction SilentlyContinue
        if ($wspSol -eq $null) {
            Write-Host "Solution $Name not found."
        }
    }
    while($wspSol.JobExists)
    {
        Start-Sleep -Seconds 2
        Write-Host -NoNewline "."
        $wspSol = Get-SPSolution $Name
    }
    Write-Host ""
    $wspSol | fl
    return $wspSol
}
#
function Build-StartSPServices(){
param
(
    [Parameter(Mandatory=$false, HelpMessage='-ServiceNames Optional, provide a set of service names to restart.')]
    [Array]$ServiceNames=@("SharePoint Timer Service","SharePoint Administration", "World Wide Web Publishing Service", "IIS Admin Service", "ProjectEventService16"),
    [Parameter(Mandatory=$false, HelpMessage='Start or restart.')]
    [switch] $Restart
)
    #$ServiceNames=@("SharePoint Timer Service","SharePoint Administration", "SharePoint Server Search 15", "World Wide Web Publishing Service", "IIS Admin Service"),
    #$ServiceNames=@("SharePoint 2010 Timer","SharePoint 2010 Administration", "SharePoint Server Search 14", "World Wide Web Publishing Service", "IIS Admin Service")
    $farm = Get-SPFarm
    $servers = $farm.Servers
    #
    foreach($server in $servers) {
        if($server.Role -ne [Microsoft.SharePoint.Administration.SPServerRole]::Invalid) {
            $serverName = $server.Name
            foreach($serviceName in $ServiceNames) {
                    if ($serviceName -eq "SharePoint Timer Service") {
                        if ( 14 -eq $SPVersion){
                            $serviceName = "SharePoint 2010 Timer"
                        }
                    }
                    if ($serviceName -eq "SharePoint Administration") {
                        if ( 14 -eq $SPVersion){
                            $serviceName = "SharePoint 2010 Administration"
                        }
                    }
                    if ($serviceName -eq "SharePoint Server Search 15") {
                        if ( 14 -eq $SPVersion){
                            $serviceName = "SharePoint Server Search 14"
                        }
                    }
                    if ($serviceName -eq "ProjectEventService16") {
                        if ( 15 -eq $SPVersion){
                            $serviceName = "ProjectEventService15"
                        }
                    }
                    #SharePoint 2010 Tracing
                    #SharePoint 2010 User Code Host
                    #SharePoint 2010 VSS Writer
                    #SharePoint Foundation Search V4
                    #SharePoint Server Search 14
                    #
                    $serviceInstance = Get-Service -ComputerName $serverName -Name $serviceName -ErrorAction SilentlyContinue
                    if ($serviceInstance -eq $null) {
                        Write-Host "Service '$serviceName' on '$serverName' not found."
                    } else {
                        if ($Restart){
                            Write-Host "Attempting to restart service '$serviceName' on '$serverName' ..." -NoNewline
                            try
                            {
                                Restart-Service -InputObject $serviceInstance
                                Write-Host " Done!"
                            }
                            catch
                            {
                                Write-Host "Error Occured: " $_.Message
                            }
                        } else {
                            if ($serviceInstance.Status -eq 'Running'){
                                Write-Host "Service '$serviceName' on '$serverName' is runnging."
                            } else {
                                Write-Host "Attempting to start service '$serviceName' on '$serverName' ..." -NoNewline
                                try
                                {
                                        Start-Service -InputObject $serviceInstance
                                        Write-Host " Done!"
                                }
                                catch
                                {
                                        Write-Host "Error Occured: " $_.Message
                                }
                            }
                        }
                    }
            }
        }
    }
}
#
function GetMyHostWebApplicationId(){
    $Id = Get-Variable -Name "MyHostId" -ValueOnly -ErrorAction SilentlyContinue -Scope Global
    if ($Id -eq $null) {
        $id = [Guid]::Empty
        Try {
            $WebApplications = Get-SPWebApplication -IncludeCentralAdministration
            if ($WebApplications.Length -gt 2) {
                $CAWebApplication = $WebApplications | ?{$_.IsAdministrationWebApplication} | Select-Object -First 1
                $sc = Get-SPServiceContext($CAWebApplication.Url)
                $upm = new-object Microsoft.Office.Server.UserProfiles.UserProfileManager($sc)
                if ($upm -ne $null) {
                        $WebApplication = Get-SPWebApplication $upm.MySiteHostUrl
                        if ($WebApplication -ne $null){
                            $Id = $WebApplication.Id
                        }
                }
            }
        } Catch {
            $id = [Guid]::Empty
        } Finally {
            Set-Variable -Name "MyHostId" -Value ($id) -Scope Global -Option ReadOnly -Force
        }
    }
    $Id
}
#
function Build-ShowCentralAdmin(){
    start-process  "C:\Program Files\Common Files\microsoft shared\Web Server Extensions\15\BIN\psconfigui.exe" -ArgumentList  @("-cmd showcentraladmin")
}
#
function Build-RemoveDllsFromGAC(){
<#
.SYNOPSIS
    Removes DLLs from the GAC.

.DESCRIPTION
    Stops IIS and
    Removes DLLs from the GAC.
#>
    Write-Host "--Build-RemoveDllsFromGAC"
    #
    [string] $serviceName=""
    if (16 -eq $SPVersion){
        $serviceName = "ProjectEventService16"
    } elseif (15 -eq $SPVersion){
        $serviceName = "ProjectEventService15"
    }
    #
    $service = Get-Service -Name $serviceName -ErrorAction SilentlyContinue
    if ($service -ne $null) {
        if ($service.Status -eq "Stopped") {
            Write-Host "stop service $serviceName"
            Stop-Service -Name $serviceName
        }
    }
    iisreset /stop
    $GACDlls | %{
    Write-Host $_.Name
    gacutil /nologo /u $_.Name
    }
}
#
function Build-AddDllsToGAC(){
<#
.SYNOPSIS
    Adds DLLs into the GAC.

.DESCRIPTION
    Adds DLLs into the GAC
    and starts IIS.
#>
    Write-Host "--Build-AddDllsToGAC"
    $GACDlls | %{
    Write-Host $_.Path
    gacutil /nologo /i $_.Path
    }
    iisreset /start
    #
    [string] $serviceName=""
    if (16 -eq $SPVersion){
        $serviceName = "ProjectEventService16"
    } elseif (15 -eq $SPVersion){
        $serviceName = "ProjectEventService15"
    }
    $service = Get-Service -Name $serviceName -ErrorAction SilentlyContinue
    if ($service -ne $null) {
        if ($service.Status -eq "Running") {
            Write-Host "start service $serviceName"
            Start-Service -Name $serviceName
        }
    }
    Write-Host ([System.DateTime]::Now)
}
#
function Build-Compile(){
<#
.SYNOPSIS
    Compile the ots solution.

.DESCRIPTION
    The complete solution.
#>
    param(
        [Switch] $Rebuild,
        [Switch] $Clean,
        [Switch] $Log,
        [Switch] $OneCPU,
        [Switch] $TwoCPU
    )
    Write-Host "--Build-Compile"
    Set-Location -LiteralPath $slnDir
    [string] $t = "build"
    if ($Rebuild){
        $t = "rebuild"
    } elseif ($Clean){
        $t = "clean"
    }
    [string] $logging = ""
    if ($Log){
        $logging = "/fileLoggerParameters:LogFile=build.log;Verbosity=diagnostic;Encoding=UTF-8"
    }
    [string] $maxcpucount = ''
    if ($OneCPU -or $Rebuild){
        $maxcpucount = '/maxcpucount:1'
    } elseif ($TwoCPU) {
        $maxcpucount = '/maxcpucount:2'
    } else {
        $maxcpucount = '/maxcpucount'
    }
    if ($BuildConfiguration -eq "Release") {
        . 'msbuild.exe' '/verbosity:minimal' $maxcpucount ('/t:'+$t) ('/p:Configuration='+$BuildConfiguration) $logging ('"'+"$($slnDir)\$($SolutionFile)"+'"')
    } else {
        #msbuild.exe /verbosity:minimal /maxcpucount /t:$t /p:Configuration=$BuildConfiguration $logging ('"'+"$($slnDir)\$($SolutionFile)"+'"')
        . 'msbuild.exe' '/verbosity:minimal' $maxcpucount ('/t:'+$t) ('/p:Configuration='+$BuildConfiguration) '/consoleloggerparameters:Summary;Verbosity=quiet' $logging ('"'+"$($slnDir)\$($SolutionFile)"+'"')
    }
    if (-not $?) {throw "Build-Compile error" }
    if ($Clean){
        $obj=dir -LiteralPath $slnDir -Recurse -Include "obj" -Directory
        $obj|%{ Remove-Item $_.FullName -Recurse }
    }
    Write-Host ([System.DateTime]::Now)
}
#
function Build-WSP(){
<#
.SYNOPSIS
    Build the SharePoint Solutions WSP.

.DESCRIPTION
    Build the SharePoint Solutions WSP,
    this depended on the Build-SetConfiguration.
#>
    param(
        [string] $SP_csproj="",
        [string] $SP_WspPath="",
        [Switch] $Application,
        [Switch] $Frontend
    )
    Write-Host "-- Build-WSP"
    #
    if (($SP_csproj -ne "") -and ($SP_WspPath -ne "")) {
        Set-Location -LiteralPath $slnDir
        #
        Write-Host "-- Package"
        Write-Host "msbuild.exe /m $($SP_csproj) /t:Package /verbosity:minimal"
        msbuild.exe /p:Configuration=$BuildConfiguration "$SP_csproj" /t:Package /verbosity:minimal
        if (-not $?) {throw "Build-WSP error" }
        #
        Write-Host "-- ValidatePackage"
        Write-Host "msbuild.exe /m $($SP_csproj) /t:ValidatePackage /verbosity:minimal"
        msbuild.exe /p:Configuration=$BuildConfiguration "$SP_csproj" /t:ValidatePackage /verbosity:minimal
        if (-not $?) {throw "Build-WSP error" }
        #
        $SPLastWriteTime = (Get-Item -LiteralPath $SP_WspPath).LastWriteTime
        Write-Host "$SPLastWriteTime - $SP_WspPath"
    } else {
        if (-not($Application -or $Frontend)) {
            $Application=$true
            $Frontend=$true
        }
        if ($Application) {
            Build-WSP -SP_csproj $Application_csproj -SP_WspPath $Application_WspPath
        }
        if ($Frontend) {
            Build-WSP -SP_csproj $Frontend_csproj -SP_WspPath $Frontend_WspPath
        }
    }
    Write-Host ([System.DateTime]::Now)
}
#
function Build-AddSolution(){
    param(
        [string] $SP_WspName="",
        [string] $SP_WspPath="",
        [Switch] $Application,
        [Switch] $Frontend
    )

    if (($SP_WspName -ne "") -and ($SP_WspPath -ne "")) {
        $sol = Get-SPSolution -Identity $SP_WspName  -ErrorAction SilentlyContinue
        if ($sol -ne $null) { throw "Build-AddSolution error $SP_WspName already exists."}
        Write-Host "Add-SPSolution -LiteralPath $SP_WspPath"
        $sol = Add-SPSolution -LiteralPath $SP_WspPath
        if (-not $?) {throw "Build-AddSolution error" }
        return $sol
    } elseif (-not($Frontend -or $Application)) {
        Write-Host "Please specifiy the solution to add -TrackProject -TechAdmin -Client"
    } else {
        Build-StartSPServices
        if ($Frontend) {
            Build-AddSolution -SP_WspName $Frontend_WspName -SP_WspPath $Frontend_WspPath
        }
        if ($Application) {
            Build-AddSolution -SP_WspName $Application_WspName -SP_WspPath $Application_WspPath
        }
        Write-Host ([System.DateTime]::Now)
    }
}
#
function Build-InstallSolution(){
    param(
        [string] $SP_WspName="",
        [Switch] $Application,
        [Switch] $Frontend
    )

    if ($SP_WspName -ne "") {
        Build-StartSPServices
        $sol = Get-SPSolution -Identity $SP_WspName  -ErrorAction SilentlyContinue
        if ($sol -eq $null) { throw "Build-InstallSolution error $SP_WspName not exists."}

        if ($sol.ContainsWebApplicationResource) {
            $MyHostWebApplicationId = GetMyHostWebApplicationId
            $WebApplications=Get-SPWebApplication
            $WebApplications | ?{$_.Id -ne $MyHostWebApplicationId}| %{
                $WebApplication = $_
                Write-Host "Install-SPSolution -Identity $SP_WspName -WebApplication $($WebApplication.Url) -GACDeployment -FullTrustBinDeployment -Force"
                Install-SPSolution -Identity $SP_WspName -WebApplication $WebApplication -GACDeployment -FullTrustBinDeployment -Force
                $sol = Build-WaitForJobDone -Name $SP_WspName
            }
        } else {
            Write-Host "Install-SPSolution -Identity $SP_WspName -GACDeployment"
            $sol = Install-SPSolution -Identity $SP_WspName -GACDeployment
            $sol = Build-WaitForJobDone -Name $SP_WspName
        }
        return $sol
    } elseif (-not($Frontend -or $Application)) {
        Write-Host "Please specifiy the solution to install -TrackProject -TechAdmin -Client"
    } else {
        Build-StartSPServices
        if ($Frontend) {
            Build-InstallSolution -SP_WspName $Frontend_WspName
        }
        if ($Application) {
            Build-InstallSolution -SP_WspName $Application_WspName
        }
        Write-Host ([System.DateTime]::Now)
    }
}
#
function Build-UpdateSolution(){
    param(
        [string] $SP_WspName="",
        [string] $SP_WspPath="",
        [Switch] $Application,
        [Switch] $Frontend
    )
    if (($SP_WspName -ne "") -and ($SP_WspPath -ne "")) {
        $sol = Get-SPSolution -Identity $SP_WspName  -ErrorAction SilentlyContinue
        if ($sol -eq $null) { throw "Build-InstallSolution error $SP_WspName already exists."}
        Write-Host "Update-SPSolution -LiteralPath $SP_WspPath -Identity $SP_WspName -GACDeployment"
        $sol = Update-SPSolution -LiteralPath $SP_WspPath -Identity $SP_WspName -GACDeployment
        if ($sol -ne $null) { $sol = Build-WaitForJobDone -Name $SP_WspName }
        return $sol
    } else {
        Build-StartSPServices
        if ($Application) {
            Build-UpdateSolution -SP_WspName $Application_WspName -SP_WspPath $Application_WspPath
        }
        if ($Frontend) {
            Build-UpdateSolution -SP_WspName $Frontend_WspName -SP_WspPath $Frontend_WspPath
        }
        Write-Host ([System.DateTime]::Now)
    }
}
#
function Build-DeploySolution(){
    param(
        [string] $SP_WspName="",
        [string] $SP_WspPath="",
        [Switch] $Application,
        [Switch] $Frontend
    )
    if (($SP_WspName -ne "") -and ($SP_WspPath -ne "")) {
        $sol = Get-SPSolution -Identity $SP_WspName -ErrorAction SilentlyContinue
        if ($sol -ne $null) {
            if (-not $sol.Deployed -or $sol.DeployedServers.Count -eq 0) {
            Remove-SPSolution -Identity $SP_WspName
            $sol = Build-WaitForJobDone -Name $SP_WspName
            $sol = Get-SPSolution -Identity $SP_WspName -ErrorAction SilentlyContinue
            }
        }
        if ($sol -eq $null){
            Write-Host "Add-SPSolution -LiteralPath $SP_WspPath"
            $sol = Add-SPSolution -LiteralPath $SP_WspPath
            if ($sol -eq $null){ throw "Build-DeploySolution Error"}
            #
            if ($sol.ContainsWebApplicationResource) {
                Build-InstallSolution
            } else {
                Write-Host "Install-SPSolution -Identity $SP_WspName -GACDeployment"
                $sol = Install-SPSolution -Identity $SP_WspName -GACDeployment
                $sol = Build-WaitForJobDone -Name $SP_WspName
            }
        } else {
            Write-Host "Update-SPSolution -LiteralPath $SP_WspPath -Identity $SP_WspName -GACDeployment"
            $sol = Update-SPSolution -LiteralPath $SP_WspPath -Identity $SP_WspName -GACDeployment
            $sol = Build-WaitForJobDone -Name $SP_WspName
        }
        return $sol
    } else {
        Build-StartSPServices
        if ($Application) {
            Build-DeploySolution -SP_WspName $Application_WspName -SP_WspPath $Application_WspPath
        }
        if ($Frontend) {
            Build-DeploySolution -SP_WspName $Frontend_WspName -SP_WspPath $Frontend_WspPath
        }
        Write-Host ([System.DateTime]::Now)
    }
}
#
function Build-UninstallSolution(){
    param(
        [string] $SP_WspName="",
        [Switch] $Application,
        [Switch] $Frontend
    )
    if ($SP_WspName -ne "") {
        Build-StartSPServices
        $sol = Get-SPSolution -Identity $SP_WspName  -ErrorAction SilentlyContinue
        if ($sol -ne $null) {
            if ($sol.ContainsWebApplicationResource) {
                $WebApplications=Get-SPWebApplication -IncludeCentralAdministration
                $WebApplications | ?{-not $_.IsAdministrationWebApplication} | %{
                    $WebApplication = $_
                    Uninstall-SPSolution -Identity $SP_WspName -WebApplication $WebApplication
                    Build-WaitForJobDone -Name $SP_WspName | out-null
                }
                $WebApplications | ?{$_.IsAdministrationWebApplication} | %{
                    $WebApplication = $_
                    Uninstall-SPSolution -Identity $SP_WspName -WebApplication $WebApplication
                    Build-WaitForJobDone -Name $SP_WspName | out-null
                }
            } else {
                Write-Host "Uninstall-SPSolution -Identity $SP_WspName"
                Uninstall-SPSolution -Identity $SP_WspName  -ErrorAction SilentlyContinue
                Build-WaitForJobDone -Name $SP_WspName | Out-Null
            }
            #
            Write-Host "Remove-SPSolution -Identity $SP_WspName"
            Remove-SPSolution -Identity $SP_WspName -ErrorAction SilentlyContinue
            Write-Host ([System.DateTime]::Now)
        }
        return $sol
    } else {
        Build-StartSPServices
        if ($Application) {
            Build-UninstallSolution -SP_WspName $Application_WspName
        }
        if ($Frontend) {
            Build-UninstallSolution -SP_WspName $Frontend_WspName
        }
        Write-Host ([System.DateTime]::Now)
    }
}
#
function Build-TestGac(){
    Write-Host "--Build-TestGac"
    powershell -version 2.0 -NoLogo -ExecutionPolicy Unrestricted -Command ". $($buildDir)\build.ps1;Build-TestGacExternal"
    Write-Host "--Build-TestGac"
}
#
function Build-TestGacExternal(){
    $ok=$true
    $dll_Name=$OP_Name
    Write-Host $dll_Name
    $dll = [System.Reflection.Assembly]::LoadWithPartialName($dll_Name)
    if ($dll -eq $null) {
        Write-Error "$dll_Name not found"
        $ok = $false
    }
    if ($ok) {
        [System.Reflection.AssemblyName[]] $dlls = $dll.GetReferencedAssemblies()
        while (($dlls -ne $null) -and ($dlls.Length -gt 0)) {
            [System.Reflection.AssemblyName[]] $dlls = $dlls | %{
                if ($ok) {
                    Write-Host $_.FullName
                    $dll = [System.Reflection.Assembly]::Load($_)
                    if ($dll -eq $null){
                        Write-Error "$($_.FullName) not found"
                        $ok = $false
                    }else{
                        $dll.GetReferencedAssemblies()
                    }
                }
            }
        }
    }
    Write-Host "Press any key to continue..."
    [System.Console]::ReadKey() | Out-Null
}
#
function Build-Hefezopf(){
<#
.SYNOPSIS
    Build TTS

.DESCRIPTION
    Build TTS via ungac, compile, wsp, gac, undeploy, test in this order.
#>
    param(
        [Switch] $ungac,
        [Switch] $clean,
        [Switch] $compile,
        [Switch] $wsp,
        [Switch] $updateSolution,
        [Switch] $gac,
        [Switch] $deploy,
        [Switch] $undeploy,
        [Switch] $test,
        [Switch] $warmup,
        [Switch] $showcentraladmin,
        [string] $showUrl=""
    )
    process{
        if(-not($compile -or $clean -or $wsp -or $deploy -or $ungac -or $gac -or $test -or $warmup -or $showcentraladmin)) {
            $ungac=$true
            $compile=$true
            $gac=$true
            #$test=$true
        }
        $startDT = [System.DateTime]::Now
        if ($ungac){ Build-RemoveDllsFromGAC; if (-not $?){ return; }}
        if ($clean){ Build-Compile -Clean; }
        if ($compile){ Build-Compile; if (-not $?){ return; }}
        if ($wsp){ Build-WSP; if (-not $?){ return; }}
        if ($gac){ Build-AddDllsToGAC; if (-not $?){ return; }}
        if ($undeploy){ Build-UninstallSolution -Client -TechAdmin -TrackProject; if (-not $?){ return; }}
        if ($deploy){ Build-DeploySolution -Client -TechAdmin -TrackProject; if (-not $?){ return; }}
        if ($updateSolution) { Build-UpdateSolution -Client; if (-not $?){ return; }}
        if ($test){ Build-RunTest; if (-not $?){ return; }}
        if ($warmup){ Build-Warmup; }
        if ($showcentraladmin){ Build-ShowCentralAdmin; }
        if ($showUrl -ne "") { Start-Process $showUrl }
        $endDT = [System.DateTime]::Now
        $duration = $endDT.Subtract($startDT).TotalSeconds
        Write-Host ($endDT.ToString("s") + " Duration:"+$duration.ToString())
    }
}
#
function Build-GetProjectLinks() {
    $csprojs = New-Object -TypeName "System.Collections.ArrayList"
    dir $($slnDir) -Directory -Recurse | %{ $_.FullName } | %{
        dir -LiteralPath $_ -Filter "*.csproj" | % {
            $csprojs.Add($_.FullName) | Out-Null
        }
    }
    $p=""
    $ns = @{msbuild="http://schemas.microsoft.com/developer/msbuild/2003"}
    $csprojs | Sort-Object | %{
        $lp = $_
        $projectName = [System.IO.Path]::GetFileNameWithoutExtension($lp)
        $c = Get-Content -LiteralPath $lp
        [xml] $x = $c
        $links = Select-Xml -Xml $x -Namespace $ns -XPath "//msbuild:Link"
        $links | %{
            $link = $_
            $projectPath=$link.Node.InnerText
            $filePath=$link.Node.ParentNode.Include
            $prjDir=[System.IO.Path]::GetDirectoryName($lp)
            $fp=[System.IO.Path]::GetFullPath([System.IO.Path]::Combine($prjDir, $filePath))
            if (Test-Path $fp){
                #Write-Host "OK        : $projectName : $projectPath : $filePath : $fp"
                Write-Host "OK        : $projectName : $projectPath : $filePath "
            } else {
                Write-Host "NOT FOUND : $projectName : $projectPath : $filePath : $fp"
            }
        }
    }
}
#
function Build-Dependency(){
    $fileNamesExe = New-Object -TypeName "System.Collections.ArrayList"
    $fileNamesAss = New-Object -TypeName "System.Collections.ArrayList"
    #dir "$binDir\server" -Filter "*.exe" | %{ $fileNamesExe.Add($_.FullName);$fileNamesAss.Add($_.FullName) } | Out-Null
    #dir "$binDir\client" -Filter "*.exe" | %{ $fileNamesExe.Add($_.FullName);$fileNamesAss.Add($_.FullName) } | Out-Null
    #dir "$binDir\server" -Filter "*.dll" | %{ $fileNamesAss.Add($_.FullName) } | Out-Null
    #dir "$binDir\client" -Filter "*.dll" | %{ $fileNamesAss.Add($_.FullName) } | Out-Null
    #dir "$binDir\..\lib" -Filter "*.dll" | %{ $fileNamesAss.Add($_.FullName) } | Out-Null
    dir "$binDir\Debug" -Filter "*.exe" | %{ $fileNamesExe.Add($_.FullName);$fileNamesAss.Add($_.FullName) } | Out-Null
    dir "$binDir\Debug" -Filter "*.dll" | %{ $fileNamesAss.Add($_.FullName) } | Out-Null
    #
    $name2ass = @{}
    $name2deps = @{}
    #
    $asses = $fileNamesAss | %{
        $fn = $_
        $ass = [System.Reflection.Assembly]::ReflectionOnlyLoadFrom($fn)
        $fullName = $ass.GetName().FullName
        $name2ass[$fullName] = $ass
        $name2deps[$fullName] = @{}
        $ass
    }
    $asses | %{
        $ass = $_
        $fullName = $ass.FullName
        $refNames = $ass.GetReferencedAssemblies() | %{ $_.FullName } | ?{ -not $_.StartsWith("System.")} | ?{ -not $_.StartsWith("mscorlib,")} | ?{ -not $_.StartsWith("System,")}
        $refNames | %{ $name2deps[$fullName][$_]=1 }
    }
    #
    [string[]]$lines1 = $name2deps.Keys | %{
        $fullName = $_
        $deps = $name2deps[$fullName]
        [string]$shortName = $fullName.Split(',')[0]
        [string[]]$depsShortNames = $deps.Keys | %{ $_.Split(',')[0] + "@" + $deps[$_] } | Sort-Object
        [string]$depsShortNamesComa = ""
        if (($depsShortNames -ne $null) -and ($depsShortNames.Length -ne 0)) {
            $depsShortNamesComa = [string]::Join(", ", $depsShortNames)
        }
        "$($shortName) - $($depsShortNamesComa)"
    } | Sort-Object
    #
    $lines1 | Set-Content -LiteralPath "c:\temp\Build-Dependency1.txt"
    #
    $added = 1
    while($added -gt 0){
        $added = 0
        $asses | %{
            $ass = $_
            $fullName = $ass.FullName
            $deps = $name2deps[$fullName].Keys|Sort-Object
            $deps | %{
                $depName=$_
                if ($name2deps[$depName] -ne $null) {
                    $depsdeps=$name2deps[$depName].Keys|Sort-Object
                        $depsdeps | %{
                        if ($name2deps[$fullName][$_] -eq $null){
                            $name2deps[$fullName][$_] = 1+$name2deps[$fullName][$depName]
                            $added++
                        }
                    }
                }
            }
        }
    }
    #
    [string[]]$lines2 = $name2deps.Keys | %{
        $fullName = $_
        $deps = $name2deps[$fullName]
        [string]$shortName = $fullName.Split(',')[0]
        [string[]]$depsShortNames = $deps.Keys | %{ $_.Split(',')[0] + "@" + $deps[$_] } | Sort-Object
        [string]$depsShortNamesComa = ""
        if (($depsShortNames -ne $null) -and ($depsShortNames.Length -ne 0)) {
            $depsShortNamesComa = [string]::Join(", ", $depsShortNames)
        }
        "$($shortName) - $($depsShortNamesComa)"
    } | Sort-Object
    #
    $lines2 | Set-Content -LiteralPath "c:\temp\Build-Dependency2.txt"
    #
}
#
function Build-AssemblyInfos(){
<#
.SYNOPSIS
    Show AssemblyInfos

.DESCRIPTION
    FullName and ManifestResourceNames
#>
    param(
        [string] $FilePath
    )
    $a=[System.Reflection.Assembly]::LoadFile($FilePath)
    $a.GetName().FullName
    $a.GetManifestResourceNames()
}
#
function Build-TestEmbeddedAssemblies(){
    param(
        [string] $FilePath
    )
    $ass = [System.Reflection.Assembly]::LoadFile($FilePath)
    $typeLoader = ($ass.GetTypes() | ? {$_.Name -eq "EmbeddedResourceAssemblyLoader"} | Select-Object -First 1)[0]
    $activate=$typeLoader.GetMethod("Activate")
    $loader=$activate.Invoke($null, @())
    $typeService = ($ass.GetTypes() | ? {$_.Name.EndsWith("Service")} | Select-Object -First 1)[0]
    $propertyTypeAssemblyQualifedName = $typeService.GetProperty("TypeAssemblyQualifedName")
    $typeAssemblyQualifedName=$propertyTypeAssemblyQualifedName.GetValue($null)
    $t=[System.Type]::GetType($typeAssemblyQualifedName)
    if ($t -ne $null){
        Write-Host "Cannot load $typeAssemblyQualifedName ."
    } else {
        $exe=$t.Assembly
        $refs = $exe.GetReferencedAssemblies()
        $refsNames = $refs | ?{ $_.FullName.StartsWith("SOLVIN.") } | %{$_.FullName}
        $refsNames | %{
            $ass = [System.Reflection.Assembly]::Load($_)
            if ($ass -eq $null){
                Write-Host "Cannot load $_"
            }
        }
    }
    $loader
}
#
function Copy-Item-WithLog(){
    param(
        [string] $LiteralPath,
        [string] $Destination,
        [string] $Filter=$null,
        [Switch] $Recurse
    )
    Write-Host "Copy $LiteralPath to $Destination"
    Copy-Item -LiteralPath $LiteralPath -Destination $Destination -Recurse:$Recurse -Filter $Filter
    if (-not $?){ return; "Faulting" }
}
#
function Build-Install(){
    param(
        [Switch] $Debug,
        [Switch] $Release,
        [Switch] $DontCompile,
        [Switch] $Copy,
        [Switch] $CopyOverwrite
    )
    #
    if ($Release){
        Build-SetConfiguration -Configuration "Release"
    } elseif ($Debug){
        Build-SetConfiguration -Configuration "Debug"
    }
    if (-not $DontCompile) {
        if ($BuildConfiguration -eq "Debug") {
            Build-RemoveDllsFromGAC; if (-not $?){ return; }
        }
        Build-Compile; if (-not $?){ return; }
        Build-WSP; if (-not $?){ return; }
    }
    #
    $global:now = [System.DateTime]::Now.ToString("s").Replace(":", "-")
    $global:tempDir = [System.IO.Path]::Combine($rootDir, "temp" )
    if (-not ([System.IO.Directory]::Exists($tempDir))){[void][System.IO.Directory]::CreateDirectory($tempDir)}
    $global:tempDir = [System.IO.Path]::Combine($tempDir, $now )
    if (-not ([System.IO.Directory]::Exists($tempDir))){[void][System.IO.Directory]::CreateDirectory($tempDir)}
    #
    $global:ttsDir = [System.IO.Path]::Combine($tempDir, "Hefezopf")
    if (-not ([System.IO.Directory]::Exists($ttsDir))){[void][System.IO.Directory]::CreateDirectory($ttsDir)}
    #
   
    Write-Host ([System.DateTime]::Now)
}
#
if (([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator"))
{
    Write-Output 'Hint: Running as Administrator!'
}
else
{
    Write-Output 'WARNING: Running Limited!'
}

if ((Get-Command -Name "msbuild.exe" -ErrorAction SilentlyContinue) -eq $null) {
    Write-Host "try to load VsDevCmd.bat"
    . "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\Tools\VsDevCmd.bat"
}
if ((Get-Command -Name "msbuild.exe" -ErrorAction SilentlyContinue) -eq $null) {
    Write-Host "WARNING: not a developer shell"
} else {
    #build
}
Write-Host (Get-Bits)

function Build-Help(){
    Write-Host 'Build-Help - this help'
    #
    Write-Host ''
    Write-Host 'Build-Install -Release   -- compiles the release version'
    #
    Write-Host 'Build-SetConfiguration   -- set the configuration for msbuild -Release / -Debug'
    Write-Host 'Build-Compile            -- compile the solution'
    Write-Host 'Build-WSP                -- build the wsp'
    #
    Write-Host ''
    #
    Write-Host 'Build-RemoveDllsFromGAC  -- IIS stop; remove DLLs'
    Write-Host 'Build-AddDllsToGAC       -- add DLLs; IIS start'
    Write-Host ''
    #
    Write-Host 'Build-DisplaySolution    -- shows the installed solutions'
    Write-Host 'Build-AddSolution        -- add the solution'
    Write-Host 'Build-InstallSolution    -- install the solution'
    Write-Host 'Build-UpdateSolution     -- update the solution'
    Write-Host 'Build-DeploySolution     -- install or update the solution'
    Write-Host 'Build-UninstallSolution  -- uninstall and remove the solution'
    #
    Write-Host ''
    #
    Write-Host 'Build-Hefezopf -compile -wsp -deploy'
    Write-Host 'Build-TTS -compile -ungac -gac'
    #
    Write-Host ''
    Write-Host '#'
    Write-Host ''
    #
    Write-Host 'Build-DirLists'
    Write-Host ''
    Write-Host 'get-help Build-* --- to show all'
}
#
Push-Location
if (($host | select version).Version.Major -gt 1)  {$Host.Runspace.ThreadOptions = "ReuseThread"}
Set-Location 'C:\Program Files\Common Files\microsoft shared\Web Server Extensions'
Add-PSSnapin -Name Microsoft.SharePoint.PowerShell
Pop-Location
#
Build-SetConfiguration -Configuration "Debug"