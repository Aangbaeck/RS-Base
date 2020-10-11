using Jot;
using Jot.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;


namespace RS_StandardComponents
{

    static class Services
    {
        // expose the tracker instance
        public static Tracker Tracker = new Tracker(new JsonFileStore(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "//" + "WindowPositions/"));

        static Services()
        {
            //tell Jot how to track Window objects
            Tracker.Configure<TitlebarUserCtrl>()
            .Id(w => w.Name + "_" + RemoveInvalidCharactes(w.Title) + "_" + SystemParameters.PrimaryScreenWidth + "x" + SystemParameters.PrimaryScreenHeight) // <-- include the screen resolution in the id  //TODO cant get unique window ids
            .Properties(w => new { w.BoundWindow.Top, w.BoundWindow.Width, w.BoundWindow.Height, w.BoundWindow.Left, w.BoundWindow.WindowState })
            .PersistOn(nameof(TitlebarUserCtrl.Closing))
            .StopTrackingOn(nameof(TitlebarUserCtrl.Closing));
        }

        static string RemoveInvalidCharactes(string strIn)
        {
            if (strIn == null)
                return "";
            // Replace invalid characters with empty strings.
            string illegal = @"<>:""/\|?* ";
            foreach (char c in illegal)
            {
                strIn = strIn.Replace(c.ToString(), "");
            }
            return strIn;
        }
    }

}
