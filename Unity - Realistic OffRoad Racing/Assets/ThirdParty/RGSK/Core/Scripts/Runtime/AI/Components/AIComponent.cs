namespace RGSK
{
    public abstract class AIComponent
    {
        public AIController Controller { get; private set; }

        public virtual void Initialize(AIController ai)
        {
            Controller = ai;
        }

        public abstract void Update();
        public abstract void Reset();
    }
}