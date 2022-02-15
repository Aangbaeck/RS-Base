using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Newtonsoft.Json;
using RS_Base;
using RS_Base.Net.Model;
using RS_Base.Views;
using RS_StandardComponents;
using Serilog;

namespace RS_Base.Views
{
    public partial class MainV
    {
        public MainV()
        {
            MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;  //This makes the window no go underneath the bottom taskbar
            MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;

            InitializeComponent();
        }
    }
}