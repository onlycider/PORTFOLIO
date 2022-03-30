using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MunchProject
{
    public class RegisteredProductInfo
    {
        private UserRegisteredProductData mUserRegisteredProductData;
        public UserRegisteredProductData UserRegisteredProductData { get { return mUserRegisteredProductData; } }

        private string mDirectory;
        public string Directory { get { return mDirectory; } }

        private Texture2D mProductImage;
        public Texture2D ProductImage { get { return mProductImage; } }

        private string mApiResult;
        public string ApiResult { get { return mApiResult; } }

        private Texture2D mSquaredImage;
        public Texture2D SquaredImage { get { return mSquaredImage; } }

        private Texture2D mVgaImage;
        public Texture2D VgaImage { get { return mVgaImage; } }

        private ProductData mProductData;
        public ProductData ProductData { get { return mProductData; } }

        private SourceType mSourceType;
        public SourceType SourceType { get { return mSourceType; } }

        public RegisteredProductInfo(string directory)
        {
            mDirectory = directory;
            mUserRegisteredProductData = new UserRegisteredProductData(MunchDatabase.instance.LoadRegisteredJsonData(mDirectory));
            mProductData = MunchDatabase.instance.productDictionary.GetProductById(mUserRegisteredProductData.ProductID);
            mApiResult = MunchDatabase.instance.LoadVisionApiResult(mDirectory);
            mSourceType = SourceType.User;
        }

        public RegisteredProductInfo(RegisteredProductInfo info)
        {
            mDirectory = info.Directory;
            if (info.ProductImage != null)
            {
                mProductImage = new Texture2D(info.ProductImage.width, info.ProductImage.height);
                mProductImage.SetPixels(info.ProductImage.GetPixels());
                mProductImage.Apply();
            }
            mProductData = info.ProductData;
            mUserRegisteredProductData = info.UserRegisteredProductData;
            mApiResult = info.ApiResult;
            mSourceType = info.SourceType;
        }

        public RegisteredProductInfo(string uid, Texture2D texture2D)
        {
            mUserRegisteredProductData = new UserRegisteredProductData(uid);
            mProductImage = new Texture2D(texture2D.width, texture2D.height);
            mProductImage.SetPixels(texture2D.GetPixels());
            mProductImage.Apply();
            mSourceType = SourceType.Sample;
        }

        public void ShowVisionApiResult()
        {
            Debug.Log(mApiResult);
        }


        public Texture2D GetSquareTexture()
        {
            if (mProductImage == null)
            {
                mProductImage = MunchDatabase.instance.LoadRegisterdProductImage(mDirectory);
            }

            if (mSquaredImage != null)
            {
                return mSquaredImage;
            }

            int w = mProductImage.width;
            int h = mProductImage.height;

            const int cropWidth = 1080;

            int yOffset = (h / 2) - (cropWidth / 2);

            int cropIndex = 0;

            Color32[] source = mProductImage.GetPixels32();
            Color32[] centerImage = new Color32[cropWidth * cropWidth];
            for (int y = 0; y < cropWidth; y++)
            {
                for (int x = 0; x < cropWidth; x++)
                {
                    cropIndex = x + (y + yOffset) * cropWidth;
                    centerImage[y * cropWidth + x] = source[cropIndex];
                }
            }

            mSquaredImage = new Texture2D(cropWidth, cropWidth, mProductImage.format, false);
            mSquaredImage.SetPixels32(centerImage);
            mSquaredImage.Apply();

            DestroyProductImage();
            return mSquaredImage;
        }

        public Texture2D GetVgaTexture()
        {
            if (mProductImage == null)
            {
                mProductImage = MunchDatabase.instance.LoadRegisterdProductImage(mDirectory);
            }

            if (mVgaImage != null)
            {
                return mVgaImage;
            }

            int w = mProductImage.width;
            int h = mProductImage.height;


            const int cropWidth = 1080;
            const int cropHeight = 810;

            int yOffset = (h / 2) - (cropHeight / 2);

            Color32[] source = mProductImage.GetPixels32();
            Color32[] centerImage = new Color32[cropWidth * cropHeight];

            int cropIndex = 0;
            for (int y = 0; y < cropHeight; y++)
            {
                for (int x = 0; x < cropWidth; x++)
                {
                    cropIndex = ((y + yOffset) * cropWidth) + x;
                    centerImage[y * cropWidth + x] = source[cropIndex];
                }
            }

            mVgaImage = new Texture2D(cropWidth, cropHeight, mProductImage.format, false);
            mVgaImage.SetPixels32(centerImage);
            mVgaImage.Apply();

            DestroyProductImage();
            return mVgaImage;
        }

        public void DestroyInstance()
        {
            mUserRegisteredProductData = null;
            // mProductData = null;

            DestroyProductImage();
            DestroySquareImage();
            DestroyVgaImage();
        }

        public void DestroyProductImage()
        {
            if (mProductImage != null)
            {
                MonoBehaviour.DestroyImmediate(mProductImage);
                mProductImage = null;
            }
        }

        public void DestroySquareImage()
        {
            if (mSquaredImage != null)
            {
                MonoBehaviour.DestroyImmediate(mSquaredImage);
                mSquaredImage = null;
            }
        }

        public void DestroyVgaImage()
        {
            if (mVgaImage != null)
            {
                MonoBehaviour.DestroyImmediate(mVgaImage);
                mVgaImage = null;
            }
        }
    }
}
