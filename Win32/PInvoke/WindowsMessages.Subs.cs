namespace XJK.Win32.PInvoke
{
    /// <summary>
    /// power-management event, use with WM_POWERBROADCAST
    /// </summary>
    public enum PBT : uint
    {
        /// <summary>
        /// Power status has changed.
        /// </summary>
        APMPOWERSTATUSCHANGE = 0x000A,
        /// <summary>
        /// Operation is resuming automatically from a low-power state. This message is sent every time the system resumes.
        /// </summary>
        APMRESUMEAUTOMATIC = 0x0012,
        /// <summary>
        /// Operation is resuming from a low-power state. This message is sent after PBT_APMRESUMEAUTOMATIC if the resume is triggered by user input, such as pressing a key.
        /// </summary>
        APMRESUMESUSPEND = 0x0007,
        /// <summary>
        /// System is suspending operation.
        /// </summary>
        APMSUSPEND = 0x0004,
        /// <summary>
        /// A power setting change event has been received. 
        /// </summary>
        POWERSETTINGCHANGE = 0x8013,
    }
}
