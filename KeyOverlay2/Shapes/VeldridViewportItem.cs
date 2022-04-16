using Veldrid;

namespace InEngine.Shapes
{
    public abstract class VeldridViewportItem : IDisposable
    {
        protected Pipeline Pipeline;
        protected readonly BaseWindow Window;        

        public VeldridViewportItem(BaseWindow Window)
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
