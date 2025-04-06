using UnityEngine;

namespace MotionSensorItem
{
    [CreateAssetMenu(fileName = "NewItem", menuName = "Other/Item")]
    public class ItemOverride : Item
    {
        public bool disabled;

        [Space]
        public string itemAssetName;

        public string itemName = "N/A";

        public string description;

        public enum itemType2
        {
            drone,
            orb,
            cart,
            item_upgrade,
            player_upgrade,
            power_crystal,
            grenade,
            melee,
            healthPack,
            gun,
            tracker,
            mine,
            pocket_cart,
            sensor
        }
        [Space]
        public new itemType2 itemType;

        public SemiFunc.emojiIcon emojiIcon;

        public SemiFunc.itemVolume itemVolume;

        public SemiFunc.itemSecretShopType itemSecretShopType;

        [Space]
        public ColorPresets colorPreset;

        public GameObject prefab;

        public Value value;

        [Space]
        public int maxAmount = 1;

        public int maxAmountInShop = 1;

        [Space]
        public bool maxPurchase;

        public int maxPurchaseAmount = 1;

        [Space]
        public Quaternion spawnRotationOffset;

        public bool physicalItem = true;

        private void OnValidate()
        {
            if (!SemiFunc.OnValidateCheck())
            {
                itemAssetName = base.name;
                prefab = Resources.Load<GameObject>("Items/" + itemAssetName);
            }
        }
    }
}
