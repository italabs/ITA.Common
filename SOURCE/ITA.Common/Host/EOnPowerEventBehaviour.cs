namespace ITA.Common.Host
{
    public enum EOnPowerEventBehaviour
    {
        Ignore, // Continue doing what it's doing
        Pause,  // Pause & Continue upon restoration. Crypto storage remains mounted
        Stop    // Stop & Start upon restoration. Crypto storage is dismounted
    }
}
