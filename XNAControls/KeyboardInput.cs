using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace XNAControls
{
    /// <remarks>
    /// Originally created by Promit: 
    /// http://www.gamedev.net/topic/457783-xna-getting-text-from-keyboard/page__p__4040190#entry4040190
    /// I've moved it to my XNA library, commented functions and done a bit of cleanup.
    /// </remarks>
    /// <summary>
    /// Grabs keyboard information, allowing XNA to work with keyboard information through events.
    /// </summary>
    public static class KeyboardInput
    {
        /// <summary>	
        /// Occurs when a character is being entered.
        /// </summary>		
        public static event CharEnteredHandler CharacterEntered;
        /// <summary>
        /// Occurs when a key is pressed down. 
        /// If a key is held down, this will fire multiple times due to keyboard repeat.		
        /// </summary>		
        public static event KeyEventHandler KeyDown;
        /// <summary>
        /// Occurs when a key is released.		
        /// </summary>
        public static event KeyEventHandler KeyUp;
        private delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
        
        private static IntPtr prevWndProc;
        private static WndProc hookProcDelegate;
        private static IntPtr hIMC;

        //various Win32 constants that we need		
        private const int GWL_WNDPROC = -4;
        private const int WM_KEYDOWN = 0x100;
        private const int WM_KEYUP = 0x101;
        private const int WM_CHAR = 0x102;
        private const int WM_IME_SETCONTEXT = 0x0281;
        private const int WM_INPUTLANGCHANGE = 0x51;
        private const int WM_GETDLGCODE = 0x87;
        private const int WM_IME_COMPOSITION = 0x10f;
        private const int DLGC_WANTALLKEYS = 4;
        
        //Win32 functions that we're using	
        [DllImport("Imm32.dll")]
        private static extern IntPtr ImmGetContext(IntPtr hWnd);
        [DllImport("Imm32.dll")]
        private static extern IntPtr ImmAssociateContext(IntPtr hWnd, IntPtr hIMC);
        [DllImport("user32.dll")]
        private static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private static bool initialized;
        private static GameWindow window;

        /// <summary>
        /// Initializes the <see cref="KeyboardInput"/> type using the given <see cref="GameWindow"/>.
        /// </summary>
        /// <param name="window">The XNA <see cref="GameWindow"/> to which text input should be linked.</param>	
        public static void Initialize(GameWindow window)
        {
            if (initialized)
                if (KeyboardInput.window != window)
                    throw new InvalidOperationException("TextInput.Initialize can only be called once!");
                else
                    return;
            else
                KeyboardInput.window = window;

            hookProcDelegate = new WndProc(HookProc);
            prevWndProc = (IntPtr)SetWindowLong(window.Handle, GWL_WNDPROC,
                (int)Marshal.GetFunctionPointerForDelegate(hookProcDelegate));
            hIMC = ImmGetContext(window.Handle);
            initialized = true;
        }
        /// <summary>
        /// Initializes the <see cref="KeyboardInput"/> type using the given <see cref="Game"/>.
        /// </summary>
        /// <param name="game">The XNA <see cref="Game"/> to which text input should be linked.</param>
        public static void Initialize(Game game)
        {
            Initialize(game.Window);
        }
        /// <summary>
        /// Gets a value indicating whether either of the initialization methods have already been called.
        /// </summary>
        public static bool IsInitialized
        {
            get { return initialized; }
        }
        private static IntPtr HookProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            IntPtr returnCode = CallWindowProc(prevWndProc, hWnd, msg, wParam, lParam);
            switch (msg)
            {
                case WM_GETDLGCODE:
                    returnCode = (IntPtr)(returnCode.ToInt32() | DLGC_WANTALLKEYS);
                    break;
                case WM_KEYDOWN:
                    if ((int)wParam == 16) shiftDown = true;
                    if ((int)wParam == 17) controlDown = true;

                    if (KeyDown != null)
                        KeyDown(null, new KeyEventArgs((Keys)wParam, shiftDown, controlDown));
                    break;
                case WM_KEYUP:
                    if ((int)wParam == 16) shiftDown = false;
                    if ((int)wParam == 17) controlDown = false;

                    if (KeyUp != null)
                        KeyUp(null, new KeyEventArgs((Keys)wParam, shiftDown, controlDown));
                    break;
                case WM_CHAR:
                    if (CharacterEntered != null)
                        CharacterEntered(null, new CharacterEventArgs((char)wParam, lParam.ToInt32()));
                    break;
                case WM_IME_SETCONTEXT:
                    if (wParam.ToInt32() == 1)
                        ImmAssociateContext(hWnd, hIMC);
                    break;
                case WM_INPUTLANGCHANGE:
                    ImmAssociateContext(hWnd, hIMC);
                    returnCode = (IntPtr)1;
                    break;
            }
            return returnCode;
        }

        private static bool controlDown = false;
        private static bool shiftDown = false;
    }
}
