using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MunchProject
{
    public class TotalApiResult
    {
        private string m_apiResult;
        public string apiResult { get { return m_apiResult; } }
        private Texture2D m_resultTexture;
        public Texture2D resultTexture { get { return m_resultTexture; } }

        public TotalApiResult(string result, Texture2D texture)
        {
            m_apiResult = result;
            m_resultTexture = texture;
        }
    }
}
