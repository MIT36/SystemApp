using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace WorkerService
{
    public partial class Service : ServiceBase
    {
        Timer timer;
        //EventLog eventLog1;
        public Service()
        {
            InitializeComponent();
            timer = new Timer();
            timer.Interval = 10000; // 60 seconds
            timer.Elapsed += new ElapsedEventHandler(this.OnTimer);
            //eventLog1 = new EventLog();
            //if (!EventLog.SourceExists("MySource"))
            //{
            //    EventLog.CreateEventSource(
            //        "MySource", "MyNewLog");
            //}
            //eventLog1.Source = "MySource";
            //eventLog1.Log = "MyNewLog";
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                //eventLog1.WriteEntry("begin start");
                timer.Start();
                ProcessExtensions.ProcessExtensions.StartProcessAsCurrentUser(AppHelper.GetPathExeApp());
                //eventLog1.WriteEntry("end start");
            }
            catch(Exception ex)
            {
                //eventLog1.WriteEntry($"{ex.Message}\r\n{ex.StackTrace}", EventLogEntryType.Error);
            }
        }

        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            //File.WriteAllText("1sve.txt", "starting timer");
            try
            {
                ProcessExtensions.ProcessExtensions.StartProcessAsCurrentUser(AppHelper.GetPathExeApp());
            }
            catch(Exception ex)
            {
                //eventLog1.WriteEntry($"{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        protected override void OnStop()
        {
            timer?.Stop();
            timer?.Dispose();
            //eventLog1?.Dispose();
        }
    }
}
