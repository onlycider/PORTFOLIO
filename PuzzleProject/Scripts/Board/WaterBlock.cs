using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IKPuzzle
{
    public class WaterBlock : PuzzleBlock
    {
        protected override void OnAwake()
        {
            base.OnAwake();
            m_attributeType = AttributeType.FIRE;
        }
    }
}

