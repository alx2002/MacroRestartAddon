// See https://aka.ms/new-console-template for more information
using MacroRestarter;
using System.Diagnostics;

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
    while (true)
    {
        Console.Clear();
        if (processes.Length == 0)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("macro has crashed, restarting macro");
            foreach (var process in Process.GetProcessesByName("macro"))
            {
                Console.WriteLine("Closing " + process);
                process.Kill();
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
                procc.Kill();
            }
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("Waiting for patcher to finish!");
            patcherproc.WaitForExit();
            //Process.Start(AppDomain.CurrentDomain.BaseDirectory + @"\macro.exe");
            proc.Start(); //start macro

        }
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Restarting Macro");
        Thread.Sleep(TimeSpan.FromMinutes(5)); //run every 5min
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










