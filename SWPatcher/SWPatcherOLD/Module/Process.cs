using System.Runtime.InteropServices;
using System.Text;

namespace SWPatcher.Module
{
    class Process
    {
        static public bool isAnyRunning(string processName)
        {
            foreach (System.Diagnostics.Process node in System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(processName.ToLower())))
                if (node.MainModule.FileName.ToLower().EndsWith(processName.ToLower()))
                    return true;
            return false;
        }

        static public System.Diagnostics.Process getRunning(string processName)
        {
            foreach (System.Diagnostics.Process node in System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(processName.ToLower())))
                if (GetExecutablePath(node).ToLower().EndsWith(processName.ToLower()))
                    return node;
            return null;
        }

        static public System.Collections.Generic.List<System.Diagnostics.Process> getAllRunning(string processName)
        {
            System.Collections.Generic.List<System.Diagnostics.Process> theList = new System.Collections.Generic.List<System.Diagnostics.Process>();
            foreach (System.Diagnostics.Process node in System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(processName.ToLower())))
                if (GetExecutablePath(node).ToLower().EndsWith(processName.ToLower()))
                    theList.Add(node);
            return theList;
        }

        private static string GetExecutablePath(System.Diagnostics.Process process)
        {
            //If running on Vista or later use the new function
            if (System.Environment.OSVersion.Version.Major >= 6)
                return GetExecutablePathAboveVista(process.Id);
            return process.MainModule.FileName;
        }

        private static string GetExecutablePathAboveVista(int ProcessId)
        {
            var buffer = new StringBuilder(1024);
            string result = null;
            System.IntPtr hprocess = OpenProcess(ProcessAccessFlags.QueryLimitedInformation, false, ProcessId);
            if (hprocess != System.IntPtr.Zero)
            {
                try
                {
                    int size = buffer.Capacity;
                    if (QueryFullProcessImageName(hprocess, 0, buffer, out size))
                    {
                        result = buffer.ToString();
                    }
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    CloseHandle(hprocess);
                }
            }
            buffer.Clear();
            return result;
        }

        [DllImport("kernel32.dll")]
        private static extern bool QueryFullProcessImageName(System.IntPtr hprocess, int dwFlags,
               StringBuilder lpExeName, out int size);
        [DllImport("kernel32.dll")]
        private static extern System.IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess,
                       bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(System.IntPtr hHandle);

        [System.Flags]
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
    }
}
