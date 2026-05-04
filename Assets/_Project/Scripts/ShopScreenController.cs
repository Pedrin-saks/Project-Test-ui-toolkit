using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Enaldinho.UI
{
    public class ShopScreenController : UIScreenBase
    {
        [Header("Data")]
        [SerializeField] private ShopDatabaseSO database;
        [Header("Templates")]
        [SerializeField] private VisualTreeAsset categoryCardAsset;

        private Label _screenTitle;
        private Button _backButton;
        private Button _closeButton;
        private VisualElement _categoryGrid;

        private List<ShopCategoryDataSO> _categories;

        protected override void InitializeUIElements()
        {
            _screenTitle = QueryElement<Label>("ScreenTitle");
            _backButton = QueryElement<Button>("BackButton");
            _closeButton = QueryElement<Button>("CloseButton");
            _categoryGrid = QueryElement<VisualElement>("CategoriesGrid");
        }

        protected override void RegisterCallbacks()
        {
            if (_backButton != null)
                _backButton.clicked += HandleBackPressed;

            if (_closeButton != null)
                _closeButton.clicked += HandleClosePressed;
        }

        protected override void UnregisterCallbacks()
        {
            if (_backButton != null)
                _backButton.clicked -= HandleBackPressed;

            if (_closeButton != null)
                _closeButton.clicked -= HandleClosePressed;
        }

        protected override void OnScreenEnabled()
        {
            if (database == null)
            {
                Debug.LogError("[ShopScreenController] ShopDatabaseSO não está atribuído.");
                return;
            }

            if (categoryCardAsset == null)
            {
                Debug.LogError("[ShopScreenController] O card VisualTreeAsset não está atribuído..");
                return;
            }

            if (_categoryGrid == null)
            {
                Debug.LogError("[ShopScreenController] O CategoriesGrid não foi encontrado em ShoppingCategoriesDocument.");
                return;
            }

            PopulateCategories();
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

            cardTemplate.RegisterCallback<ClickEvent>(_ => HandleCategoryPressed(category));

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

        private void HandleCategoryPressed(ShopCategoryDataSO category)
        {
            if (_screenTitle != null)
                _screenTitle.text = category.DisplayName.ToUpperInvariant();

            Debug.Log($"Mock open category | {category.DisplayName}");
        }

        private void HandleBackPressed()
        {
            if (_screenTitle != null)
                _screenTitle.text = "BUILDINGS";
        }

        private void HandleClosePressed()
        {
            Debug.Log("Mock close shop");
        }
    }
}
