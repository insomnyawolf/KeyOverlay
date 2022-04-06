namespace LowLevelInputHooks
{
    public enum MouseMessage
    {
        WM_MOUSEMOVE = 0x0200,
        WM_LBUTTONDOWN = 0x0201,
        WM_LBUTTONUP = 0x0202,
        WM_LBUTTONDBLCLK = 0x0203,
        WM_RBUTTONDOWN = 0x0204,
        WM_RBUTTONUP = 0x0205,
        WM_RBUTTONDBLCLK = 0x0206,
        WM_MBUTTONDOWN = 0x0207,
        WM_MBUTTONUP = 0x0208,
        WM_MBUTTONDBLCLK = 0x0209,

        WM_MOUSEWHEEL = 0x020A,
        WM_MOUSEHWHEEL = 0x020E,

        WM_NCMOUSEMOVE = 0x00A0,
        WM_NCLBUTTONDOWN = 0x00A1,
        WM_NCLBUTTONUP = 0x00A2,
        WM_NCLBUTTONDBLCLK = 0x00A3,
        WM_NCRBUTTONDOWN = 0x00A4,
        WM_NCRBUTTONUP = 0x00A5,
        WM_NCRBUTTONDBLCLK = 0x00A6,
        WM_NCMBUTTONDOWN = 0x00A7,
        WM_NCMBUTTONUP = 0x00A8,
        WM_NCMBUTTONDBLCLK = 0x00A9
    }

    /// <summary>
    ///  Specifies the appearance of a button.
    /// </summary>
    [Flags]
    public enum ButtonState
    {
        Checked = (int)DFCS.CHECKED,
        Flat = (int)DFCS.FLAT,
        Inactive = (int)DFCS.INACTIVE,
        Normal = 0,
        Pushed = (int)DFCS.PUSHED,
        All = Flat | Checked | Pushed | Inactive,
    }

    [Flags]
    public enum DFCS : uint
    {
        CAPTIONCLOSE = 0x0000,
        CAPTIONMIN = 0x0001,
        CAPTIONMAX = 0x0002,
        CAPTIONRESTORE = 0x0003,
        CAPTIONHELP = 0x0004,
        MENUARROW = 0x0000,
        MENUCHECK = 0x0001,
        MENUBULLET = 0x0002,
        MENUARROWRIGHT = 0x0004,
        SCROLLUP = 0x0000,
        SCROLLDOWN = 0x0001,
        SCROLLLEFT = 0x0002,
        SCROLLRIGHT = 0x0003,
        SCROLLCOMBOBOX = 0x0005,
        SCROLLSIZEGRIP = 0x0008,
        SCROLLSIZEGRIPRIGHT = 0x0010,
        BUTTONCHECK = 0x0000,
        BUTTONRADIOIMAGE = 0x0001,
        BUTTONRADIOMASK = 0x0002,
        BUTTONRADIO = 0x0004,
        BUTTON3STATE = 0x0008,
        BUTTONPUSH = 0x0010,
        INACTIVE = 0x0100,
        PUSHED = 0x0200,
        CHECKED = 0x0400,
        TRANSPARENT = 0x0800,
        HOT = 0x1000,
        ADJUSTRECT = 0x2000,
        FLAT = 0x4000,
        MONO = 0x8000
    }
}
