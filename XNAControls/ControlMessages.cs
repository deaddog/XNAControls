using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNAControls
{
    public enum ControlMessages
    {
        KEYBOARD_CHARACTER = 0x00000001,
        KEYBOARD_KEYDOWN = 0x00000002,
        KEYBOARD_KEYUP = 0x00000003,
        MOUSE_MOVE = 0x00000004,
        MOUSE_DOWN = 0x00000005,
        MOUSE_UP = 0x00000006,
        MOUSE_CLICK = 0x00000007,
        MOUSE_WHEEL = 0x00000008,
        CONTROL_GOTFOCUS = 0x00000009,
        CONTROL_LOSTFOCUS = 0x0000000a,
        MOUSE_ENTER = 0x0000000b,
        MOUSE_LEAVE = 0x0000000c,
        CONTROL_SIZECHANGED = 0x0000000d,
        CONTROL_LOCATIONCHANGED = 0x0000000e
    }
}
