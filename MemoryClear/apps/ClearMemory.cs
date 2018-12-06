using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MemoryClear
{
   public  class ClearMemory
    {
        [DllImport("psapi.dll")]
        static extern int EmptyWorkingSet(IntPtr hwProc);
        public  void Clear()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes)
            {
                //对于系统进程会拒绝访问，导致出错，此处对异常不进行处理。
                try
                {

                    EmptyWorkingSet(process.Handle);
                }
                catch
                {
                }
            }
           // Thread.Sleep(2000);
           
        }
    }
}
