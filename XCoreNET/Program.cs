using Global;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using static Global.gb;


namespace XCoreNET
{
    static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ProgramModel pm = null;
            CoreWebView2Environment.LoaderDllFolderPath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + "/runtimes/win-x86/native");

            string path = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + "/settings/programs_settings.json");
            if (File.Exists(path))
            {
                var data = File.ReadAllText(path);
                pm = JsonConvert.DeserializeObject<ProgramModel>(data);
                gb.launcherHomepage = new Uri(pm.launcherURL);
                gb.mainHomepage = new Uri(pm.mainURL);
            }
            else
            {
                pm = new ProgramModel();
                pm.launcher = true;
                pm.noWevView = false;
                pm.launcherURL = gb.launcherHomepage.ToString();
                pm.mainURL = gb.mainHomepage.ToString();

                var data = JsonConvert.SerializeObject(pm);
                Directory.CreateDirectory(Path.GetFullPath(Directory.GetCurrentDirectory() + "/settings"));
                File.WriteAllText(path, data);
            }

            if (args.Length == 0)
            {
                if (pm != null)
                {
                    var nowArgs = new List<string>();
                    if (pm.launcher)
                        nowArgs.Add("-launcher");
                    if (pm.noWevView)
                        nowArgs.Add("-noWebView");

                    if (pm.launcher)
                    {
                        Console.WriteLine("初始化啟動器進入點");
                        Application.Run(new minecraftForm(nowArgs.ToArray()));
                    }
                    else
                    {
                        Application.Run(new main(nowArgs.ToArray()));
                    }
                }
                else
                {
                    Application.Run(new main());
                }
            }
            else
            {
                bool onlyLauncher = false;
                foreach (var arg in args)
                {
                    if (arg.ToLower().Equals("-launcher"))
                    {
                        Console.WriteLine("初始化啟動器進入點");
                        onlyLauncher = true;
                    }
                }

                if (onlyLauncher)
                    Application.Run(new minecraftForm(args));
                else
                    Application.Run(new main(args));
            }
        }
    }
}
