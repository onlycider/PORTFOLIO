using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MunchProject
{
    public class RegisterPreviewView : Panel
    {
        public ProductCard productCard;

        private RegisteredProductInfo m_currentRegisteredProduct;

        public override void SetPanelInfo(object info)
        {
            base.SetPanelInfo(info);
            m_currentRegisteredProduct = info as RegisteredProductInfo;
            productCard.SetProductCard(m_currentRegisteredProduct);
        }

        public void OnClickCloseButton()
        {
            PanelManager.Instance.RemovePanel(this);
        }
    }
}
