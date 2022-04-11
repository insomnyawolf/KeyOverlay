using Veldrid;

namespace KeyOverlay2.Shapes
{
    internal abstract class VeldridViewportItem : IDisposable
    {
        internal Pipeline Pipeline;
        internal readonly BaseWindow Window;

        internal abstract void Draw();

        internal VeldridViewportItem(BaseWindow Window)
        {
            this.Window = Window;
        }

        // Destructor
        bool IsFinalized = false;

        ~VeldridViewportItem()
        {
            Dispose();
        }

        public virtual void Dispose()
        {
            if (!IsFinalized)
            {
                Pipeline.Dispose();
                IsFinalized = true;
                GC.SuppressFinalize(this);
            }
        }
    }
}
