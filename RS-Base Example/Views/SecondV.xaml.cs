using System.Windows;

namespace RS_Base.Views
{
    /// <summary>
    /// Description for TheSecondWindowV.
    /// </summary>
    public partial class SecondV : Window
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
            this.Close();
        }
        
        

        
    }
}