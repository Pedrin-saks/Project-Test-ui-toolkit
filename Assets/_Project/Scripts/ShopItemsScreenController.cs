using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;

namespace Enaldinho.UI
{
    public class ShopItemsScreenController : UIScreenBase
    {
        [Header("Data")]
        [SerializeField] private ShopDatabaseSO database;
        [SerializeField] private ShopCategoryDataSO selectedCategory;
        [Header("Templates")]
        [SerializeField] private VisualTreeAsset buildingCardAsset;
        [Header("Layout")]
        [SerializeField] private float buildingCardSpacing = 18f;
        [SerializeField] private float buildingCardLeftMargin = 0f;

        private Label _screenTitle;
        private Label _softCurrencyValue;
        private Label _premiumCurrencyValue;
        private Button _backButton;
        private Button _closeButton;
        private Button _softCurrencyAddButton;
        private Button _premiumCurrencyAddButton;
        private VisualElement _buildingsRow;

        protected override void InitializeUIElements()
        {
            _screenTitle = QueryElement<Label>("ScreenTitle");
            _softCurrencyValue = QueryElement<Label>("SoftCurrencyValue");
            _premiumCurrencyValue = QueryElement<Label>("PremiumCurrencyValue");
            _backButton = QueryElement<Button>("BackButton");
            _closeButton = QueryElement<Button>("CloseButton");
            _softCurrencyAddButton = QueryElement<Button>("SoftCurrencyAddButton");
            _premiumCurrencyAddButton = QueryElement<Button>("PremiumCurrencyAddButton");
            _buildingsRow = QueryElement<VisualElement>("BuildingsRow");
        }

        protected override void RegisterCallbacks()
        {
            if (_backButton != null)
                _backButton.clicked += HandleBackPressed;

            if (_closeButton != null)
                _closeButton.clicked += HandleClosePressed;

            if (_softCurrencyAddButton != null)
                _softCurrencyAddButton.clicked += HandleSoftCurrencyAddPressed;

            if (_premiumCurrencyAddButton != null)
                _premiumCurrencyAddButton.clicked += HandlePremiumCurrencyAddPressed;
        }

        protected override void UnregisterCallbacks()
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

        protected override void OnScreenEnabled()
        {
            if (database == null)
            {
                Debug.LogError("[ShopItemsScreenController] ShopDatabaseSO is not assigned.");
                return;
            }

            if (buildingCardAsset == null)
            {
                Debug.LogError("[ShopItemsScreenController] Building card VisualTreeAsset is not assigned.");
                return;
            }

            if (_buildingsRow == null)
            {
                Debug.LogError("[ShopItemsScreenController] BuildingsRow was not found in ShopItemsDocument.");
                return;
            }

            BindHeader();
            BindHudValues();
            PopulateBuildings();
        }

        private void BindHeader()
        {
            if (_screenTitle == null)
                return;

            _screenTitle.text = selectedCategory != null
                ? selectedCategory.DisplayName.ToUpperInvariant()
                : "COMMUNITY";
        }

        private void BindHudValues()
        {
            if (_softCurrencyValue != null)
                _softCurrencyValue.text = database.SoftCurrencyDisplay;

            if (_premiumCurrencyValue != null)
                _premiumCurrencyValue.text = database.PremiumCurrencyDisplay;
        }

        private void PopulateBuildings()
        {
            _buildingsRow.Clear();

            List<ShopBuildingDataSO> buildings = selectedCategory != null
                ? selectedCategory.Buildings
                : null;

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
                    Debug.LogWarning($"[ShopItemsScreenController] Building '{building.DisplayName}' is missing IconSprite.");
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
                            "Mock purchase | item: {0} | price: {1}",
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
            Debug.Log("Mock back to categories");
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
