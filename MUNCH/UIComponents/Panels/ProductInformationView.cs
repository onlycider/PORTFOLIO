using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Linq;

namespace MunchProject
{
    public class ProductInformationView : Panel
    {
        public ProductCard productCard;

        private Vector2 mPositionStart;
        private Vector2 m_endDragPoint;

        private RegisteredProductInfo m_currentRegisteredProduct;

        private string[] m_dirs;

        private int m_currentDirIndex;
        private int m_nextDirIndex;

        private List<ProductCard> m_cards;

        private Vector2 TOP_STANDARD_POINT = new Vector2(0, 1440f);

        private const float LIMIT_MOVING_DISTANCE = 250f;
        private const float MOVE_SPEED = 6000f;

        public DragArea dragArea;

        private bool mIsMoving = false;

        private List<RegisteredProductInfo> searchedProducts;

        public override void SetPanelInfo(object info)
        {
            base.SetPanelInfo(info);
            RegisteredProductInfo productInfo = info as RegisteredProductInfo;
            switch (productInfo.SourceType)
            {
                case SourceType.Sample:
                    m_currentRegisteredProduct = new RegisteredProductInfo(productInfo.UserRegisteredProductData.uid, productInfo.ProductImage);
                    break;

                case SourceType.User:
                    m_currentRegisteredProduct = new RegisteredProductInfo(productInfo.Directory);
                    break;
            }

            productInfo.ShowVisionApiResult();
            productCard.SetProductCard(m_currentRegisteredProduct);

            SearchResultView searchedView = PanelManager.Instance.GetPanel<SearchResultView>(PanelTag.SearchResultView);
            searchedProducts = searchedView.RegisteredProducts;

            dragArea.ACTION_BEGIN_DRAG = OnBeginDrag;
            dragArea.ACTION_DRAG = OnDrag;
            dragArea.ACTION_END_DRAG = OnEndDrag;

            CheckProducts();
        }

        private void CheckProducts()
        {
            // m_dirs = MunchDatabase.instance.GetRegisteredProductInfos();
            // if (m_dirs.Length <= 1)
            // {
            //     return;
            // }

            if (searchedProducts.Count <= 1)
            {
                return;
            }

            // m_currentDirIndex = MunchDatabase.instance.GetDirectoryIndex(m_currentRegisteredProduct.UserRegisteredProductData.uid);
            RegisteredProductInfo currentInfo = searchedProducts.Where(m => m.UserRegisteredProductData.uid == m_currentRegisteredProduct.UserRegisteredProductData.uid).FirstOrDefault();
            m_currentDirIndex = searchedProducts.IndexOf(currentInfo);
            m_nextDirIndex = m_currentDirIndex + 1;
            if (searchedProducts.Count <= m_nextDirIndex)
            {
                m_nextDirIndex = 0;
            }

            m_cards = new List<ProductCard>();
            ProductCard addedCard = Instantiate<ProductCard>(productCard, dragArea.transform);
            addedCard.InitializeAnchoredPosition();
            addedCard.transform.SetAsFirstSibling();
            Debug.Log($"{searchedProducts.Count}, {m_nextDirIndex}" );
            addedCard.SetProductCard(new RegisteredProductInfo(searchedProducts[m_nextDirIndex]));

            m_cards.Add(productCard);
            m_cards.Add(addedCard);
        }

        public void ExchangeCard()
        {
            if (searchedProducts == null || searchedProducts.Count <= 0)
            {
                return;
            }
            // if (m_dirs == null)
            // {
            //     return;
            // }
            m_currentDirIndex = m_nextDirIndex;
            m_nextDirIndex = m_currentDirIndex + 1;
            if (searchedProducts.Count <= m_nextDirIndex)
            {
                m_nextDirIndex = 0;
            }

            productCard.DestoryProductCard();

            productCard.InitializeAnchoredPosition();
            productCard.transform.SetAsFirstSibling();
            productCard.SetProductCard(new RegisteredProductInfo(searchedProducts[m_nextDirIndex]));
            productCard.SetCardEulerAngles(Vector3.zero);

            m_cards.Reverse();
            productCard = m_cards[0];

            mIsMoving = false;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (mIsMoving)
            {
                return;
            }
            mPositionStart = eventData.position;

            if (mPositionStart.y > ScreenUtil.ScreenCenterY)
            {
                productCard.SetTwistPivotType(ProductCard.ETwistPivotTypes.Bottom);
            }
            else
            {
                productCard.SetTwistPivotType(ProductCard.ETwistPivotTypes.Top);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (mIsMoving)
            {
                return;
            }

            m_endDragPoint = eventData.position;

            Vector2 movePoint = m_endDragPoint - mPositionStart;
            productCard.SetAnchoredPosition(movePoint + Vector2.zero);

            Vector2 movedX = new Vector2(movePoint.x, 0f);
            float height = TOP_STANDARD_POINT.y; // 1440 샘플,
            float hypotenuse = Vector2.Distance(TOP_STANDARD_POINT, movedX);

            float rad = Mathf.Acos(height / hypotenuse);
            float degree = rad * Mathf.Rad2Deg;

            if (movePoint.x > 0)
            {
                productCard.TwistCardAngles(new Vector3(0f, 0f, degree));
            }
            else
            {
                productCard.TwistCardAngles(new Vector3(0f, 0f, -degree));
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (mIsMoving)
            {
                return;
            }

            m_endDragPoint = eventData.position;

            if (searchedProducts.Count <= 1)
            {
                CancelDescision();
                return;
            }

            float distance = Vector2.Distance(m_endDragPoint, mPositionStart);
            if (distance > LIMIT_MOVING_DISTANCE)
            {
                ProceedDescision();
            }
            else
            {
                CancelDescision();
            }
        }

        private void ProceedDescision()
        {
            StartCoroutine(BlowUpCard());
        }

        private IEnumerator BlowUpCard()
        {
            mIsMoving = true;
            Vector2 directionVector = (m_endDragPoint - mPositionStart).normalized;

            float distance = 0f;
            while (distance <= 2200f)
            {
                Vector2 movingVector = directionVector * MOVE_SPEED * Time.deltaTime;

                productCard.MovingCard(movingVector);
                distance = Vector2.Distance(Vector2.zero, productCard.anchoredPosition);
                yield return new WaitForEndOfFrame();
            }
            ExchangeCard();
        }

        private void CancelDescision()
        {
            StartCoroutine(ReturningProductCard());
        }

        private IEnumerator ReturningProductCard()
        {
            mIsMoving = true;
            float distance = Vector2.Distance(Vector2.zero, productCard.anchoredPosition);
            while (distance > 0.001f)
            {
                Vector2 movingVector = Vector2.Lerp(productCard.anchoredPosition, Vector2.zero, 0.5f);
                productCard.SetAnchoredPosition(movingVector);

                float standardDegree = productCard.twistedEulerAngles.z > 270f ? 360f : 0f;
                float twistingDegree = Mathf.Lerp(productCard.twistedEulerAngles.z, standardDegree, 0.5f);
                productCard.SetCardEulerAngles(new Vector3(0, 0, twistingDegree));

                distance = Vector2.Distance(productCard.anchoredPosition, Vector2.zero);
                yield return new WaitForEndOfFrame();
            }

            productCard.MovingCard(Vector2.zero);
            productCard.SetCardEulerAngles(Vector3.zero);
            mIsMoving = false;
        }

        public void OnClickCloseButton()
        {
            PanelManager.Instance.RemovePanel(this);
        }
    }
}
