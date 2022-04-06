using Veldrid;

namespace KeyOverlay2.Shapes
{
    internal abstract class VeldridViewportItem
    {
        internal Pipeline Pipeline;
        internal readonly BaseWindow Window;

        internal abstract void Draw();

        internal VeldridViewportItem(BaseWindow Window)
        {
            this.Window = Window;
        }
    }
}
