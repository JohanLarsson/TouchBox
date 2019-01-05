namespace TouchBox
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Wrapper for https://docs.microsoft.com/en-us/windows/desktop/api/_input_touchinjection/
    /// </summary>
    public static class Touch
    {
        public static void Tap(int x, int y)
        {
            if (!InitializeTouchInjection(1, TouchMode.TOUCH_FEEDBACK_DEFAULT))
            {
                throw new InvalidOperationException($"Initialize touch failed: {Marshal.GetLastWin32Error()}");
            }

            var contacts = new POINTER_TOUCH_INFO
            {
                pointerInfo =
                {
                    pointerType = POINTER_INPUT_TYPE.PT_TOUCH,
                    pointerFlags = POINTER_FLAGS.POINTER_FLAG_DOWN | POINTER_FLAGS.POINTER_FLAG_INRANGE | POINTER_FLAGS.POINTER_FLAG_INCONTACT,
                    ptPixelLocation = new POINT{x =x, y=y},
                    pointerId = 1,
                },
                touchFlags = TOUCH_FLAGS.TOUCH_FLAGS_NONE,
                touchMasks = TOUCH_MASK.TOUCH_MASK_CONTACTAREA | TOUCH_MASK.TOUCH_MASK_ORIENTATION | TOUCH_MASK.TOUCH_MASK_PRESSURE,
                orientation = 90,
                pressure = 32000,
                rcContact = new Rect { left = x - 2, right = x + 2, top = y - 2, bottom = y + 2 },
            };

            if (!InjectTouchInput(1, ref contacts))
            {
                throw new InvalidOperationException($"Touch down failed: {Marshal.GetLastWin32Error()}");
            }

            contacts.pressure = 0;
            contacts.pointerInfo.pointerFlags = POINTER_FLAGS.POINTER_FLAG_UP;
            if (!InjectTouchInput(1, ref contacts))
            {
                throw new InvalidOperationException($"Touch up failed: {Marshal.GetLastWin32Error()}");
            }
        }

        [DllImport("User32.dll")]
        static extern bool InitializeTouchInjection(uint maxCount, TouchMode dwMode);

        [DllImport("User32.dll")]
        static extern bool InjectTouchInput(int count, ref POINTER_TOUCH_INFO contacts);

        public enum TouchMode
        {
            /// <summary>
            /// Specifies default touch visualizations.
            /// </summary>
            TOUCH_FEEDBACK_DEFAULT = 0x1,

            /// <summary>
            /// Specifies indirect touch visualizations.
            /// </summary>
            TOUCH_FEEDBACK_INDIRECT = 0x2,

            /// <summary>
            /// Specifies no touch visualizations.
            /// </summary>
            TOUCH_FEEDBACK_NONE = 0x3,
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct Rect
        {
            [FieldOffset(0)]
            public int left;
            [FieldOffset(4)]
            public int top;
            [FieldOffset(8)]
            public int right;
            [FieldOffset(12)]
            public int bottom;
        }

        public enum TOUCH_FLAGS { TOUCH_FLAGS_NONE = 0x00000000/*Indicates that no flags are set.*/ }

        public enum TOUCH_MASK
        {
            /// <summary>
            /// Default - none of the optional fields are valid
            /// </summary>
            TOUCH_MASK_NONE = 0x00000000,

            /// <summary>
            /// The rcContact field is valid
            /// </summary>
            TOUCH_MASK_CONTACTAREA = 0x00000001,

            /// <summary>
            /// The orientation field is valid
            /// </summary>
            TOUCH_MASK_ORIENTATION = 0x00000002,

            /// <summary>
            /// The pressure field is valid
            /// </summary>
            TOUCH_MASK_PRESSURE = 0x00000004

        }

        public enum POINTER_FLAGS
        {
            POINTER_FLAG_NONE = 0x00000000,
            POINTER_FLAG_NEW = 0x00000001,
            POINTER_FLAG_INRANGE = 0x00000002,
            POINTER_FLAG_INCONTACT = 0x00000004,
            POINTER_FLAG_FIRSTBUTTON = 0x00000010,
            POINTER_FLAG_SECONDBUTTON = 0x00000020,
            POINTER_FLAG_THIRDBUTTON = 0x00000040,
            POINTER_FLAG_OTHERBUTTON = 0x00000080,
            POINTER_FLAG_PRIMARY = 0x00000100,
            POINTER_FLAG_CONFIDENCE = 0x00000200,
            POINTER_FLAG_CANCELLED = 0x00000400,
            POINTER_FLAG_DOWN = 0x00010000,
            POINTER_FLAG_UPDATE = 0x00020000,
            POINTER_FLAG_UP = 0x00040000,
            POINTER_FLAG_WHEEL = 0x00080000,
            POINTER_FLAG_HWHEEL = 0x00100000
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }

        public enum POINTER_INPUT_TYPE
        {
            PT_POINTER = 0x00000001,
            PT_TOUCH = 0x00000002,
            PT_PEN = 0x00000003,
            PT_MOUSE = 0x00000004
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct POINTER_INFO
        {
            public POINTER_INPUT_TYPE pointerType;
            public uint pointerId;
            public uint frameId;
            public POINTER_FLAGS pointerFlags;

            public IntPtr sourceDevice;
            public IntPtr hwndTarget;

            public POINT ptPixelLocation;
            public POINT ptHimetricLocation;

            public POINT ptPixelLocationRaw;
            public POINT ptHimetricLocationRaw;

            public uint dwTime;
            public uint historyCount;

            public uint inputData;
            public uint dwKeyStates;
            public ulong PerformanceCount;
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct POINTER_TOUCH_INFO
        {
            /// <summary>
            /// Contains basic pointer information common to all pointer types.
            /// </summary>
            public POINTER_INFO pointerInfo;

            /// <summary>
            /// Lists the touch flags.
            /// </summary>
            public TOUCH_FLAGS touchFlags;

            public TOUCH_MASK touchMasks;

            /// <summary>
            /// Pointer contact area in pixel screen coordinates. 
            /// By default, if the device does not report a contact area,
            /// this field defaults to a 0-by-0 rectangle centered around the pointer location.
            /// </summary>
            public Rect rcContact;

            public Rect rcContactRaw;

            /*
             * A pointer orientation, with a value between 0 and 359, where 0 indicates a touch pointer 
             * aligned with the x-axis and pointing from left to right; increasing values indicate degrees
             * of rotation in the clockwise direction.
             * This field defaults to 0 if the device does not report orientation.
             */
            public uint orientation;

            /*
             * Pointer pressure normalized in a range of 0 to 256.
             */
            public uint pressure;
        }
    }
}