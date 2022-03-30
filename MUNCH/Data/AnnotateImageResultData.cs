using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MunchProject
{
    public enum SourceType
    {
        Sample,
        User,
    }

    public class ImageRate
    {
        private string mUID;
        public string UID { get { return mUID; } }

        private float mMatchRate;
        public float MatchRate { get { return mMatchRate; } }

        private float mObjectMatchRate;
        public float ObjectMatchRate { get { return mObjectMatchRate; } }

        private float mColorMatchScore = 0f;
        public float ColorMatchScore { get { return mColorMatchScore; } }

        public float TotalMatchRate { get { return mObjectMatchRate + mMatchRate + mColorMatchScore; } }

        private SourceType mSourceType;
        public SourceType SourceType { get { return mSourceType; } }

        private ImagePropertiesAnnotation mImageProperties;
        public ImagePropertiesAnnotation ImageProperties { get { return mImageProperties; } }

        public ImageRate(AnnotateImageResultData data, float matchRate, float objMatchRate, float colorScore)
        {
            mUID = data.UID;
            mSourceType = data.SourceType;
            mMatchRate = matchRate;
            mObjectMatchRate = objMatchRate;
            mColorMatchScore = colorScore;
        }

        public ImageRate(AnnotateImageResultData data, float matchRate, float objMatchRate, ImagePropertiesAnnotation imageProperties)
        {
            mUID = data.UID;
            mSourceType = data.SourceType;
            mMatchRate = matchRate;
            mObjectMatchRate = objMatchRate;
            mImageProperties = imageProperties;
        }
    }


    public class AnnotateImageResultData
    {
        private string mUID;
        public string UID { get { return mUID; } }

        private Dictionary<string, LabelAnnotation> mLabelAnnotations;
        public Dictionary<string, LabelAnnotation> LabelAnnotations { get { return mLabelAnnotations; } }
        // private Dictionary<string, LocalizedObjectAnnotation> mObjectAnnotations;
        private List<LocalizedObjectAnnotation> mObjectAnnotations;
        public List<LocalizedObjectAnnotation> ObjectAnnotations { get { return mObjectAnnotations; } }

        private ImagePropertiesAnnotation mPropertiesAnnotation;
        public ImagePropertiesAnnotation PropertiesAnnotaion { get { return mPropertiesAnnotation; } }

        private SourceType mSourceType;
        public SourceType SourceType { get { return mSourceType; } }

        public AnnotateImageResultData(Dictionary<string, object> data, string uid)
        {
            mUID = uid;
            mLabelAnnotations = new Dictionary<string, LabelAnnotation>();
            mObjectAnnotations = new List<LocalizedObjectAnnotation>();
            mPropertiesAnnotation = new ImagePropertiesAnnotation();

            if (data.ContainsKey("labelAnnotations"))
            {
                Dictionary<string, object>[] objects = data["labelAnnotations"] as Dictionary<string, object>[];
                foreach (Dictionary<string, object> objectData in objects)
                {
                    LabelAnnotation labelAnnotation = new LabelAnnotation(objectData);
                    if (!mLabelAnnotations.ContainsKey(labelAnnotation.Description))
                    {
                        mLabelAnnotations.Add(labelAnnotation.Description, labelAnnotation);
                    }
                }
            }

            if (data.ContainsKey("localizedObjectAnnotations"))
            {
                Dictionary<string, object>[] objects = data["localizedObjectAnnotations"] as Dictionary<string, object>[];
                foreach (Dictionary<string, object> objectData in objects)
                {
                    LocalizedObjectAnnotation objectAnnotation = new LocalizedObjectAnnotation(objectData);
                    mObjectAnnotations.Add(objectAnnotation);
                }
            }

            if (data.ContainsKey("imagePropertiesAnnotation"))
            {
                Dictionary<string, object> propertiesAnnotation = data["imagePropertiesAnnotation"] as Dictionary<string, object>;

                if (!propertiesAnnotation.ContainsKey("dominantColors"))
                {
                    return;
                }

                Dictionary<string, object> dominantColors = propertiesAnnotation["dominantColors"] as Dictionary<string, object>;

                if (!dominantColors.ContainsKey("colors"))
                {
                    return;
                }

                Dictionary<string, object>[] colors = dominantColors["colors"] as Dictionary<string, object>[];

                if (colors == null || colors.Length < 1)
                {
                    return;
                }

                Dictionary<string, object> color = colors[0];

                if (!color.ContainsKey("color"))
                {
                    return;
                }

                Dictionary<string, object> rgb = color["color"] as Dictionary<string, object>;

                int red = (int)rgb["red"];
                // red /= 32;
                int green = (int)rgb["green"];
                // green /= 32;
                int blue = (int)rgb["blue"];
                // blue /= 32;

                mPropertiesAnnotation.SetColorRGB(red, green, blue);
            }
        }

        public void SetSourceType(SourceType sourceType)
        {
            mSourceType = sourceType;
        }

        public float GetObjectMatchRate(List<LocalizedObjectAnnotation> objects)
        {
            if (objects == null || objects.Count <= 0)
            {
                return 0;
            }

            if (mObjectAnnotations == null || mObjectAnnotations.Count <= 0)
            {
                return 0;
            }

            LocalizedObjectAnnotation compareObject = objects[0];
            LocalizedObjectAnnotation targetObject = mObjectAnnotations[0];

            if (compareObject.Name != targetObject.Name)
            {
                return 0;
            }

            float denominator = compareObject.Score >= targetObject.Score ? (float)compareObject.Score : (float)targetObject.Score;
            float numerator = compareObject.Score >= targetObject.Score ? (float)targetObject.Score : (float)compareObject.Score;

            return numerator / denominator * 0.5f;
        }

        public float GetMatchRate(Dictionary<string, LabelAnnotation> compareLabels)
        {
            if (compareLabels == null || compareLabels.Count <= 0)
            {
                return 0;
            }

            if (mLabelAnnotations == null || mLabelAnnotations.Count <= 0)
            {
                return 0;
            }

            int containCount = 0;
            foreach (string key in compareLabels.Keys)
            {
                if (mLabelAnnotations.ContainsKey(key))
                {
                    ++containCount;
                }
            }
            float denominator = compareLabels.Count >= mLabelAnnotations.Count ? (float)compareLabels.Count : (float)mLabelAnnotations.Count;

            return containCount / denominator;
        }

        public float GetColorScore(ImagePropertiesAnnotation annotation)
        {

            return mPropertiesAnnotation.CompareColor(annotation);
        }

        public string GetMatchScoreList(Dictionary<string, LabelAnnotation> compareLabels)
        {
            string matchScoreChart = string.Empty;
            int containCount = 0;
            foreach (string key in compareLabels.Keys)
            {
                if (mLabelAnnotations.ContainsKey(key))
                {
                    ++containCount;
                    matchScoreChart += $"{key} - score\t{compareLabels[key].Score}\t{mLabelAnnotations[key].Score}\n";
                }
            }

            return $"\t{compareLabels.Count}개 중 {containCount}\t{mLabelAnnotations.Count}개 중{containCount}\n" + matchScoreChart;
        }
    }

    public class LabelAnnotation
    {
        private string mDescription;
        public string Description { get { return mDescription; } }

        private double mScore;
        public double Score { get { return mScore; } }
        public LabelAnnotation(Dictionary<string, object> data)
        {
            mDescription = (string)data["description"];
            mScore = (double)data["score"];
        }
    }

    public class LocalizedObjectAnnotation
    {
        private string mName;
        public string Name { get { return mName; } }

        private double mScore;
        public double Score { get { return mScore; } }
        public LocalizedObjectAnnotation(Dictionary<string, object> data)
        {
            mName = (string)data["name"];
            mScore = (double)data["score"];
        }
    }

    public class ImagePropertiesAnnotation
    {
        private int mRed;
        public int Red { get { return mRed; } }
        private int mGreen;
        public int Green { get { return mGreen; } }
        private int mBlue;
        public int Blue { get { return mBlue; } }

        public ImagePropertiesAnnotation()
        {
            mRed = -1;
            mGreen = -1;
            mBlue = -1;
        }

        public void SetColorRGB(int red, int green, int blue)
        {
            mRed = red;
            mGreen = green;
            mBlue = blue;

            // Debug.Log($"Setting Color RGB :: {red}, {green}, {blue}");
        }

        public float CompareColor(ImagePropertiesAnnotation annotation)
        {
            // Debug.Log($"{mRed}\t{mGreen}\t{mBlue}");
            if (mRed == annotation.Red && mGreen == annotation.Green && mBlue == annotation.Blue)
            {
                return 0.1f;
            }

            return 0f;
        }
    }
}

