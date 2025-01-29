using Anti_Recoil_Application.Core.Services;
using Anti_Recoil_Application.ViewModels;
using System.Runtime.InteropServices;
using System.Windows;

namespace Anti_Recoil_Application.Core.Handlers
{
    public class InputHandler
    {
        // Boolean flag to indicate whether the fire action is triggered
        public bool Fire;

        // Constants representing virtual key codes for specific keyboard and mouse buttons
        private const int VK_FBUTTON = 0x70; // The virtual key code for F1 key (0x70 is the code for F1, but could be adjusted for other function keys)

        private const int VK_LBUTTON = 0x01; // The virtual key code for the left mouse button (0x01 represents the left button click)

        private const int VK_RBUTTON = 0x02; // The virtual key code for the right mouse button (0x02 represents the right button click)

        private const int MOUSEEVENTF_MOVE = 0x0001; // Constant representing mouse movement event (0x0001 is used to indicate movement of the mouse)

        // A private boolean flag to track whether the F1 key has been pressed
        private bool _f1HasPressed;

        private readonly CoreService _core;

        /// <summary>
        /// Moves the mouse cursor relative to its current position by the specified fractional amounts in the horizontal (x) and vertical (y) directions.
        /// </summary>
        /// <param name="x">The fractional amount by which to move the cursor horizontally. Positive values move the cursor right, while negative values move it left.</param>
        /// <param name="y">The fractional amount by which to move the cursor vertically. Positive values move the cursor down, while negative values move it up.</param>
        float remainderFractionalX = 0, remainderFractionalY = 0;

        public InputHandler(CoreService core)
        {
            _core = core;
        }

        public void Update()
        {
            if (_core == null)
                return;

            if (GetKey(VK_FBUTTON) && !_f1HasPressed)
            {
                _f1HasPressed = true;
                _core.IsActive = !_core.IsActive;
            }
            else if (!GetKey(VK_FBUTTON) && _f1HasPressed)
                _f1HasPressed = false;

            var key = VK_FBUTTON;
            for (int i = 0; i < _core.Weapons.Length; i++)
            {
                key = VK_FBUTTON + i + 1; // Increment FKey by i + 1
                if (GetKey(key) && _core.SelectedWeapon != _core.Weapons[i])
                {
                    _core.SelectedWeapon = _core.Weapons[i];
                }
            }

            if (!_core.IsActive)
                return;

            Fire = GetKey(VK_RBUTTON) && GetKey(VK_LBUTTON);
        }

        public void MoveMouseRelativeFractional(float x, float y)
        {
            x = remainderFractionalX + x;
            y = remainderFractionalY + y;
            int xInt = (int)Math.Floor(x + 0.5f);
            int yInt = (int)Math.Floor(y + 0.5f);
            remainderFractionalX = x - xInt;
            remainderFractionalY = y - yInt;
            MoveMouse(xInt, yInt);
        }

        private void MoveMouse(int dx, int dy)
        {

            mouse_event(MOUSEEVENTF_MOVE, Convert.ToInt32(dx), Convert.ToInt32(dy), 1, UIntPtr.Zero);
        }
        private bool GetKey(int virtualKeyCode)
        {
            return (GetAsyncKeyState(virtualKeyCode) & 0x8000) != 0;
        }

        #region Setup 
        [DllImport("user32.dll")]
        private static extern bool GetGUIThreadInfo(uint idThread, ref GUITHREADINFO lpgui);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, UIntPtr dwExtraInfo);

        [DllImport("user32.dll")]
        private static extern int GetAsyncKeyState(int vKey);

        [StructLayout(LayoutKind.Sequential)]
        public struct GUITHREADINFO
        {
            public int cbSize;
            public int flags;
            public IntPtr hwndActive;
            public IntPtr hwndFocus;
            public IntPtr hwndCapture;
            public IntPtr hwndMenuOwner;
            public IntPtr hwndMoveSize;
            public IntPtr hwndCaret;
            public System.Drawing.Rectangle rcCaret;
        }

        #endregion

    }

}
