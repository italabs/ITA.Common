Option Explicit

'Cluster resource entry points. More details here:
'http://msdn.microsoft.com/en-us/library/aa372846(VS.85).aspx

'Cluster resource Online entry point
'Make sure the FTP service is started
Function Online( )

    Online = true 

End Function

 
'Cluster resource offline entry point
'On offline, do nothing.
Function Offline( )

    Offline = true

End Function


'Cluster resource LooksAlive entry point
'Check for the state of the FTP service
Function LooksAlive( )

	LooksAlive = True

End Function


'Cluster resource IsAlive entry point
'Do the same health checks as LooksAlive
'If a more thorough than what we do in LooksAlive is required, this should be performed here
Function IsAlive()   

    IsAlive = LooksAlive

End Function


'Cluster resource Open entry point
Function Open()

    Open = true

End Function


'Cluster resource Close entry point
Function Close()

    Close = true

End Function


'Cluster resource Terminate entry point
Function Terminate()

    Terminate = true

End Function