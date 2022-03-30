using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MunchProject
{
    public partial class MunchDatabase
    {
        private Dictionary<string, AnnotateImageResultData> mSamples;
        public Dictionary<string, AnnotateImageResultData> Samples { get { return mSamples; } }

        private List<ImageRate> mTopRankProducts;
        public List<ImageRate> TopRankProducts { get { return mTopRankProducts; } }

        public void LoadResourcesSamples()
        {
            Debug.Log("LoadResourcesSamples ##");
            mSamples = new Dictionary<string, AnnotateImageResultData>();
            TextAsset[] samples = Resources.LoadAll<TextAsset>("SampleProducts");

            foreach (TextAsset sample in samples)
            {
                Dictionary<string, object> sampleData = JsonFx.Json.JsonReader.Deserialize<Dictionary<string, object>>(sample.text);
                AnnotateImageResultData sampleResult = new AnnotateImageResultData(sampleData, sample.name);
                sampleResult.SetSourceType(SourceType.Sample);
                mSamples.Add(sampleResult.UID, sampleResult);
            }
        }

        public void LoadRegisteredProduct()
        {
            if (mSamples == null)
            {
                mSamples = new Dictionary<string, AnnotateImageResultData>();
            }

            string[] dirs = GetRegisteredProductInfos();
            foreach (string dir in dirs)
            {
                string apiResult = LoadVisionApiResult(dir);

                Dictionary<string, object> sampleData = JsonFx.Json.JsonReader.Deserialize<Dictionary<string, object>>(apiResult);
                AnnotateImageResultData sampleResult = new AnnotateImageResultData(sampleData, dir);
                sampleResult.SetSourceType(SourceType.User);
                mSamples.Add(sampleResult.UID, sampleResult);
            }
        }

        public List<ImageRate> GetTopRankProducts()
        {
            if (mImageApiResultData == null)
            {
                return null;
            }
            List<ImageRate> matchRates = new List<ImageRate>();
            foreach (AnnotateImageResultData data in MunchDatabase.instance.Samples.Values)
            {
                float matchRate = data.GetMatchRate(mImageApiResultData.LabelAnnotations);
                float objMatchRate = data.GetObjectMatchRate(mImageApiResultData.ObjectAnnotations);

                if (matchRate + objMatchRate < 0.24f)
                {
                    continue;
                }

                // float colorScore = data.GetColorScore(mImageApiResultData.PropertiesAnnotaion);
                ImageRate imageRate = new ImageRate(data, matchRate, objMatchRate, data.PropertiesAnnotaion);
                matchRates.Add(imageRate);
            }

            matchRates.Sort(CompareMatchRate);

            string matchRateChart = string.Empty;

            matchRateChart = $"촬영RGB\t{mImageApiResultData.PropertiesAnnotaion.Red}\t{mImageApiResultData.PropertiesAnnotaion.Green}\t{mImageApiResultData.PropertiesAnnotaion.Blue}\n";
            foreach (ImageRate matchRate in matchRates)
            {
                // matchRateChart += $"{matchRate.UID}\t{matchRate.MatchRate}\t{matchRate.ObjectMatchRate}\t{matchRate.ColorMatchScore}\t{matchRate.MatchRate + matchRate.ObjectMatchRate + matchRate.ColorMatchScore}\n";
                matchRateChart += $"{matchRate.UID}\t{matchRate.ImageProperties.Red}\t{matchRate.ImageProperties.Green}\t{matchRate.ImageProperties.Blue}\n";
                // Debug.Log($"{matchRate.UID}, {matchRate.SourceType.ToString()} :: {matchRate.MatchRate}");
            }
            Debug.Log(matchRateChart);

            // foreach (AnnotateImageResultData data in MunchDatabase.instance.Samples.Values)
            // {
            //     string scoreChartSubject =  $"{data.UID}\n\n";
            //     string matchScoreChart = scoreChartSubject + data.GetMatchScoreList(mImageApiResultData.LabelAnnotations);

            //     Debug.Log(matchScoreChart);
            // }

            return matchRates;
        }

        private int CompareMatchRate(ImageRate imageRate1, ImageRate imageRate2)
        {
            return -imageRate1.TotalMatchRate.CompareTo(imageRate2.TotalMatchRate);
        }
    }

}
