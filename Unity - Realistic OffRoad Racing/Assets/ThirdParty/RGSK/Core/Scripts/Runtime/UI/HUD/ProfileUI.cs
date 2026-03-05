using UnityEngine;

namespace RGSK
{
    public class ProfileUI : EntityUIComponent
    {
        public ProfileDefinitionUI profileDefinitionUI;

        public override void Bind(RGSKEntity e)
        {
            base.Bind(e);
            profileDefinitionUI?.UpdateUI(Entity?.ProfileDefiner?.definition);
        }

        public override void Update()
        {

        }

        public override void Refresh()
        {

        }

        public override void Destroy()
        {

        }
    }
}