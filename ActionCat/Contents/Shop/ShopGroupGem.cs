using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopGroupGem : ShopGroupBundle
{
    public ShopGemSlot[] products;

    public override void SetShopGroup()
    {
        base.SetShopGroup();
        foreach(ShopGemSlot product in products)
        {
            ShopProductData productData = m_productDatas[product.order];
            product.SetShopProductData(productData);
        }
    }

    private void DrawProducts()
    {

    }
}
