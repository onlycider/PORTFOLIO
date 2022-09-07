using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IKPuzzle
{
    public class Board : MonoBehaviour
    {
        private BoardCell[] m_cells;
        public BoardCell[] cell { get { return m_cells; } }

        private MultiKeyDictionary<int, int, BoardCell> m_boardCells;
        public MultiKeyDictionary<int, int, BoardCell> boardCells{get{return m_boardCells;}}

        public GridLayoutGroup grid;

        private int m_columnCount;
        public int columnCount { get { return m_columnCount; } }

        private int m_rowCount;
        public int rowCount { get { return m_rowCount; } }

        void Awake()
        {
            m_cells = GetComponentsInChildren<BoardCell>();
            m_columnCount = grid.constraintCount;
            m_rowCount = m_cells.Length / m_columnCount;
            SetBoardToCells();
        }

        public void SetBoardToCells()
        {
            m_boardCells = new MultiKeyDictionary<int, int, BoardCell>();
            foreach (BoardCell cell in m_cells)
            {
                cell.board = this;
                m_boardCells.Add(cell.cellRowNum, cell.cellColumnNum, cell);
            }
        }

        public void GetBoardCell()
        {
            BoardCell[] childCells = transform.GetComponentsInChildren<BoardCell>();
        }
    }
}

