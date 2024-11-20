using Assets.Scripts.Define;

namespace Assets.Scripts.Battle
{
    public class WoodenData : Entity
    {
        public override void F_InitData(int cfgId)
        {
            m_displayId = cfgId;
            m_enityType = EM_EntityType.Wooden;
            base.F_InitData(cfgId);
        }
    }
}
