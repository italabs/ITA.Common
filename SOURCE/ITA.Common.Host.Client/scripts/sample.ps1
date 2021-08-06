#Test script
Import-Module D:\BuildRoot\ITA\COMPONENT\ITA.Common\REL_3.0.0\SOURCE\Release\AnyCPU\ITA.Common.Host.Client.dll
get-Help get-ServiceHost -full

# Getting HOST and dump automatically
get-ServiceHost -service ITAEngine

# Getting HOST and dump manually 
$h = get-ServiceHost -service ITAEngine

Write-Host OnBatteryAction = $h.OnBatteryAction
Write-Host OnLowBatteryAction = $h.OnLowBatteryAction
Write-Host OnSuspendAction = $h.OnSuspendAction
Write-Host EnableSuspend = $h.EnableSuspend
Write-Host AutoStart = $h.AutoStart
Write-Host InstanceID = $h.InstanceID
Write-Host InstanceName = $h.InstanceName
Write-Host ServiceName = $h.ServiceName
Write-Host ServiceDisplayName = $h.ServiceDisplayName
Write-Host ServiceStatus = $h.ServiceStatus

# Starting HOST
get-ServiceHost -service ITAEngine | start-ServiceHost
#get-ServiceHost -service ITAEngine | set-ServiceHost -enableSuspend 1
#get-ServiceHost -service ITAEngine
wait-ServiceHost -status Running -timeout 10000 -input $h