// See https://aka.ms/new-console-template for more information
using MacroRestarter;
using System.Diagnostics;

ConsoleWindow.QuickEditMode(false);
Console.ForegroundColor= ConsoleColor.DarkCyan;
string currdir = AppDomain.CurrentDomain.BaseDirectory+"\\ErrorLog.txt";
try
{
    while (true)
    {
        var backgroundprocess = Process.GetProcesses().Where(pr => pr.MainWindowHandle != IntPtr.Zero);
        Process[] processes = Process.GetProcessesByName("macro");
        if (processes.Length == 0)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("macro has crashed, restarting macro");
            foreach (var process in Process.GetProcessesByName("macro"))
            {
                process.Kill();
            }
            var patcherproc = Process.Start(AppDomain.CurrentDomain.BaseDirectory + @"\patcher.exe");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("Waiting for patcher to finish!");
            patcherproc.WaitForExit();
            Process.Start(AppDomain.CurrentDomain.BaseDirectory + @"\macro.exe");
        }
        else //if its open close it in the background
        {
            Console.WriteLine("Macro running in background");
            foreach (Process proc in backgroundprocess)
            {
                Console.WriteLine("closing macro in background " + proc.ProcessName);
                proc.Kill();
            }
            var patcherproc = Process.Start(AppDomain.CurrentDomain.BaseDirectory + @"\patcher.exe");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("Waiting for patcher to finish!");
            patcherproc.WaitForExit();
            Process.Start(AppDomain.CurrentDomain.BaseDirectory + @"\macro.exe");
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










