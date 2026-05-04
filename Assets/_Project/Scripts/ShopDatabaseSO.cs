using System.Collections.Generic;
using UnityEngine;

namespace Enaldinho.UI
{
    [CreateAssetMenu(menuName = "Shop/Shop Database", fileName = "ShopDatabase")]
    public class ShopDatabaseSO : ScriptableObject
    {
        [SerializeField] private string softCurrencyDisplay = "120k";
        [SerializeField] private string premiumCurrencyDisplay = "120k";
        [SerializeField] private List<ShopCategoryDataSO> categories = new();

        public string SoftCurrencyDisplay => softCurrencyDisplay;
        public string PremiumCurrencyDisplay => premiumCurrencyDisplay;
        public List<ShopCategoryDataSO> Categories => categories;
    }
}
