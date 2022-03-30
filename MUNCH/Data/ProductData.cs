using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MunchProject
{
    public class ProductDictionary
    {
        private MultiKeyDictionary<string, string, string, ProductData> m_products;
        public MultiKeyDictionary<string, string, string, ProductData> products { get { return m_products; } }

        private Dictionary<int, ProductData> mProductByID;


        public ProductDictionary(Dictionary<string, object>[] datas)
        {
            m_products = new MultiKeyDictionary<string, string, string, ProductData>();
            mProductByID = new Dictionary<int, ProductData>();
            foreach (Dictionary<string, object> data in datas)
            {
                ProductData product = new ProductData(data);
                m_products.Add(product.category, product.manufacturer, product.productName, product);
                mProductByID.Add(product.id, product);
            }
        }

        public ProductData GetProduct(string category, string manufacturer, string name)
        {
            if (!m_products.ContainsKey(category, manufacturer, name))
            {
                return null;
            }

            return m_products[category, manufacturer, name];
        }

        public ProductData GetProductById(int id)
        {
            ProductData product = null;
            if (mProductByID.ContainsKey(id))
            {
                product = mProductByID[id];
            }
            return product;
        }
    }

    public class ProductData
    {
        private int m_id;
        public int id { get { return m_id; } }

        private string m_category = string.Empty;
        public string category { get { return m_category; } }

        private string m_productName;
        public string productName { get { return m_productName; } }

        private string m_manufacturer;
        public string manufacturer { get { return m_manufacturer; } }

        private string m_modelName;
        public string modelName { get { return m_modelName; } }

        private int m_price;
        public int price { get { return m_price; } }

        public ProductData(Dictionary<string, object> data)
        {
            m_id = (int)data["id"];
            m_category = (string)data["category"];
            m_productName = (string)data["name"];
            m_manufacturer = (string)data["manufacturer"];
            m_modelName = (string)data["model_name"];
            m_price = (int)data["price"];
        }
    }
}