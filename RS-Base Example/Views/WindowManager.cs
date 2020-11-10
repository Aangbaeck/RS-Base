using GalaSoft.MvvmLight;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using RS_StandardComponents;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace RS_Base.Views
{
    public class WindowManager : ObservableObject
    {
        public bool AllWindowsAreEditable { get; private set; }
        public RSWindow MainWindow { get; private set; }

        public void CreateMainWindow()
        {
            
            var win = OpenWindowButLoadOldSettings("Main title", typeof(MainV), icon:PackIconKind.AboutCircle, sizeToContent: SizeToContent.Width, enableClosable: true, enableMaximize: true, enableMinimize: true);
            MainWindow = win;
            win.Closing += SaveOpenWindows;

            if (File.Exists(AllWindowPath))
            {
                var json = File.ReadAllText(AllWindowPath);
                try
                {
                    var winInfoList = JsonConvert.DeserializeObject<WindowInfo[]>(json);
                    foreach (var winInfo in winInfoList)
                    {
                        OpenWindowButLoadOldSettings(winInfo.Title, winInfo.Content);
                    }
                }
                catch (Exception e)
                {
                    Log.Error("Could not restore windows. Something has changed...", e);
                }
            }
        }
        public void EnableEditAllWindows(bool makeEditable)
        {
            AllWindowsAreEditable = makeEditable;
            foreach (var item in WindowList.Values)
            {
                item.win.Titlebar.EnablePinMode = makeEditable;
            }

            if (!makeEditable)
            {
                foreach (var item in WindowList.Values)
                {
                    item.info.ShouldBePinned = item.win.Titlebar.IsPinned;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="title">Name of the window. Can not contain illegal file characters because a file with the same name is created to keep track of position and other stuff</param>
        /// <param name="tag">parameter(s) to be sent in with the constructor</param>
        /// <param name="sizeToContent"></param>
        /// <param name="shouldBeRestoredAtStartup"></param>
        /// <param name="enableClosable"></param>
        /// <param name="enableMaximize"></param>
        /// <param name="enableMinimize"></param>
        /// <returns></returns>
        public RSWindow OpenWindowButLoadOldSettings(string title, Type content, object tag = null, PackIconKind icon = PackIconKind.Accelerometer, SizeToContent sizeToContent = SizeToContent.Manual, bool shouldBePinned = false, bool enableClosable = true, bool enableMaximize = true, bool enableMinimize = true)
        {
            Rect workArea = SystemParameters.WorkArea;
            var winInfo = new WindowInfo(title) { Content = content, Icon=icon, Tag = tag, SizeToContent = sizeToContent, ShouldBePinned = shouldBePinned, EnableClosable = enableClosable, EnableMaximize = enableMaximize, EnableMinimize = enableMinimize, WorkArea = workArea };
            var path = FilePath(title);
            if (File.Exists(path))
            {
                try
                {
                    var json = File.ReadAllText(path);
                    var winInfoLoaded = JsonConvert.DeserializeObject<WindowInfo>(json);

                    if (winInfoLoaded?.WorkArea?.GetHashCode() == workArea.GetHashCode())  //This means that the screen resolutionn is the same as last time.
                        winInfo = winInfoLoaded;
                }
                catch (Exception e)
                {
                    Log.Error("Could not deserialize old window file. Will create new one when closing...", e);
                }
            }
            return OpenWindow(winInfo);
        }
        public RSWindow OpenWindow(WindowInfo winInfo)
        {
            //string title, object tag = null, SizeToContent sizeToContent = SizeToContent.Manual, bool shouldBeRestoredAtStartup = false
            var win = new RSWindow();
            try
            {
                object content;
                //This injects the tag into the View constructor. Nice when the view need a specific Viewmodel.
                //Then it can use the Tag to identify which Viewmodel it should choose from in the Viewmodellocator.
                if (winInfo.Tag != null)
                    content = Activator.CreateInstance(winInfo.Content, winInfo.Tag);
                else
                    content = Activator.CreateInstance(winInfo.Content);
                win.SetContent(content);
                win.SetTitle(winInfo.Title);
                win.SetIcon(winInfo.Icon);

                win.Titlebar.EnableClosable = winInfo.EnableClosable;
                win.Titlebar.EnableMaximize = winInfo.EnableMaximize;
                win.Titlebar.EnableMinimize = winInfo.EnableMinimize;
                win.Titlebar.IsPinned = winInfo.ShouldBePinned;

                if (winInfo.Height.HasValue) win.Height = winInfo.Height.GetValueOrDefault();
                if (winInfo.Width.HasValue) win.Width = winInfo.Width.GetValueOrDefault();
                if (winInfo.Left.HasValue) win.Left = winInfo.Left.GetValueOrDefault();
                if (winInfo.Top.HasValue) win.Top = winInfo.Top.GetValueOrDefault();

                win.WindowState = winInfo.WindowState;
                win.SizeToContent = winInfo.SizeToContent;
                win.Owner = MainWindow;

                WindowList.TryGetValue(win.Title, out var openWin);
                if (openWin.win != null)
                {
                    openWin.win.Show();
                    openWin.win.WindowState = WindowState.Normal;
                    openWin.win.Activate();
                    win = openWin.win;
                }
                else
                {
                    win.Show();
                    WindowList.Add(win.Title, (win, winInfo));
                    win.Closing += (obj, e) => RememberMyState(obj, winInfo);
                    win.Activate();
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Could not open window due to error.");
            }
            return win;
        }
        public class WindowInfo
        {
            public WindowInfo(string title)
            {
                Title = title;
            }
            public Type Content { get; set; }
            public PackIconKind Icon { get; set; }
            public bool EnableClosable { get; set; }
            public bool EnableMaximize { get; set; }
            public bool EnableMinimize { get; set; }
            public double? Height { get; set; }
            public double? Left { get; set; }
            public bool ShouldBePinned { get; set; }
            public SizeToContent SizeToContent { get; set; }
            /// <summary>
            /// This is an extra ID for identifying which Datacontext the view should choose. If this is != null, it will be injected in the constructor of the view.
            /// </summary>
            public object Tag { get; set; }
            public string Title { get; set; }
            public double? Top { get; set; }
            public double? Width { get; set; }
            public WindowState WindowState { get; set; }
            public Rect? WorkArea { get; set; }
        }
        private string AllWindowPath => WPath + "WindowsToOpen.state";
        private Dictionary<string, (RSWindow win, WindowInfo info)> WindowList { get; } = new Dictionary<string, (RSWindow, WindowInfo)>();
        private string WPath => Directory.CreateDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "//" + "WindowPositions//").FullName;
        private string FilePath(string title) => WPath + title + ".state";
        private void RememberMyState(object obj, WindowInfo winInfo)
        {
            RSWindow win = (RSWindow)obj;

            //Save state
            winInfo.Top = win.Top;
            winInfo.Height = win.Height;
            winInfo.Left = win.Left;
            winInfo.Width = win.Width;
            winInfo.WindowState = win.WindowState;
            winInfo.ShouldBePinned = win.Titlebar.IsPinned;
            winInfo.WorkArea = SystemParameters.WorkArea;
            var jsonString = JsonConvert.SerializeObject(winInfo);
            File.WriteAllText(FilePath(win.Title), jsonString);

            WindowList.Remove(win.Title);
        }
        private void SaveOpenWindows(object sender, CancelEventArgs e)
        {
            foreach (var item in WindowList.Values)
            {
                item.info.ShouldBePinned = item.win.Titlebar.IsPinned;
            }
            var winListCopy = WindowList.Select(s => s.Value).ToArray(); //Need to copy the list since the items remove itself from the list and we get a concurrency error otherwise
            var jsonString = JsonConvert.SerializeObject(winListCopy.Where(w => w.info.ShouldBePinned).Select(s => s.info).ToArray());
            File.WriteAllText(AllWindowPath, jsonString);
            foreach (var win in winListCopy)
            {
                win.win.Close();
            }
        }
    }
}