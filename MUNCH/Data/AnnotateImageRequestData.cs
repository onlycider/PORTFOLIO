using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
Vision api call json Data Format

{
  "requests":[
    {
      "image":{
        "content":"/9j/7QBEUGhvdG9...image contents...eYxxxzj/Coa6Bax//Z"
      },
      "features":[
        {
          "type":"LABEL_DETECTION",
          "maxResults":1
        }
      ]
    }
  ]
}
*/

namespace CloudVisionApiData
{
  [System.Serializable]
  public class AnnotateImageRequests
  {
      public List<AnnotateImageRequestData> requests;

      public AnnotateImageRequests()
      {
          requests = new List<AnnotateImageRequestData>();
      }

      public void AddRequest(AnnotateImageRequestData requestData)
      {
          requests.Add(requestData);
      }
  }

  /// <summary>
  /// 
  /// </summary>
  [System.Serializable]
  public class AnnotateImageRequestData
  {
      public AnnotateImage image;
      public List<Feature> features;

      public AnnotateImageRequestData(string encodedImage)
      {
          image = new AnnotateImage(encodedImage);
          features = new List<Feature>();
      }

      public void AddFeatures(ApiType apiType, int resultCount)
      {
          features.Add(new Feature(apiType, resultCount));
      }
  }

  /// <summary>
  /// content 이미지 파일 바이트 배열을 base64로 인코딩 한 결과
  /// </summary>
  [System.Serializable]
  public class AnnotateImage
  {
      public string content;

      public AnnotateImage(string encodedImage)
      {
          content = encodedImage;
      }
  }

  /// <summary>
  /// 타입에 들어갈수 있는 vision api 타입
  /// 1. OCR
  ///     DOCUMENT_TEXT_DETECTION - 이미지의 텍스트 감지
  /// 2. CROP_HINTS - 자르기 힌트 감지
  /// 3. FACE_DETECTION - 얼굴 감지
  /// 4. IMAGE_PROPERTIES - 이미지 속성 감지
  /// 5. LABEL_DETECTION - 일반 객체, 위치, 활동, 동물 종, 제품 등을 식별
  /// 6. LANDMARK_DETECTION - 명소 감지는 이미지에서 유명한 자연 경관과 인공 구조물을 감지
  /// 7. LOGO_DETECTION - 로고 감지는 이미지에서 인기 제품 로고를 감지
  /// 8. OBJECT_LOCALIZATION - 여러 객체 감지
  /// 9. WEB_DETECTION - 웹참조 감지
  /// </summary>
  [System.Serializable]
  public class Feature
  {
      public string type;
      public int maxResults;

      public Feature(ApiType apiType, int resultCount)
      {
          type = apiType.ToString();
          maxResults = resultCount;
      }
  }

  public enum ApiType
  {
      TYPE_UNSPECIFIED = 0,
      LOGO_DETECTION,
      LABEL_DETECTION,
      TEXT_DETECTION,
      IMAGE_PROPERTIES,
      OBJECT_LOCALIZATION
  }

}
