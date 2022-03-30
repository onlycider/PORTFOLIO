using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace MunchProject
{
    public class MainCameraView : Panel
    {
        public enum EDirection
        {
            NOT_MOVED,
            UP,
            DOWN,
            LEFT,
            RIGHT,
        }
        public CamScreen cameraScreen;

        private Texture2D m_croppedImage;
        public Texture2D croppedImage { get { return m_croppedImage; } }

        public DragArea dragArea;

        private Vector2 mDragStartPosition;

        private EDirection mMovedDirection = EDirection.NOT_MOVED;

        public RectTransform menuTransform;

        private int mCurrentMenu = 0;

        private const int MENU_COUNT = 4;

        private bool m_isMoving = false;

        public MenuButton[] menuButtons;

        protected override void OnAwake()
        {
            base.OnAwake();
            dragArea.ACTION_BEGIN_DRAG = OnBeginDrag;
            dragArea.ACTION_DRAG = OnDrag;
            dragArea.ACTION_END_DRAG = OnEndDrag;
        }

        protected override void OnStart()
        {
            base.OnStart();
            dragArea.SetDragAreaSize(cameraScreen.camScreenWidth, cameraScreen.camScreenHeight);
            SetMenuButtons();
        }

        private void SetMenuButtons()
        {
            menuButtons[0].SetSelectionStatus();
            foreach (MenuButton button in menuButtons)
            {
                button.MENU_CLICK_ACTION = OnClickMenuButton;
            }
        }

        private void OnClickMenuButton(int index)
        {
            int moved = mCurrentMenu - index;

            if (moved == 0)
            {
                TakingPicture();
                return;
            }

            m_isMoving = true;
            Vector2 prePosition = menuTransform.anchoredPosition;
            Vector2 endPosition = prePosition + new Vector2(moved * 220f, 0f);
            menuTransform.DOAnchorPos(endPosition, 0.4f).OnComplete(() => { m_isMoving = false; });
            menuButtons[mCurrentMenu].TransitionOriginalStatus();
            menuButtons[index].TransitionSelectionStatus();

            mCurrentMenu = index;
        }

        public void TakingPicture()
        {
            cameraScreen.PauseWebCamTexture();
            MunchDatabase.instance.SetProductImage(cameraScreen.GetPauseWebCamTexture());
            CropImage();
        }

        private void CropImage()
        {
            if (ImageCropper.Instance.IsOpen)
                return;
            Texture2D originalImage = MunchDatabase.instance.originalImage;
            ImageCropper.Settings settings = new ImageCropper.Settings() { };
            ImageCropper.Instance.MarkTextureNonReadable = false;
            ImageCropper.Instance.Show(originalImage, ResultCrop, settings, CroppedImageResizePolicy);
        }

        private void CroppedImageResizePolicy(ref int width, ref int height)
        {
            Debug.Log(width);
            Debug.Log(height);
        }

        private void ResultCrop(bool result, Texture originalImage, Texture2D croppedImage)
        {
            if (result == false)
            {
                CancelCrop();
                return;
            }

            m_croppedImage = new Texture2D(croppedImage.width, croppedImage.height, croppedImage.format, false);
            m_croppedImage.LoadRawTextureData(croppedImage.GetRawTextureData());
            m_croppedImage.Apply();

            Debug.Log(Time.realtimeSinceStartup);


#if UNITY_EDITOR
            TextAsset textAsset = Resources.Load<TextAsset>("SampleData/api_sample8_powerade");
            SetVisionApiResultText(textAsset);
#else
            CloudVisionApi.SendVisionApi(m_croppedImage, SetVisionApiResultText);
#endif
        }

        private void SetVisionApiResultText(TextAsset results)
        {
            if (m_croppedImage == null)
                return;
            Debug.Log("SetVisionApiResultText ------------------------------");
            Dictionary<string, object> apiResult = JsonFx.Json.JsonReader.Deserialize<Dictionary<string, object>>(results.text);
            MunchDatabase.instance.SetVisionApiResult(apiResult);
            MunchSceneManager.LoadScene(EScene.ProductManagingScene);
        }

        private void SetVisionApiResultText(string results)
        {
            if (m_croppedImage == null)
                return;
            Debug.Log("SetVisionApiResultText ------------------------------");
            Dictionary<string, object> resultData = JsonFx.Json.JsonReader.Deserialize<Dictionary<string, object>>(results);
            Dictionary<string, object>[] responses = resultData["responses"] as Dictionary<string, object>[];
            Dictionary<string, object> apiResult = responses[0] as Dictionary<string, object>;
            
            MunchDatabase.instance.SetVisionApiResult(apiResult);
            MunchSceneManager.LoadScene(EScene.ProductManagingScene);
        }

        private void CancelCrop()
        {
            cameraScreen.ResumeWebCamTexture();
        }

        public void OnClickAlbumButton()
        {
            cameraScreen.PauseWebCamTexture();
            PickImage(1280);
        }

        private void PickImage(int maxSize)
        {
            Texture2D pictureTexture = PictureManager.PickPictureToLocalStorage(maxSize);

            // Error
            if (pictureTexture == null)
            {
                return;
            }

            if (ImageCropper.Instance.IsOpen)
                return;

            ImageCropper.Settings settings = new ImageCropper.Settings() { };
            ImageCropper.Instance.MarkTextureNonReadable = false;
            ImageCropper.Instance.Show(pictureTexture, ResultCrop, settings, CroppedImageResizePolicy);

            // m_selectedTexture = new Texture2D(pictureTexture.width, pictureTexture.height, pictureTexture.format, false);
            // m_selectedTexture.LoadRawTextureData(pictureTexture.GetRawTextureData());
            // m_selectedTexture.Apply();

            // rawImage.texture = m_selectedTexture;
            // float width = (float)m_selectedTexture.width;
            // float height = (float)m_selectedTexture.height;
            // rawImage.rectTransform.sizeDelta = new Vector2(width, height);

            Destroy(pictureTexture);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if(m_isMoving)
            {
                return;
            }
            
            mDragStartPosition = eventData.position;
            mMovedDirection = EDirection.NOT_MOVED;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if(m_isMoving)
            {
                return;
            }

            if (mMovedDirection != EDirection.NOT_MOVED)
            {
                return;
            }
            
            float moveX = eventData.position.x - mDragStartPosition.x;
            float moveY = eventData.position.y - mDragStartPosition.y;

            EDirection horizontalDirection = EDirection.NOT_MOVED;
            EDirection verticalDirection = EDirection.NOT_MOVED;

            if (moveX > 0)
            {
                horizontalDirection = EDirection.RIGHT;
            }
            else if (moveX < 0)
            {
                horizontalDirection = EDirection.LEFT;
            }

            if (moveY > 0)
            {
                verticalDirection = EDirection.UP;
            }
            else if (moveY < 0)
            {
                verticalDirection = EDirection.DOWN;
            }

            float moveAbsX = Mathf.Abs(moveX);
            float moveAbsY = Mathf.Abs(moveY);

            if (moveAbsX > moveAbsY)
            {
                mMovedDirection = horizontalDirection;
            }
            else
            {
                mMovedDirection = verticalDirection;
            }

            SelectMenu();
        }

        private void SelectMenu()
        {
            int selectedMenu = mCurrentMenu;
            switch (mMovedDirection)
            {
                case EDirection.LEFT:
                    if (++selectedMenu >= MENU_COUNT)
                    {
                        selectedMenu = MENU_COUNT - 1;
                    }
                    break;
                case EDirection.RIGHT:
                    if (--selectedMenu < 0)
                    {
                        selectedMenu = 0;
                    }
                    break;
            }

            int moved = mCurrentMenu - selectedMenu;

            if (moved == 0)
            {
                return;
            }

            m_isMoving = true;
            Vector2 prePosition = menuTransform.anchoredPosition;
            Vector2 endPosition = prePosition + new Vector2(moved * 220f, 0f);
            menuTransform.DOAnchorPos(endPosition, 0.4f).OnComplete(() => { m_isMoving = false; });
            menuButtons[mCurrentMenu].TransitionOriginalStatus();
            menuButtons[selectedMenu].TransitionSelectionStatus();

            mCurrentMenu = selectedMenu;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            mDragStartPosition = Vector2.zero;
        }
    }
}