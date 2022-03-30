using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if PLATFORM_ANDROID
using UnityEngine.Android;
#elif PLATFORM_IOS
using UnityEngine.iOS;
#endif

namespace MunchProject
{
    public class ApplicationLauncher : MonoBehaviour
    {
        void Awake()
        {
            Application.targetFrameRate = 120;
            MunchDatabase.Initialize();
            MunchDatabase.instance.LoadMunchData();
            MunchDatabase.instance.LoadResourcesSamples();
            MunchDatabase.instance.LoadRegisteredProduct();

#if UNITY_ANDROID && !UNITY_EDITOR
            ApplicationChrome.statusBarState = ApplicationChrome.States.Visible;
            ApplicationChrome.statusBarState = ApplicationChrome.States.TranslucentOverContent;
#endif
        }

        void Start()
        {
            PermissionCamera();
            // TextAsset[] textAssets = MunchDatabase.instance.GetSamples();
            // foreach(TextAsset asset in textAssets)
            // {
            //     Debug.Log(asset.name);
            // }
        }

#if PLATFORM_ANDROID
        public void PermissionCamera()
        {

            if (Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                PanelManager.Instance.AddPanel<MainCameraView>(PanelTag.MainCameraView);
            }
            else
            {
                var callbacks = new PermissionCallbacks();
                callbacks.PermissionDenied += PermissionCallbacks_PermissionDenied;
                callbacks.PermissionGranted += PermissionCallbacks_PermissionGranted;
                callbacks.PermissionDeniedAndDontAskAgain += PermissionCallbacks_PermissionDeniedAndDontAskAgain;
                Permission.RequestUserPermission(Permission.Camera, callbacks);
            }
        }

        internal void PermissionCallbacks_PermissionDeniedAndDontAskAgain(string permissionName)
        {
            Debug.Log($"{permissionName} PermissionDeniedAndDontAskAgain");
            Application.Quit();
        }

        internal void PermissionCallbacks_PermissionGranted(string permissionName)
        {
            Debug.Log($"{permissionName} PermissionCallbacks_PermissionGranted");
            StartCoroutine(DelayExecuteApp());

        }

        internal void PermissionCallbacks_PermissionDenied(string permissionName)
        {
            Debug.Log($"{permissionName} PermissionCallbacks_PermissionDenied");
            Application.Quit();
        }

        private IEnumerator DelayExecuteApp()
        {
            yield return new WaitForSeconds(0.1f);
            PanelManager.Instance.AddPanel<MainCameraView>(PanelTag.MainCameraView);
        }
#elif PLATFORM_IOS
        public void PermissionCamera()
        {
            StartCoroutine(ExecuteCamera());
        }
        IEnumerator ExecuteCamera()
        {
            foreach (WebCamDevice device in WebCamTexture.devices)
            {
                Debug.Log(device.name);
            }

            yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
            if (Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                Debug.Log("webcam found");
                PanelManager.Instance.AddPanel<MainCameraView>(PanelTag.MainCameraView);
            }
            else
            {
                Debug.Log("webcam not found");
                Application.Quit();
            }
        }
#endif
    }
}
