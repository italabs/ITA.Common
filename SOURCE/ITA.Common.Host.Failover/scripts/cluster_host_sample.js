//
// Mock for cluster object "Resource"
// Uncomment it to test the script outside of cluster environment, for example via cscript.exe
//

/*
function ResourceMock() 
{
    this.LogInformation = function(msg) 
    {
        WScript.Echo(msg);
    };
} 

var Resource = new ResourceMock();
*/

//
// Main script
//

Resource.LogInformation("Creating FailoverClusterResource object");

var host = new ActiveXObject("ITA.Common.Host.Failover.FailoverClusterResource");
host.Url = "net.pipe://localhost/ITAEngine/default";
host.WindowsService = "ITAEngineSvc_default";
host.WaitTimeout = 30000;
host.ControlWindowsService = true; // true - it starts both w32 service and logically, false - logically only

function Online ()
{
    Resource.LogInformation ("Entering Online");

    var retVal = host.Online ();

    Resource.LogInformation ("Leaving Online:" + retVal);

    return retVal;
}

function Offline ()
{
    Resource.LogInformation ("Entering Offline");

    var retVal = host.Offline ();

    Resource.LogInformation ("Leaving Offline:" + retVal);

    return retVal;
}

function LooksAlive ()
{
    Resource.LogInformation ("Entering LooksAlive");

    var retVal = host.LooksAlive ();

    Resource.LogInformation ("Leaving LooksAlive:" + retVal);

    return retVal;
}

function IsAlive ()
{
    Resource.LogInformation ("Entering IsAlive");

    var retVal = host.IsAlive ();

    Resource.LogInformation ("Leaving IsAlive:" + retVal);
    
    return retVal;
}

function Open ()
{
    Resource.LogInformation ("Entering Open");

    var retVal = host.Open ();

    Resource.LogInformation ("Leaving Open:" + retVal);

    return retVal;
}

function Close ()
{
    Resource.LogInformation ("Entering Close");

    var retVal = host.Close ();

    Resource.LogInformation ("Leaving Close:" + retVal);

    return retVal;
}

function Terminate ()
{
    Resource.LogInformation ("Entering Terminate");

    var retVal = host.Terminate ();

    Resource.LogInformation ("Leaving Terminate:" + retVal);

    return retVal;
}

function Test ()
{
    Resource.LogInformation("Creating FailoverClusterResource object - done");

    Resource.LogInformation("Testing LooksAlive");
    WScript.Echo ("IsAlive="+LooksAlive ());

    Resource.LogInformation("Testing IsAlive");
    WScript.Echo ("IsAlive="+IsAlive ());

    Resource.LogInformation("Testing Online");
    WScript.Echo (Online ());
    WScript.Echo ("IsAlive="+IsAlive ());

    Resource.LogInformation("Testing Offline");
    WScript.Echo (Offline ());
    WScript.Echo ("IsAlive="+IsAlive ());

    Resource.LogInformation("Testing Online");
    WScript.Echo (Online ());
    WScript.Echo ("IsAlive="+IsAlive ());
}

//Test ();
