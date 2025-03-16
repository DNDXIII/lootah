using Gameplay.Items.Instances;
using UnityEngine;

namespace Gameplay.Items.Blueprints
{
    public abstract class BaseItemBlueprint : ScriptableObject
    {
        public int id;
        public bool stackable;
        public Sprite sprite;

        public abstract BaseItem GenerateItem();
    }
}