using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MunchProject
{
    public partial class MunchDatabase
    {
        public ProductDictionary productDictionary
        {
            get
            {
                if(m_productDictionary == null)
                {
                    m_productDictionary = new ProductDictionary(LoadJsonData("sample_meta_data"));
                }

                return m_productDictionary;
            }
        }
        private ProductDictionary m_productDictionary;
    }
}

