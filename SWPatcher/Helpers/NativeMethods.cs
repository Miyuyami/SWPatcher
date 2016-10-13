using SWPatcher.RTPatch;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SWPatcher.Helpers
{
    public class NativeMethods
    {
        [Flags]
        public enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VirtualMemoryOperation = 0x00000008,
            VirtualMemoryRead = 0x00000010,
            VirtualMemoryWrite = 0x00000020,
            DuplicateHandle = 0x00000040,
            CreateProcess = 0x000000080,
            SetQuota = 0x00000100,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            QueryLimitedInformation = 0x00001000,
            Synchronize = 0x00100000
        }

        [DllImport("patchw32.dll", EntryPoint = "RTPatchApply32@12")]
        internal static extern uint RTPatchApply32(string command, RTPatchCallback func, bool waitFlag);

        [DllImport("patchw64.dll", EntryPoint = "RTPatchApply32")]
        internal static extern uint RTPatchApply64(string command, RTPatchCallback func, bool waitFlag);

        [DllImport("kernel32.dll")]
        internal static extern bool QueryFullProcessImageName(IntPtr hprocess, int dwFlags, StringBuilder lpExeName, out int size);

        [DllImport("kernel32.dll")]
        internal static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool CloseHandle(IntPtr hHandle);
    }
}
