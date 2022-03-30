using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MunchProject
{
    public partial class MunchDatabase
    {
        private Dictionary<string, object> m_visionApiResult;
        public Dictionary<string, object> visionApiResult { get { return m_visionApiResult; } }

        private AnnotateImageResultData mImageApiResultData;
        public AnnotateImageResultData ImageApiResultData { get { return mImageApiResultData; } }

        public void SetVisionApiResult(Dictionary<string, object> result)
        {
            m_visionApiResult = result;
            mImageApiResultData = new AnnotateImageResultData(result, DateTime.Now.ToString("yyyyMMddHHmmssfff"));
            mTopRankProducts = GetTopRankProducts();
        }

        public void SaveVisionApiResult(string directoryName)
        {
            if (m_visionApiResult == null)
            {
                return;
            }

            string annotatesImage = JsonFx.Json.JsonWriter.Serialize(m_visionApiResult);
            string filePath = $"{directoryName}api_result.txt";
            FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter writer = new StreamWriter(fileStream, System.Text.Encoding.UTF8);
            writer.Write(annotatesImage);
            writer.Close();
        }

        public string LoadVisionApiResult(string directoryName)
        {
            string filePath = $"{directoryName}/api_result.txt";
            //추후 Json으로 리턴
            return File.ReadAllText(filePath);
        }
    }
}