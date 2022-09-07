using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IKPuzzle
{
    public class PuzzleBlockPool : MonoBehaviour
    {
        public static PuzzleBlockPool instance = null;

        public PuzzleBlock[] puzzleBlocks;

        void Awake()
        {
            instance = this;
        }

        public GameObject GetPuzzleBlock(AttributeType type)
        {
            int index = (int)type - 1;
            if(index < 0)
            {
                return null;
            }
            return puzzleBlocks[index].gameObject;
        }
    }
}