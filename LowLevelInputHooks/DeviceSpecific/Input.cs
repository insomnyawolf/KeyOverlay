namespace LowLevelInputHooks.DeviceSpecific
{
    public enum InputOrigin
    {
        Mouse,
        Keyboard,
    }

    public enum KeyboardInputAction
    {
        Down,
        Up,
#warning only works on non global inputs
        Repeat,
    }

    public enum MouseInputAction
    {
        Down,
        Up,
        Move,
        None
    }
}
