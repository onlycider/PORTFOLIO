using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MunchProject
{
    public class RegisterCompleteView : Panel
    {
        public ProductCard productCard;
        private RegisteredProductInfo m_currentRegisteredProduct;

        public GameObject saveCompleteToastMessage;

        public override void SetPanelInfo(object info)
        {
            base.SetPanelInfo(info);
            RegisteredProductInfo productInfo = info as RegisteredProductInfo;
            m_currentRegisteredProduct = new RegisteredProductInfo(productInfo.Directory);
            productCard.SetProductCard(m_currentRegisteredProduct);
            productCard.SAVE_CARD_ACTION = CallbackSucceededSaveCard;
            saveCompleteToastMessage.SetActive(false);
        }

        public void OnClickSaveCardToAlbum()
        {
            productCard.SaveCardToAlbum();
        }

        public void CallbackSucceededSaveCard()
        {
            StartCoroutine(ShowToastMessage());
        }

        private IEnumerator ShowToastMessage()
        {
            saveCompleteToastMessage.SetActive(true);
            yield return new WaitForSeconds(1.5f);
            saveCompleteToastMessage.SetActive(false);
        }

        public void OnClickCloseButton()
        {
            MunchDatabase.instance.InitializeProductImage();
            MunchSceneManager.LoadScene(EScene.Camera);
        }
    }
}