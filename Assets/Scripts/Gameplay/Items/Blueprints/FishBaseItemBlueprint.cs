using Gameplay.Items.Instances;
using UnityEngine;

namespace Gameplay.Items.Blueprints
{
    [CreateAssetMenu(fileName = "Fish", menuName = "Items/Fish")]
    public class FishBaseItemBlueprint : BaseItemBlueprint
    {
        public override BaseItem GenerateItem()
        {
            return new FishItem(this);
        }
    }
}