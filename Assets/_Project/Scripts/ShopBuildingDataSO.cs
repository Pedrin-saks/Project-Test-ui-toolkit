using UnityEngine;

namespace Enaldinho.UI
{
    [CreateAssetMenu(menuName = "Shop/Building Data", fileName = "ShopBuilding_")]
    public class ShopBuildingDataSO : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private int price;
        [SerializeField] private string bonusText;
        [SerializeField] private bool isLocked;
        [SerializeField] private string artClass;

        public string Id => id;
        public string DisplayName => displayName;
        public int Price => price;
        public string BonusText => bonusText;
        public bool IsLocked => isLocked;
        public string ArtClass => artClass;
    }
}
