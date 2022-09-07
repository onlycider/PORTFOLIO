using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopGroupCoin : ShopGroupBundle
{
    public ShopCoinSlot[] coinProducts;

    public override void SetShopGroup()
    {
        base.SetShopGroup();

        foreach(ShopCoinSlot coinProduct in coinProducts)
        {
            ShopProductData coinProductData = m_productDatas[coinProduct.order];
            coinProduct.SetShopProductData(coinProductData);
        }
    }

}
