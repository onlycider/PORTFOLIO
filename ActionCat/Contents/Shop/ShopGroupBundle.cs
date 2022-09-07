using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopGroupBundle : MonoBehaviour
{
    protected Dictionary<int, ShopProductData> m_productDatas;
    protected View_Shop m_shopView;
    public View_Shop.ShopGroup shopGroup;

    public virtual void SetShopGroup()
    {
        m_productDatas = GameDatabase.Instance.shopProducts.GetShopGroup((int)shopGroup);
        m_shopView = GetComponentInParent<View_Shop>();
        // Debug.Log(m_products.Count);
    }

    public void SetParent(Transform _parent)
    {
        transform.SetParent(_parent);
        transform.localPosition = Vector3.zero;
    }

    public ShopProductData GetProductData(int _idx)
    {
        foreach(var item in m_productDatas)
        {
            if(    item.Value.productId == _idx)
                return item.Value;
        }
        return null;
    }
}
