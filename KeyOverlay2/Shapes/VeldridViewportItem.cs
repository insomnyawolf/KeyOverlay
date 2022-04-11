using Veldrid;

namespace KeyOverlay2.Shapes
{
    internal abstract class VeldridViewportItem : IDisposable
    {
        protected Pipeline Pipeline;
        protected readonly BaseWindow Window;        

        internal VeldridViewportItem(BaseWindow Window)
        {
            this.Window = Window;
        }

        public abstract void Draw();

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
