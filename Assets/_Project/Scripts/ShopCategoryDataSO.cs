using System.Collections.Generic;
using UnityEngine;

namespace Enaldinho.UI
{
    [CreateAssetMenu(menuName = "Shop/Category Data", fileName = "ShopCategory_")]
    public class ShopCategoryDataSO : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private Sprite iconSprite;
        [SerializeField] private string artClass;
        [SerializeField] private List<ShopBuildingDataSO> buildings = new();

        public string Id => id;
        public string DisplayName => displayName;
        public Sprite IconSprite => iconSprite;
        public string ArtClass => artClass;
        public List<ShopBuildingDataSO> Buildings => buildings;
    }
}
