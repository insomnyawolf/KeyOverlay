namespace LowLevelInputHooks.DeviceSpecific.Windows
{
    public class WindowsMouseEvent : MouseEvent
    {
        internal WindowsMouseEvent(MouseMessage MouseMessage, MsllHookStruct extra, bool Global)
        {
            Position = extra.Position;

            switch (MouseMessage)
            {
                case MouseMessage.WM_MOUSEMOVE:
                case MouseMessage.WM_NCMOUSEMOVE:
                    InputAction = MouseInputAction.Move;
                    MouseButton = MouseButton.None;
                    break;
                case MouseMessage.WM_LBUTTONDOWN:
                case MouseMessage.WM_NCLBUTTONDOWN:
                case MouseMessage.WM_LBUTTONDBLCLK:
                case MouseMessage.WM_NCLBUTTONDBLCLK:
                    InputAction = MouseInputAction.Down;
                    MouseButton = MouseButton.Left;
                    break;
                case MouseMessage.WM_LBUTTONUP:
                case MouseMessage.WM_NCLBUTTONUP:
                    InputAction = MouseInputAction.Up;
                    MouseButton = MouseButton.Left;
                    break;
                case MouseMessage.WM_RBUTTONDOWN:
                case MouseMessage.WM_NCRBUTTONDOWN:
                case MouseMessage.WM_RBUTTONDBLCLK:
                case MouseMessage.WM_NCRBUTTONDBLCLK:
                    InputAction = MouseInputAction.Down;
                    MouseButton = MouseButton.Right;
                    break;
                case MouseMessage.WM_RBUTTONUP:
                case MouseMessage.WM_NCRBUTTONUP:
                    InputAction = MouseInputAction.Up;
                    MouseButton = MouseButton.Right;
                    break;
                case MouseMessage.WM_MBUTTONDOWN:
                case MouseMessage.WM_NCMBUTTONDOWN:
                case MouseMessage.WM_MBUTTONDBLCLK:
                case MouseMessage.WM_NCMBUTTONDBLCLK:
                    InputAction = MouseInputAction.Down;
                    MouseButton = MouseButton.Middle;
                    break;
                case MouseMessage.WM_MBUTTONUP:
                case MouseMessage.WM_NCMBUTTONUP:
                    InputAction = MouseInputAction.Up;
                    MouseButton = MouseButton.Middle;
                    break;
                case MouseMessage.WM_MOUSEWHEEL:
                    MouseButton = MouseButton.Scroll;
                    if (Global)
                    {
                        Console.WriteLine(extra.mouseData);
                        if (extra.mouseData > 0)
                            InputAction = MouseInputAction.Up;
                        else if (extra.mouseData < 0)
                            InputAction = MouseInputAction.Down;
                    }
                    else
                    {
                        if (extra.WheelDelta > 0)
                            InputAction = MouseInputAction.Up;
                        else if (extra.WheelDelta < 0)
                            InputAction = MouseInputAction.Down;
                    }
                    break;
                case MouseMessage.WM_MOUSEHWHEEL:
                    InputAction = MouseInputAction.None;
                    MouseButton = MouseButton.WheelB;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
