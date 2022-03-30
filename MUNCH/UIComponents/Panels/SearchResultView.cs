using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MunchProject
{
    public class SearchResultView : Panel
    {
        public RawImage userProductImage;

        private List<RegisteredProductInfo> mRegisteredProducts;
        public List<RegisteredProductInfo> RegisteredProducts { get { return mRegisteredProducts; } }

        public SearchedProductSlot[] searchedProductSlots;

        private string m_newProductDir;

        protected override void OnAwake()
        {
            base.OnAwake();
            LoadRegisteredProductInfos();
            // TestLoadUserRegisteredProductInfos();
        }

        protected override void OnStart()
        {
            base.OnStart();
            SetUserProductImage();
        }

        public void SetNewProductDir(string dir)
        {
            m_newProductDir = dir;
        }

        private void SetUserProductImage()
        {
            userProductImage.texture = MunchDatabase.instance.GetSquareTexture();
        }

        private void LoadRegisteredProductInfos()
        {
            List<ImageRate> topRankProducts = MunchDatabase.instance.TopRankProducts;
            mRegisteredProducts = new List<RegisteredProductInfo>();
            // int count = 0;
            foreach (ImageRate imageRate in topRankProducts)
            {
                // if (++count > 5)
                // {
                //     break;
                // }

                switch (imageRate.SourceType)
                {
                    case SourceType.Sample:
                        mRegisteredProducts.Add(new RegisteredProductInfo(imageRate.UID, Resources.Load<Texture2D>($"SampleProducts/{imageRate.UID}")));
                        break;
                    case SourceType.User:
                        mRegisteredProducts.Add(new RegisteredProductInfo($"{imageRate.UID}"));
                        break;
                }
            }
            DrawRegisteredProducts();
        }

        private void TestLoadUserRegisteredProductInfos()
        {
            mRegisteredProducts = new List<RegisteredProductInfo>();
            string[] directories = MunchDatabase.instance.GetRegisteredProductInfos();
            foreach (string dir in directories)
            {
                mRegisteredProducts.Add(new RegisteredProductInfo(dir));
            }

            Debug.Log(directories.Length);
            DrawRegisteredProducts();
        }

        private void DrawRegisteredProducts()
        {
            int drawCount = searchedProductSlots.Length;

            // drawCount = drawCount > registeredProducts.Count ? registeredProducts.Count : drawCount;

            if (mRegisteredProducts.Count > drawCount)
            {
                mRegisteredProducts.RemoveRange(drawCount, mRegisteredProducts.Count - drawCount);
            }

            for (int i = 0; i < drawCount; ++i)
            {
                if (mRegisteredProducts.Count > i)
                {
                    searchedProductSlots[i].SetRegisteredProductInfo(mRegisteredProducts[i]);
                    continue;
                }

                searchedProductSlots[i].InactivateSlot();
            }
        }

        public void OnClickUserSearchProduct()
        {
            if (string.IsNullOrEmpty(m_newProductDir))
            {
                PanelManager.Instance.AddPanel<ProductRegisterView>(PanelTag.ProductRegisterView);
                return;
            }

            PanelManager.Instance.AddPanel<ProductInformationView>(PanelTag.ProductInformationView, new RegisteredProductInfo(m_newProductDir));
        }

        public void OnClickTestMenuButton()
        {
            // Dictionary<string, float> matchRates = new Dictionary<string, float>();
            // string matchRateList = string.Empty;
            // foreach (AnnotateImageResultData data in MunchDatabase.instance.Samples.Values)
            // {
            //     float matchRate = data.GetMatchRate(MunchDatabase.instance.ImageApiResultData.Labels);
            //     matchRateList += $"{data.UID}\t{matchRate}\n";
            // }
            // Debug.Log(matchRateList);
        }

        public void OnClickCloseButton()
        {
            MunchDatabase.instance.InitializeProductImage();
            MunchSceneManager.LoadScene(EScene.Camera);
        }

        public override void InvokeBackKey()
        {
            base.InvokeBackKey();
            OnClickCloseButton();
        }
    }
}

