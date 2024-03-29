﻿using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.Messaging;
using RS_Base.Net.Helper;
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
            Log.Information("STARTING APPLICATION...");
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.Current.DispatcherUnhandledException += ThreadStuffUI;
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
        }
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // Log the exception, display it, etc
            Log.Error((e.ExceptionObject as Exception), "CurrentDomain_UnhandledException!!!");
        }
        /// <summary>
        /// This often finds weird threading errors in the UI.
        /// </summary>
        private void ThreadStuffUI(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Log.Error(e.Exception, "Some UI Error!");
        }
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var vmLoc = new ViewModelLocator();
            vmLoc.WindowManager.CreateMainWindow();
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

    public class StealthConsoleSink : ILogEventSink
    {
        readonly IFormatProvider _formatProvider;

        public StealthConsoleSink(IFormatProvider formatProvider)
        {
            _formatProvider = formatProvider;
        }

        public void Emit(LogEvent logEvent)
        {
            WeakReferenceMessenger.Default.Send(logEvent.RenderMessage(_formatProvider), MessengerID.LogFrontEndMessage);
        }
    }
}
