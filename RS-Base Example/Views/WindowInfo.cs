using MaterialDesignThemes.Wpf;
using System;
using System.Windows;

namespace RS_Base.Views
{
    public partial class WindowManager
    {
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
        
        
    }
}