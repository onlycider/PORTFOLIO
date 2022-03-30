using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MunchProject
{
    public partial class MunchDatabase
    {
        private string m_userID = SystemInfo.deviceUniqueIdentifier;
        public string userID{get{return m_userID;}}
    }
}