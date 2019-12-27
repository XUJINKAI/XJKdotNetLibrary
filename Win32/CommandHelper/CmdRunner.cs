namespace XJK.Win32.CommandHelper
{
    public static class CmdRunner
    {
        public static ProcessInfoChain New(string Command, string Args)
        {
            return ProcessInfoChain.New(Command, Args);
        }

        public static ExcuteResult RunAsInvoker(string Command, string Args)
        {
            return ProcessInfoChain.New(Command, Args).Excute();
        }

        public static ExcuteResult RunAsLimited(string Command, string Args)
        {
            return ProcessInfoChain.New(Command, Args).RunAs(Privilege.Limited).Excute();
        }

        public static ExcuteResult RunAsAdmin(string Command, string Args)
        {
            return ProcessInfoChain.New(Command, Args).RunAs(Privilege.Admin).Excute();
        }

        public static ExcuteResult RunWithCmdStart(string Command, string Args)
        {
            return ProcessInfoChain.New(Command, Args).LaunchBy(LaunchType.CmdStart).Excute();
        }
    }

}
