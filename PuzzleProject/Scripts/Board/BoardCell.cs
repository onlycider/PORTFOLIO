using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

namespace IKPuzzle
{
    public enum MoveDirection
    {
        NOT_MOVED,
        UP,
        DOWN,
        LEFT,
        RIGHT,
    }

    public class BoardCell : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        enum DragStatus
        {
            BEGIN,
            DRAGGING,
            END,
        }
        private RectTransform m_rectTransform;
        public TextMeshProUGUI cellNumText;

        private int m_cellNum;

        private int m_cellRowNum;
        public int cellRowNum { get { return m_cellRowNum; } }
        private int m_cellColumnNum;
        public int cellColumnNum { get { return m_cellColumnNum; } }

        private Board m_board;
        public Board board
        {
            set
            {
                m_board = value;
                m_cellNum = transform.GetSiblingIndex();
                if (m_board != null)
                {
                    int columnCount = m_board.columnCount;
                    m_cellRowNum = (m_cellNum / columnCount);
                    m_cellColumnNum = m_cellNum % columnCount;
                }
            }
        }

        private Vector2 m_dragStartPoint;
        private MoveDirection m_movedDirection = MoveDirection.NOT_MOVED;

        private PuzzleBlock m_puzzleBlock;
        public PuzzleBlock puzzleBlock { get { return m_puzzleBlock; } }

        private DragStatus m_dragStatus;

        void Awake()
        {
            m_rectTransform = GetComponent<RectTransform>();
        }

        // Start is called before the first frame update
        // void Start()
        // {

        // }

        private void SetRowColumnNum()
        {
            int columnCount = m_board.columnCount;

            m_cellRowNum = (m_cellNum / columnCount);
            m_cellColumnNum = m_cellNum % columnCount;
            TestSetRowColumnText();
        }

        public void InitializePuzzleBlock(AttributeType type)
        {
            GameObject puzzleObject = PuzzleBlockPool.instance.GetPuzzleBlock(type);

            if (puzzleObject == null)
            {
                return;
            }

            GameObject puzzleBlock = Instantiate<GameObject>(puzzleObject, this.transform);
            puzzleBlock.transform.localPosition = Vector3.zero;
            puzzleBlock.SetActive(true);
            m_puzzleBlock = puzzleBlock.GetComponent<PuzzleBlock>();
        }

        public void SetExchangingPuzzleBlock(PuzzleBlock block)
        {
            m_puzzleBlock = block;
            m_puzzleBlock.transform.SetParent(this.transform);
            m_puzzleBlock.transform.localPosition = Vector3.zero;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Debug.Log("OnBeginDrag :: " + eventData.position);
            m_movedDirection = MoveDirection.NOT_MOVED;

            m_dragStartPoint = eventData.position;

            // m_dragStatus = DragStatus.BEGIN;
        }

        public void OnDrag(PointerEventData eventData)
        {
            // if (m_dragStatus == DragStatus.DRAGGING || m_dragStatus == DragStatus.END)
            // {
            //     return;
            // }

            if (m_movedDirection != MoveDirection.NOT_MOVED)
            {
                return;
            }

            Debug.Log("OnDrag :: " + eventData.position);
            float moveDistance = Vector2.Distance(m_dragStartPoint, eventData.position);
            Debug.Log("OnDrag Move Distance :: " + moveDistance);

            float moveX = eventData.position.x - m_dragStartPoint.x;
            float moveY = eventData.position.y - m_dragStartPoint.y;

            MoveDirection horizontalDirection = MoveDirection.NOT_MOVED;
            MoveDirection verticalDirection = MoveDirection.NOT_MOVED;

            if (moveX > 0)
            {
                horizontalDirection = MoveDirection.RIGHT;
            }
            else if (moveX < 0)
            {
                horizontalDirection = MoveDirection.LEFT;
            }

            if (moveY > 0)
            {
                verticalDirection = MoveDirection.UP;
            }
            else if (moveY < 0)
            {
                verticalDirection = MoveDirection.DOWN;
            }

            float moveAbsX = Mathf.Abs(moveX);
            float moveAbsY = Mathf.Abs(moveY);

            if (moveAbsX > moveAbsY)
            {
                m_movedDirection = horizontalDirection;
            }
            else
            {
                m_movedDirection = verticalDirection;
            }

            Debug.Log(m_movedDirection);
            ExchangeBlockPosition();
            // m_dragStatus = DragStatus.DRAGGING;
        }

        private void ExchangeBlockPosition()
        {
            // Debug.Log("PrintChangingCellPosition --------");
            int rowNum = m_cellRowNum;
            int columnNum = m_cellColumnNum;
            switch (m_movedDirection)
            {
                case MoveDirection.UP:
                    ++rowNum;
                    break;
                case MoveDirection.DOWN:
                    --rowNum;
                    break;
                case MoveDirection.LEFT:
                    --columnNum;
                    break;
                case MoveDirection.RIGHT:
                    ++columnNum;
                    break;

                default:
                    break;
            }

            Debug.Log($"{rowNum}, {columnNum}");

            int maxRowIndex = m_board.columnCount - 1;
            if (rowNum < 0 || rowNum > maxRowIndex)
            {
                Debug.Log("Warning - end row index");
                return;
            }

            int maxCoulumnIndex = m_board.rowCount - 1;
            if (columnNum < 0 || columnNum > maxCoulumnIndex)
            {
                Debug.Log("Warning - end Column index");
                return;
            }

            BoardCell changingCell = m_board.boardCells[rowNum, columnNum];

            if (changingCell.puzzleBlock == null)
            {
                return;
            }

            Debug.Log("Exchanging ------ ");
            PuzzleBlock tempBlock = changingCell.puzzleBlock;
            changingCell.SetExchangingPuzzleBlock(m_puzzleBlock);
            SetExchangingPuzzleBlock(tempBlock);
            CalculateClearPuzzleBlock();
        }

        private void CalculateClearPuzzleBlock()
        {
            int horizontalStartIndex = FindHorizontalStartIndexRecursive();
            int horizontalEndIndex = FindHorizontalEndIndexRecursive();

            int horizontalLength = horizontalEndIndex - horizontalStartIndex;

            int verticalStartIndex = FindVerticalStartIndexRecursive();
            int verticalEndIndex = FindVerticalEndIndexRecursive();

            int verticalLength = verticalEndIndex - verticalStartIndex;

            if(horizontalLength >= 3)
            {
                Debug.Log("Horizontal Clear !!!! ");
            }

            if(verticalLength >= 3)
            {
                Debug.Log("vertical Clear !!!!" );
            }
        }

        public int FindHorizontalStartIndexRecursive()
        {
            int columnValue = m_cellColumnNum - 1;
            if (columnValue < 0 || columnValue > m_board.columnCount - 1)
            {
                return m_cellColumnNum;
            }

            BoardCell cell = m_board.boardCells[m_cellRowNum, columnValue];
            if(cell.puzzleBlock == null)
            {
                return m_cellColumnNum;
            }

            if (m_puzzleBlock.attributeType != cell.puzzleBlock.attributeType)
            {
                return m_cellColumnNum;
            }

            return cell.FindHorizontalStartIndexRecursive();
        }

        public int FindHorizontalEndIndexRecursive()
        {
            int columnValue = m_cellColumnNum + 1;
            if (columnValue < 0 || columnValue > m_board.columnCount - 1)
            {
                return m_cellColumnNum;
            }

            BoardCell cell = m_board.boardCells[m_cellRowNum, columnValue];

            if(cell.puzzleBlock == null)
            {
                return m_cellColumnNum;
            }

            if (m_puzzleBlock.attributeType != cell.puzzleBlock.attributeType)
            {
                return m_cellColumnNum;
            }

            return cell.FindHorizontalEndIndexRecursive();
        }

        public int FindVerticalStartIndexRecursive()
        {
            int rowValue = m_cellRowNum - 1;

            if (rowValue < 0 || rowValue > m_board.rowCount - 1)
            {
                return m_cellRowNum;
            }

            BoardCell cell = m_board.boardCells[rowValue, m_cellColumnNum];

            if(cell.puzzleBlock == null)
            {
                return m_cellRowNum;
            }

            if (m_puzzleBlock.attributeType != cell.puzzleBlock.attributeType)
            {
                return m_cellRowNum;
            }

            return cell.FindVerticalStartIndexRecursive();
        }

        public int FindVerticalEndIndexRecursive()
        {
            int rowValue = m_cellRowNum + 1;

            if (rowValue < 0 || rowValue > m_board.rowCount - 1)
            {
                return m_cellRowNum;
            }

            BoardCell cell = m_board.boardCells[rowValue, m_cellColumnNum];
            if(cell.puzzleBlock == null)
            {
                return m_cellRowNum;
            }
            
            if (m_puzzleBlock.attributeType != cell.puzzleBlock.attributeType)
            {
                return m_cellRowNum;
            }

            return cell.FindVerticalEndIndexRecursive();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Debug.Log("OnEndDrag :: " + eventData.position);
            m_dragStartPoint = Vector2.zero;

            // m_dragStatus = DragStatus.END;
            // m_movedDirection = MoveDirection.NOT_MOVED;
        }

        /// <summary>
        /// 
        /// </summary>
        private void TestSetRowColumnText()
        {
            cellNumText.text = $"{m_cellRowNum}, {m_cellColumnNum}";
        }
    }

}
