using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IKPuzzle
{
    public class FireBlock : PuzzleBlock
    {
        protected override void OnAwake()
        {
            base.OnAwake();
            m_attributeType = AttributeType.FIRE;
        }
        // protected override void OnStart()
        // {
        //     base.OnStart();
            
        // }
    }
}
