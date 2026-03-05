namespace RGSK
{
    public abstract class VehicleComponent
    {
        public VehicleController Vehicle { get; private set; }

        public virtual void Initialize(VehicleController vc)
        {
            Vehicle = vc;
        }

        public abstract void Update();
    }
}