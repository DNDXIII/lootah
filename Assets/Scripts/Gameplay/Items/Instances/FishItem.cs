using System;
using Gameplay.Items.Blueprints;

namespace Gameplay.Items.Instances
{
    [Serializable]
    public class FishItem : BaseItem
    {
        public FishItem(BaseItemBlueprint baseItemBlueprint) : base(baseItemBlueprint)
        {
        }

        public override string GetDescription()
        {
            return "A fish";
        }
    }
}