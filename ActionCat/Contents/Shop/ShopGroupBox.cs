using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopGroupBox : ShopGroupBundle
{
    public ShopBoxSlot[] boxSlots;

    public override void SetShopGroup()
    {
        base.SetShopGroup();

        foreach(ShopBoxSlot product in boxSlots)
        {
            ShopProductData productData = m_productDatas[product.order];
            product.SetShopProductData(productData);
        }
    }

    public ShopBoxSlot GetBoxSlot(int _order)
    {
        foreach(ShopBoxSlot product in boxSlots)
        {
            if(product.order == _order)
                return product;
        }
        return null;
    }
}
