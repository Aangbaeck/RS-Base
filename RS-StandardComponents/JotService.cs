using Jot;
using Jot.Storage;
using RS_StandardComponents;
using System.IO;
using System.Reflection;
using System.Windows;

static class JotService
{
    // expose the tracker instance
    public static Tracker Tracker = new Tracker(new JsonFileStore(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "//WindowPositions//"));

    static JotService()
    {
        // tell Jot how to track Window objects
        Tracker.Configure<RSView>()
            .Id(w => w.Title
            .Replace(":","")
            .Replace("<", "")
            .Replace(">", "")
            .Replace(@"""", "")
            .Replace(@"\", "")
            .Replace("/", "")
            .Replace("?", "")
            .Replace("*", "")
            .Replace("|", ""), SystemParameters.WorkArea.Size) // <-- include the screen resolution in the id
            .Properties(w => new { w.WindowState, w.Top, w.Width, w.Height, w.Left })
            .PersistOn(nameof(RSView.Closing))
            .StopTrackingOn(nameof(RSView.Closing));
    }
}