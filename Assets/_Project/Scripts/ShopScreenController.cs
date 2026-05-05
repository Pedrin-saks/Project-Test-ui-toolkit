using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;

namespace Enaldinho.UI
{
    public class ShopScreenController : UIScreenBase
    {
        private const string CategoriesTitle = "BUILDINGS";

        [Header("Data")]
        [SerializeField] private ShopDatabaseSO database;
        [Header("Documents")]
        [SerializeField] private VisualTreeAsset categoriesDocumentAsset;
        [SerializeField] private VisualTreeAsset itemsDocumentAsset;
        [Header("Templates")]
        [SerializeField] private VisualTreeAsset categoryCardAsset;
        [SerializeField] private VisualTreeAsset buildingCardAsset;
        [Header("Layout")]
        [SerializeField] private float buildingCardSpacing = 24f;
        [SerializeField] private float buildingCardLeftMargin = 0f;

        private Label _screenTitle;
        private Label _softCurrencyValue;
        private Label _premiumCurrencyValue;
        private Button _backButton;
        private Button _closeButton;
        private Button _softCurrencyAddButton;
        private Button _premiumCurrencyAddButton;
        private VisualElement _categoryGrid;
        private VisualElement _buildingsRow;

        private List<ShopCategoryDataSO> _categories;
        private ShopCategoryDataSO _selectedCategory;

        protected override void InitializeUIElements()
        {
        }

        protected override void UnregisterCallbacks()
        {
            UnbindCurrentCallbacks();
        }

        protected override void OnScreenEnabled()
        {
            if (database == null)
            {
                Debug.LogError("[ShopScreenController] ShopDatabaseSO is not assigned.");
                return;
            }

            if (categoriesDocumentAsset == null)
            {
                Debug.LogError("[ShopScreenController] Categories document VisualTreeAsset is not assigned.");
                return;
            }

            if (itemsDocumentAsset == null)
            {
                Debug.LogError("[ShopScreenController] Items document VisualTreeAsset is not assigned.");
                return;
            }

            if (categoryCardAsset == null)
            {
                Debug.LogError("[ShopScreenController] Category card VisualTreeAsset is not assigned.");
                return;
            }

            if (buildingCardAsset == null)
            {
                Debug.LogError("[ShopScreenController] Building card VisualTreeAsset is not assigned.");
                return;
            }

            ShowCategoriesDocument();
        }

        private void ShowCategoriesDocument()
        {
            _selectedCategory = null;

            RebuildRoot(categoriesDocumentAsset);

            _screenTitle = root.Q<Label>("ScreenTitle");
            _backButton = root.Q<Button>("BackButton");
            _closeButton = root.Q<Button>("CloseButton");
            _categoryGrid = root.Q<VisualElement>("CategoriesGrid");
            _softCurrencyValue = null;
            _premiumCurrencyValue = null;
            _softCurrencyAddButton = null;
            _premiumCurrencyAddButton = null;
            _buildingsRow = null;

            if (_screenTitle != null)
                _screenTitle.text = CategoriesTitle;

            if (_backButton != null)
                _backButton.style.display = DisplayStyle.None;

            if (_closeButton != null)
                _closeButton.clicked += HandleClosePressed;

            if (_categoryGrid == null)
            {
                Debug.LogError("[ShopScreenController] CategoriesGrid was not found in ShoppingCategoriesDocument.");
                return;
            }

            PopulateCategories();
        }

        private void ShowItemsDocument(ShopCategoryDataSO category)
        {
            _selectedCategory = category;

            RebuildRoot(itemsDocumentAsset);

            _screenTitle = root.Q<Label>("ScreenTitle");
            _backButton = root.Q<Button>("BackButton");
            _closeButton = root.Q<Button>("CloseButton");
            _softCurrencyValue = root.Q<Label>("SoftCurrencyValue");
            _premiumCurrencyValue = root.Q<Label>("PremiumCurrencyValue");
            _softCurrencyAddButton = root.Q<Button>("SoftCurrencyAddButton");
            _premiumCurrencyAddButton = root.Q<Button>("PremiumCurrencyAddButton");
            _buildingsRow = root.Q<VisualElement>("BuildingsRow");
            _categoryGrid = null;

            if (_screenTitle != null)
                _screenTitle.text = category.DisplayName.ToUpperInvariant();

            if (_backButton != null)
            {
                _backButton.style.display = DisplayStyle.Flex;
                _backButton.clicked += HandleBackPressed;
            }

            if (_closeButton != null)
                _closeButton.clicked += HandleClosePressed;

            if (_softCurrencyAddButton != null)
                _softCurrencyAddButton.clicked += HandleSoftCurrencyAddPressed;

            if (_premiumCurrencyAddButton != null)
                _premiumCurrencyAddButton.clicked += HandlePremiumCurrencyAddPressed;

            if (_softCurrencyValue != null)
                _softCurrencyValue.text = database.SoftCurrencyDisplay;

            if (_premiumCurrencyValue != null)
                _premiumCurrencyValue.text = database.PremiumCurrencyDisplay;

            if (_buildingsRow == null)
            {
                Debug.LogError("[ShopScreenController] BuildingsRow was not found in ShopItemsDocument.");
                return;
            }

            PopulateBuildings(category);
        }

        private void RebuildRoot(VisualTreeAsset documentAsset)
        {
            UnbindCurrentCallbacks();
            root.Clear();
            documentAsset.CloneTree(root);
        }

        private void UnbindCurrentCallbacks()
        {
            if (_backButton != null)
                _backButton.clicked -= HandleBackPressed;

            if (_closeButton != null)
                _closeButton.clicked -= HandleClosePressed;

            if (_softCurrencyAddButton != null)
                _softCurrencyAddButton.clicked -= HandleSoftCurrencyAddPressed;

            if (_premiumCurrencyAddButton != null)
                _premiumCurrencyAddButton.clicked -= HandlePremiumCurrencyAddPressed;
        }

        private void PopulateCategories()
        {
            _categoryGrid.Clear();
            _categories = database.Categories;

            if (_categories == null)
                return;

            for (int i = 0; i < _categories.Count; i++)
            {
                ShopCategoryDataSO category = _categories[i];
                _categoryGrid.Add(CreateCategoryCard(category));
            }
        }

        private VisualElement CreateCategoryCard(ShopCategoryDataSO category)
        {
            TemplateContainer cardTemplate = categoryCardAsset.CloneTree();
            VisualElement cardRoot = cardTemplate.Q<VisualElement>("CategoryCard") ?? cardTemplate;
            VisualElement cardIcon = cardRoot.Q<VisualElement>("CardIcon");
            Label title = cardRoot.Q<Label>("CardTitleLabel");

            cardTemplate.RegisterCallback<ClickEvent>(_ => ShowItemsDocument(category));

            if (title != null)
                title.text = category.DisplayName;

            if (cardIcon != null)
            {
                if (category.IconSprite != null)
                {
                    cardIcon.style.backgroundImage = new StyleBackground(category.IconSprite);
                }
                else
                {
                    Debug.LogWarning($"[ShopScreenController] Category '{category.DisplayName}' is missing IconSprite.");
                }
            }

            return cardTemplate;
        }

        private void PopulateBuildings(ShopCategoryDataSO category)
        {
            _buildingsRow.Clear();

            List<ShopBuildingDataSO> buildings = category != null ? category.Buildings : null;
            if (buildings == null)
                return;

            for (int i = 0; i < buildings.Count; i++)
            {
                ShopBuildingDataSO building = buildings[i];
                _buildingsRow.Add(CreateBuildingCard(building));
            }
        }

        private VisualElement CreateBuildingCard(ShopBuildingDataSO building)
        {
            TemplateContainer cardTemplate = buildingCardAsset.CloneTree();
            VisualElement cardRoot = cardTemplate.Q<VisualElement>("BuildingCard") ?? cardTemplate;
            VisualElement titleBar = cardRoot.Q<VisualElement>("CardTitleBar");
            Label title = cardRoot.Q<Label>("CardTitleLabel");
            VisualElement bonusRow = cardRoot.Q<VisualElement>("CardBonusRow");
            Label bonusText = cardRoot.Q<Label>("CardBonusText");
            VisualElement image = cardRoot.Q<VisualElement>("CardImage");
            Button purchaseButton = cardRoot.Q<Button>("PurchaseButton");
            Label purchaseLabel = cardRoot.Q<Label>("PurchaseButtonLabel");
            VisualElement lockedState = cardRoot.Q<VisualElement>("LockedState");

            cardTemplate.style.marginRight = buildingCardSpacing;
            cardTemplate.style.marginLeft = buildingCardLeftMargin;

            if (title != null)
                title.text = building.DisplayName;

            if (bonusRow != null)
                bonusRow.style.display = string.IsNullOrWhiteSpace(building.BonusText) ? DisplayStyle.None : DisplayStyle.Flex;

            if (bonusText != null)
                bonusText.text = building.BonusText;

            if (image != null)
            {
                if (building.IconSprite != null)
                {
                    image.style.backgroundImage = new StyleBackground(building.IconSprite);
                }
                else if (!string.IsNullOrWhiteSpace(building.ArtClass))
                {
                    image.AddToClassList(building.ArtClass);
                }
                else
                {
                    Debug.LogWarning($"[ShopScreenController] Building '{building.DisplayName}' is missing IconSprite.");
                }
            }

            if (titleBar != null && building.IsFeatured)
                titleBar.AddToClassList("shopping-building-card__title-bar--featured");

            if (title != null && building.IsFeatured)
                title.AddToClassList("shopping-building-card__title--featured");

            if (building.IsLocked)
            {
                cardRoot.AddToClassList("shopping-building-card--locked");

                if (image != null)
                    image.AddToClassList("shopping-building-card__image--locked");

                if (title != null)
                    title.AddToClassList("shopping-building-card__title--locked");

                if (purchaseButton != null)
                    purchaseButton.style.display = DisplayStyle.None;

                if (lockedState != null)
                    lockedState.style.display = DisplayStyle.Flex;
            }
            else
            {
                if (purchaseButton != null)
                {
                    purchaseButton.style.display = DisplayStyle.Flex;
                    purchaseButton.clicked += () =>
                    {
                        Debug.Log(string.Format(
                            CultureInfo.InvariantCulture,
                            "Mock purchase | category: {0} | item: {1} | price: {2}",
                            _selectedCategory != null ? _selectedCategory.DisplayName : "Unknown",
                            building.DisplayName,
                            building.Price));
                    };
                }

                if (purchaseLabel != null)
                    purchaseLabel.text = building.Price.ToString(CultureInfo.InvariantCulture);

                if (lockedState != null)
                    lockedState.style.display = DisplayStyle.None;
            }

            return cardTemplate;
        }

        private void HandleBackPressed()
        {
            ShowCategoriesDocument();
        }

        private void HandleClosePressed()
        {
            Debug.Log("Mock close shop");
        }

        private void HandleSoftCurrencyAddPressed()
        {
            Debug.Log("Mock add soft currency");
        }

        private void HandlePremiumCurrencyAddPressed()
        {
            Debug.Log("Mock add premium currency");
        }
    }
}
