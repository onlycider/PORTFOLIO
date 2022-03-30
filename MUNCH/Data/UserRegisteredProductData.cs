using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MunchProject
{
    public class UserRegisteredProductData
    {
        private string m_uid;
        public string uid { get { return m_uid; } }

        private string m_userID;
        public string UserID { get { return m_userID; } }

        private int m_productID;
        public int ProductID { get { return m_productID; } }

        private int m_price;
        public int Price { get { return m_price; } }

        private string m_description;
        public string Description { get { return m_description; } }

        public UserRegisteredProductData()
        {
            m_uid = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            m_userID = MunchDatabase.instance.userID;
        }

        public UserRegisteredProductData(string uid)
        {
            m_uid = uid;
            m_userID = MunchDatabase.instance.userID;

            //
            m_productID = 0;
            m_price = 0;
            m_description = string.Empty;
        }

        public UserRegisteredProductData(Dictionary<string, object> data)
        {
            m_uid = (string)data["id"];
            m_userID = (string)data["user_id"];
            m_productID = (int)data["product_id"];
            m_description = (string)data["description"];
            m_price = (int)data["price"];
        }

        public void SetProductID(int id)
        {
            m_productID = id;
        }

        public void SetPrice(int price)
        {
            m_price = price;
        }

        public void SetDescription(string description)
        {
            m_description = description;
        }

        public Dictionary<string, object> GetFormattedProductData()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("id", m_uid);
            data.Add("user_id", m_userID);
            data.Add("product_id", m_productID);
            data.Add("description", m_description);
            data.Add("price", m_price);
            return data;
        }
    }
}