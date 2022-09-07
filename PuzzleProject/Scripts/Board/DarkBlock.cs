using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IKPuzzle
{
    public class DarkBlock : PuzzleBlock
    {
        protected override void OnAwake()
        {
            base.OnAwake();
            m_attributeType = AttributeType.DARK;
        }
    }
}

