using Gameplay.Items.Blueprints;

namespace Gameplay.Items.Instances
{
    [System.Serializable]
    public abstract class BaseItem
    {
        public int id;
        public bool stackable;

        protected BaseItem(BaseItemBlueprint baseItemBlueprint)
        {
            id = baseItemBlueprint.id;
            stackable = baseItemBlueprint.stackable;
        }

        public abstract string GetDescription();
    }
}