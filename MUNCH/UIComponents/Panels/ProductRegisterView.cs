using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MunchProject
{
    public enum ClassifiedKey
    {
        NONE,
        CATEGORY,
        MANUFACTURER,
        PRODUCT_NAME,
    }
    public class ProductRegisterView : Panel
    {
        public RawImage productImage;

        private const string SELECT_CATEGORY_TEXT = "카테고리 선택";
        private const string SELECT_MANUFACTURER_TEXT = "제조사 선택";
        private const string SELECT_PRODUCTNAME_TEXT = "상품명";

        private string m_selectedCategory = string.Empty;
        private string m_selectedManufacturer = string.Empty;
        private string m_selectedName = string.Empty;

        public TextMeshProUGUI categoryText;
        public TextMeshProUGUI manufacturerText;
        public TextMeshProUGUI productNameText;

        public Image manufacturerSelectIcon;
        public Image productNameSelectIcon;

        private Color DISABLED_TEXT_COLOR = new Color(0.7f, 0.7f, 0.7f, 1f);
        private Color DISABLED_SELECTOR_ICON = new Color(1f, 1f, 1f, 0.4f);

        public TMP_InputField priceInputField;
        public TMP_InputField descriptionInputField;

        protected override void OnStart()
        {
            base.OnStart();
            SetUserProductImage();
        }

        private void SetUserProductImage()
        {
            productImage.texture = MunchDatabase.instance.GetVgaTexture();
        }

        public void OnClickSelecCategoryButton()
        {
            List<string> keys = new List<string>(MunchDatabase.instance.productDictionary.products.Keys);
            SelectPopup selectPopup = PanelManager.Instance.AddPanel<SelectPopup>(PanelTag.SelectPopup, keys);
            selectPopup.SetSelectedTextAction(OnSelectedCategory);
        }

        private void OnSelectedCategory(string text)
        {
            if (m_selectedCategory != text)
            {
                SetDefaultManufacturerSelector();
                SetDisableProductNameSelector();
            }
            m_selectedCategory = text;
            categoryText.text = m_selectedCategory;
        }

        public void OnClickSelectManufacturerButton()
        {
            if (string.IsNullOrEmpty(m_selectedCategory))
            {
                return;
            }
            List<string> keys = new List<string>(MunchDatabase.instance.productDictionary.products[m_selectedCategory].Keys);

            SelectPopup selectPopup = PanelManager.Instance.AddPanel<SelectPopup>(PanelTag.SelectPopup, keys);
            selectPopup.SetSelectedTextAction(OnSelectedManufacturer);
        }

        private void OnSelectedManufacturer(string text)
        {
            if (m_selectedManufacturer != text)
            {
                SetDefaultProductNameSelector();
            }

            m_selectedManufacturer = text;
            manufacturerText.text = m_selectedManufacturer;
        }

        public void OnClickSelectProductName(string text)
        {
            if (string.IsNullOrEmpty(m_selectedCategory) || string.IsNullOrEmpty(m_selectedManufacturer))
            {
                return;
            }

            ProductDictionary dictionary = MunchDatabase.instance.productDictionary;
            Dictionary<string, ProductData> products = new Dictionary<string, ProductData>();
            if (dictionary.products.ContainsKey(m_selectedCategory))
            {
                if (dictionary.products[m_selectedCategory].ContainsKey(m_selectedManufacturer))
                {
                    dictionary.products[m_selectedCategory].TryGetValue(m_selectedManufacturer, out products);
                }
            }

            List<string> keys = new List<string>(products.Keys);
            SelectPopup selectPopup = PanelManager.Instance.AddPanel<SelectPopup>(PanelTag.SelectPopup, keys);
            selectPopup.SetSelectedTextAction(OnSelectedProductName);
        }

        private void OnSelectedProductName(string text)
        {
            m_selectedName = text;
            productNameText.text = m_selectedName;
        }

        private void SetDefaultManufacturerSelector()
        {
            m_selectedManufacturer = string.Empty;
            manufacturerText.text = SELECT_MANUFACTURER_TEXT;

            manufacturerText.color = Color.black;
            manufacturerSelectIcon.color = Color.white;
        }

        private void SetDefaultProductNameSelector()
        {
            m_selectedName = string.Empty;
            productNameText.text = SELECT_PRODUCTNAME_TEXT;

            productNameText.color = Color.black;
            productNameSelectIcon.color = Color.white;
        }

        private void SetDisableProductNameSelector()
        {
            m_selectedName = string.Empty;
            productNameText.text = SELECT_PRODUCTNAME_TEXT;

            productNameText.color = DISABLED_TEXT_COLOR;
            productNameSelectIcon.color = DISABLED_SELECTOR_ICON;
        }

        public void OnClickRegisterButton()
        {
            if (string.IsNullOrEmpty(m_selectedCategory) || string.IsNullOrEmpty(m_selectedManufacturer) || string.IsNullOrEmpty(m_selectedName))
            {
                // Draw Warning Popup
                return;
            }

            if (string.IsNullOrEmpty(descriptionInputField.text) || string.IsNullOrEmpty(priceInputField.text))
            {
                // Draw Warning Popup
                return;
            }

            int price = -1;
            if (int.TryParse(priceInputField.text, out price) == false)
            {
                // Draw Warning Popup
                return;
            }

            if (price < 0)
            {
                // Draw Warning Popup
                return;
            }

            ProductDictionary dictionary = MunchDatabase.instance.productDictionary;
            ProductData product = dictionary.GetProduct(m_selectedCategory, m_selectedManufacturer, m_selectedName);

            UserRegisteredProductData userRegisteredProductData = new UserRegisteredProductData();
            userRegisteredProductData.SetProductID(product.id);
            userRegisteredProductData.SetPrice(price);
            userRegisteredProductData.SetDescription(descriptionInputField.text);
            MunchDatabase.instance.RegisterProductData(userRegisteredProductData.GetFormattedProductData());

            OnClickCloseButton();
            string dirPath = $"{Application.persistentDataPath}/{MunchDatabase.PRODUCT_DATA_DIR}/{userRegisteredProductData.uid}";
            PanelManager.Instance.AddPanel<RegisterCompleteView>(PanelTag.RegisterCompleteView, new RegisteredProductInfo(dirPath));

            // PanelManager.Instance.AddPanel<ProductInformationView>(PanelTag.ProductInformationView, new RegisteredProductInfo(dirPath));

            // SearchResultView searchView = PanelManager.Instance.GetPanel<SearchResultView>(PanelTag.SearchResultView);
            // if(searchView != null)
            // {
            //     searchView.SetNewProductDir(dirPath);
            // }

        }

        public void OnClickPreviewButton()
        {
            // PanelManager.Instance
        }

        public void OnClickCloseButton()
        {
            PanelManager.Instance.RemovePanel(this);
        }
    }
}


