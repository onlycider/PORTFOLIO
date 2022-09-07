using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IKPuzzle
{
    public enum AttributeType
    {
        NONE,
        FIRE,
        WATER,
        EARTH,
        LIGHT,
        DARK,
    }

    public class PuzzleBlock : MonoBehaviour
    {
        protected AttributeType m_attributeType;
        public AttributeType attributeType{get{return m_attributeType;}}

        void Awake()
        {
            OnAwake();
        }

        // Start is called before the first frame update
        void Start()
        {
            OnStart();
        }

        protected virtual void OnAwake(){}
        protected virtual void OnStart(){}

        
    }
}
