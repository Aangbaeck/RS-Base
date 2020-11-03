using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using RS_Base.Net;
using RS_Base.Net.Helper;
using RS_Base.Services;
using RS_Base.Views;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;

namespace RS_Base
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.File(Common.LogfilesPath + "Logfile.log", rollOnFileSizeLimit: true, fileSizeLimitBytes: 20000000, retainedFileCountLimit: 5)
                .WriteTo.Logger(lc => lc.Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Error).WriteTo.StealthConsoleSink())
                .CreateLogger();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            //We only want one application to run.
            Process current = Process.GetCurrentProcess();
            // get all the processes with current process name
            Process[] processes = Process.GetProcessesByName(current.ProcessName);
            foreach (Process process in processes)
            {
                //Ignore the current process  
                if (process.Id != current.Id)
                {
                    process.Kill();
                }
            }
            //DispatcherHelper.Initialize();
        }
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // Log the exception, display it, etc
            Log.Error((e.ExceptionObject as Exception), "CurrentDomain_UnhandledException!!!");
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            new ViewModelLocator();
            new MainV().Show();
        }
    }
    public static class StealthConsoleSinkExtensions
    {
        public static LoggerConfiguration StealthConsoleSink(
            this LoggerSinkConfiguration loggerConfiguration,
            IFormatProvider fmtProvider = null)
        {
            return loggerConfiguration.Sink(new StealthConsoleSink(fmtProvider));
        }
    }

    /// <summary>
    /// Used for catching the log messages inside the application. Just Register for the <see cref="MessengerID.LogFrontEndMessage" /> to grab it.
    /// </summary>
    public class StealthConsoleSink : ILogEventSink
    {
        readonly IFormatProvider _formatProvider;

        public StealthConsoleSink(IFormatProvider formatProvider)
        {
            _formatProvider = formatProvider;
        }

        public void Emit(LogEvent logEvent)
        {
            Messenger.Default.Send(logEvent.RenderMessage(_formatProvider), MessengerID.LogFrontEndMessage);
        }
    }
}
