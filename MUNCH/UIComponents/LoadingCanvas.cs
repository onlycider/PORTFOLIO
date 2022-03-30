using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingCanvas : MonoBehaviour
{
    private static LoadingCanvas mInstance;

    public static LoadingCanvas Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = Instantiate(Resources.Load<LoadingCanvas>("Objects/LoadingCanvas"));
            }
            return mInstance;
        }
    }

    public void On()
    {
        gameObject.SetActive(true);
    }

    public void Off()
    {
        gameObject.SetActive(false);
    }
}
