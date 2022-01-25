// See https://aka.ms/new-console-template for more information
using MacroRestarter;
using System.Diagnostics;
using System.Runtime.InteropServices;


#region User32
[DllImport("user32.dll")]
static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
IntPtr HWND_BOTTOM = new IntPtr(1); //stay on bottom pointer
const UInt32 SWP_NOSIZE = 0x0001;
const UInt32 SWP_NOMOVE = 0x0002;
const UInt32 SWP_SHOWWINDOW = 0x0040;
IntPtr HWND_TOPMOST = new IntPtr(-1);// stay on top pointers
#endregion

Process[] brave = Process.GetProcessesByName("brave");
Process[] adb = Process.GetProcessesByName("adb");



ConsoleWindow.QuickEditMode(false);
Console.ForegroundColor= ConsoleColor.DarkCyan;
string currdir = AppDomain.CurrentDomain.BaseDirectory+"\\ErrorLog.txt";

string macrofilename = AppDomain.CurrentDomain.BaseDirectory + @"\macro.exe";
Process proc = new Process();
proc.StartInfo.Arguments = null;
proc.StartInfo.WindowStyle = ProcessWindowStyle.Minimized; //start minimized
proc.StartInfo.FileName = macrofilename;
proc.StartInfo.UseShellExecute = true;

var backgroundprocess = Process.GetProcessesByName("macro").Where(pr => pr.MainWindowHandle != IntPtr.Zero);
Process[] processes = Process.GetProcessesByName("macro");

var patcherproc = Process.Start(AppDomain.CurrentDomain.BaseDirectory + @"\patcher.exe");
try
{
    Console.WriteLine("waiting 1 min before startup");
    Thread.Sleep(TimeSpan.FromMinutes(1)); //wait 1 min to startup all applications
    while (true)
    {
        #region hidetobottom
        foreach (Process p in brave) //brave browser   
        {
            IntPtr windowHandle = p.MainWindowHandle; //send to top
            SetWindowPos(windowHandle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
        }
        foreach (Process p in adb) //adb emulator, do we need to add nox?
        {
            IntPtr windowHandle = p.MainWindowHandle; //send to bottom
            SetWindowPos(windowHandle, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
        }
        #endregion
        Console.Clear();
        if (processes.Length == 0)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("macro has crashed, restarting macro");
            foreach (var process in Process.GetProcessesByName("macro"))
            {
                Console.WriteLine("Closing " + process);
                //process.Kill(); stop closing because they crash anyway hopefully they naturally crash
            }
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("Waiting for patcher to finish!");
            patcherproc.WaitForExit();
            //Process.Start(AppDomain.CurrentDomain.BaseDirectory + @"\macro.exe");
            proc.Start(); //start macro
        }
        else //if its open close it in the background
        {
            Console.WriteLine("Macro running in background");
            foreach (Process procc in backgroundprocess)
            {
                Console.WriteLine("closing  " + procc.ProcessName + " in background");
                //procc.Kill(); stop closing because they crash anyway hopefully they naturally crash
            }
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("Waiting for patcher to finish!");
            patcherproc.WaitForExit();
            //Process.Start(AppDomain.CurrentDomain.BaseDirectory + @"\macro.exe");
            proc.Start(); //start macro

        }
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Restarting Macro");
        Thread.Sleep(TimeSpan.FromMinutes(5)); //run every 5min otherwise it crashes preemptively
    }
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Saving Errors to Log");
    using (StreamWriter sw = new StreamWriter(currdir, true))
    { // If file exists, text will be appended ; otherwise a new file will be created
        sw.Write(string.Format("Message: {0}<br />{1}StackTrace :{2}{1}Date :{3}{1}-----------------------------------------------------------------------------{1}", ex.Message, Environment.NewLine, ex.StackTrace, DateTime.Now.ToString()));
    }
}










