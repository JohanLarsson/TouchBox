namespace TouchBox
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;

    /// <summary>
    /// https://social.technet.microsoft.com/wiki/contents/articles/6460.simulating-touch-input-in-windows-8-using-touch-injection-api.aspx?PageIndex=2
    /// </summary>
    static class Touch2
    {
        static Touch2()
        {
            if (!InitializeTouchInjection())
            {
                throw new Win32Exception();
            }
        }

        private static PointerTouchInfo[] contacts = new PointerTouchInfo[1];

        public static void Tap(int x, int y)
        {
            Down(x, y);
            Up();
        }

        public static void Down(int x, int y)
        {
            contacts = new[]
            {
                new PointerTouchInfo
                {
                    PointerInfo =
                    {
                        pointerType = POINTER_INPUT_TYPE.PT_TOUCH,
                        pointerFlags = POINTER_FLAG.DOWN | POINTER_FLAG.INRANGE | POINTER_FLAG.INCONTACT,
                        ptPixelLocation = new TouchPoint(x, y),
                        pointerId = 0,
                    },
                    TouchFlags = TouchFlags.NONE,
                    Orientation = 90,
                    Pressure = 32000,
                    TouchMasks = TouchMask.CONTACTAREA | TouchMask.ORIENTATION | TouchMask.PRESSURE,
                    ContactArea = ContactArea.Create(new TouchPoint(x, y), 2),
                }
            };
            if (!InjectTouchInput(1, contacts))
            {
                throw new Win32Exception();
            }
        }

        public static void Drag(int x, int y)
        {
            //Setting the Pointer Flag to Drag
            contacts[0].PointerInfo.pointerFlags = POINTER_FLAG.UPDATE | POINTER_FLAG.INRANGE | POINTER_FLAG.INCONTACT;

            contacts[0].PointerInfo.ptPixelLocation.X = x;
            contacts[0].PointerInfo.ptPixelLocation.Y = y;

            if (!InjectTouchInput(1, contacts))
            {
                throw new Win32Exception();
            }
        }

        public static void Up()
        {
            contacts[0].Pressure = 0;
            contacts[0].PointerInfo.pointerFlags = POINTER_FLAG.UP;
            if (!InjectTouchInput(1, contacts))
            {
                throw new Win32Exception();
            }
        }

        [DllImport("User32.dll", SetLastError = true)]
        static extern bool InitializeTouchInjection(uint maxCount = 1, TouchFeedback feedbackMode = TouchFeedback.DEFAULT);

        [DllImport("User32.dll", SetLastError = true)]
        static extern bool InjectTouchInput(int count, [MarshalAs(UnmanagedType.LPArray), In] PointerTouchInfo[] contacts);

        public enum POINTER_FLAG
        {
            NONE = 0x00000000,
            NEW = 0x00000001,
            INRANGE = 0x00000002,
            INCONTACT = 0x00000004,
            FIRSTBUTTON = 0x00000010,
            SECONDBUTTON = 0x00000020,
            THIRDBUTTON = 0x00000040,
            OTHERBUTTON = 0x00000080,
            PRIMARY = 0x00000100,
            CONFIDENCE = 0x00000200,
            CANCELLED = 0x00000400,
            DOWN = 0x00010000,
            UPDATE = 0x00020000,
            UP = 0x00040000,
            WHEEL = 0x00080000,
            HWHEEL = 0x00100000
        }

        public enum TouchFeedback
        {
            DEFAULT = 0x1,
            INDIRECT = 0x2,
            NONE = 0x3
        }

        public struct ContactArea
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public static ContactArea Create(TouchPoint p, int r) => new ContactArea
            {
                left = p.X - r,
                right = p.X + r,
                top = p.Y - r,
                bottom = p.Y + r,
            };
        }

        public enum TouchFlags
        {
            NONE = 0x00000000
        }

        public enum TouchMask
        {
            NONE = 0x00000000,
            CONTACTAREA = 0x00000001,
            ORIENTATION = 0x00000002,
            PRESSURE = 0x00000004
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct TouchPoint
        {
            public TouchPoint(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }

            public int X;
            public int Y;
        }

        public enum POINTER_INPUT_TYPE
        {
            //PT_POINTER = 0x00000001,
            PT_TOUCH = 0x00000002,
            //PT_PEN = 0x00000003,
            //PT_MOUSE = 0x00000004
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct POINTER_INFO
        {
            public POINTER_INPUT_TYPE pointerType;
            public uint pointerId;
            public uint frameId;
            public POINTER_FLAG pointerFlags;
            public IntPtr sourceDevice;
            public IntPtr hwndTarget;
            public TouchPoint ptPixelLocation;
            public TouchPoint ptPixelLocationRaw;
            public TouchPoint ptHimetricLocation;
            public TouchPoint ptHimetricLocationRaw;
            public uint dwTime;
            public uint historyCount;
            public uint inputData;
            public uint dwKeyStates;
            public ulong PerformanceCount;
            public PointerButtonChangeType ButtonChangeType;
        }


        public enum PointerButtonChangeType
        {
            NONE,
            FIRSTBUTTON_DOWN,
            FIRSTBUTTON_UP,
            SECONDBUTTON_DOWN,
            SECONDBUTTON_UP,
            THIRDBUTTON_DOWN,
            THIRDBUTTON_UP,
            FOURTHBUTTON_DOWN,
            FOURTHBUTTON_UP,
            FIFTHBUTTON_DOWN,
            FIFTHBUTTON_UP
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PointerTouchInfo
        {
            public POINTER_INFO PointerInfo;
            public TouchFlags TouchFlags;
            public TouchMask TouchMasks;
            public ContactArea ContactArea;
            public ContactArea ContactAreaRaw;
            public uint Orientation;
            public uint Pressure;

            private static PointerTouchInfo Create(int x, int y, uint id)
            {
                var contact = new PointerTouchInfo
                {
                    PointerInfo =
                    {
                        pointerType = POINTER_INPUT_TYPE.PT_TOUCH,
                        pointerFlags = POINTER_FLAG.DOWN | POINTER_FLAG.INRANGE | POINTER_FLAG.INCONTACT,
                        ptPixelLocation = new TouchPoint(x, y),
                        pointerId = id,
                    },
                    TouchFlags = TouchFlags.NONE,
                    Orientation = 90,
                    Pressure = 32000,
                    TouchMasks = TouchMask.CONTACTAREA | TouchMask.ORIENTATION | TouchMask.PRESSURE,
                    ContactArea = ContactArea.Create(new TouchPoint(x, y), 2),
                };

                return contact;
            }

            public void Move(int deltaX, int deltaY)
            {
                this.PointerInfo.ptPixelLocation.X += deltaX;
                this.PointerInfo.ptPixelLocation.Y += deltaY;
                this.ContactArea.left += deltaX;
                this.ContactArea.right += deltaX;
                this.ContactArea.top += deltaY;
                this.ContactArea.bottom += deltaY;
            }
        }
    }
}