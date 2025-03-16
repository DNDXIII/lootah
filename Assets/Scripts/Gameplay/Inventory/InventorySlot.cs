using Gameplay.Items.Instances;

namespace Gameplay.Inventory
{
    [System.Serializable]
    public class InventorySlot
    {
        public int id;
        public BaseItem BaseItem;
        public int amount;

        public InventorySlot()
        {
            id = -1;
            BaseItem = null;
            amount = 0;
        }

        public InventorySlot(BaseItem baseItem, int amount)
        {
            if (baseItem == null)
            {
                id = -1;
                this.BaseItem = null;
                this.amount = 0;
                return;
            }

            id = baseItem.id;
            this.BaseItem = baseItem;
            this.amount = amount;
        }

        public void UpdateSlot(BaseItem baseItem, int _amount)
        {
            if (baseItem == null)
            {
                id = -1;
                BaseItem = null;
                amount = 0;
                return;
            }

            id = baseItem.id;
            BaseItem = baseItem;
            amount = _amount;
        }

        public void UpdateSlot(InventorySlot slot)
        {
            id = slot.id;
            BaseItem = slot.BaseItem;
            amount = slot.amount;
        }

        public void AddAmount(int value)
        {
            amount += value;
        }
    }
}