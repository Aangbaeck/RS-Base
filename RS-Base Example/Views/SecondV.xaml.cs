using System.Threading;
using System.Windows;

namespace RS_Base.Views
{
    /// <summary>
    /// Description for TheSecondWindowV.
    /// </summary>
    public partial class SecondV 
    {
        /// <summary>
        /// Initializes a new instance of the TheSecondWindowV class.
        /// </summary>
        public SecondV()
        {
            InitializeComponent();
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();  //Closes parent window.
        }
    }
}