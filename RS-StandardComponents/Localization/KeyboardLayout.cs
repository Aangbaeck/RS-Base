using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Input;

namespace RS_StandardComponents
{

    public delegate void KeyboardLayoutChanged(CultureInfo oldCultureInfo, CultureInfo newCultureInfo);
    public class KeyboardLayout : IDisposable
    {
        private readonly Timer _timer;
        private CultureInfo _currentLayout = CultureInfo.InvariantCulture;

        public static KeyboardLayout Instance { get { return Nested.instance; } }

        private class Nested
        {
            static Nested() { }

            internal static readonly KeyboardLayout instance = new KeyboardLayout();
        }
        private KeyboardLayout()
        {
            _timer = new Timer(new TimerCallback(CheckKeyboardLayout), null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
        }

        public KeyboardLayoutChanged KeyboardLayoutChanged;


        private void CheckKeyboardLayout(object sender)
        {
            var layout = GetCurrentKeyboardLayout();
            if (_currentLayout.Name != layout.Name && KeyboardLayoutChanged != null)
            {
                KeyboardLayoutChanged(_currentLayout, layout);
                _currentLayout = layout;
            }

        }

        public void Dispose()
        {
            _timer.Dispose();
            GC.SuppressFinalize(this);
        }

        ~KeyboardLayout()
        {
            _timer.Dispose();
        }


        [DllImport("user32.dll")] static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")] static extern uint GetWindowThreadProcessId(IntPtr hwnd, IntPtr proccess);
        [DllImport("user32.dll")] static extern IntPtr GetKeyboardLayout(uint thread);
        public static CultureInfo GetCurrentKeyboardLayout()
        {
            try
            {
                IntPtr foregroundWindow = GetForegroundWindow();
                uint foregroundProcess = GetWindowThreadProcessId(foregroundWindow, IntPtr.Zero);
                int keyboardLayout = GetKeyboardLayout(foregroundProcess).ToInt32() & 0xFFFF;
                return new CultureInfo(keyboardLayout);
            }
            catch (Exception)
            {
                return new CultureInfo(1033); // Assume English if something went wrong.
            }
        }
    }
}
