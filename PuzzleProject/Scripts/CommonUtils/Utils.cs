using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

namespace IKPuzzle
{
    public class Utils {

        public static Array GetEnumValues<T>()
        {
            Array arr = Enum.GetValues(typeof(T));
            return arr;
        }

        /// <summary>
        /// json 데이터 yn 값으로 넘어오는 경우
        /// </summary>
        /// <param name="_value"></param>
        /// <returns></returns>
        public static bool WebBoolValueConverter(string _value)
        {
            bool value = false;
            if(_value.Equals("Y"))
                value = true;

            return value;
        }
        
        public static bool WebBoolValueConverter(int _value)
        {
            bool value = false;
            if(_value > 0)
                value = true;

            return value;
        }

        public static string ConvertIntToStringBool(int _value)
        {
            string value = "N";
            if(_value > 0)
                value = "Y";

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_value"></param>
        /// <returns></returns>
        public static byte ConvertIntToByte(int _value){

            // byte value = Convert.ToByte(_value);
            // try {
            // 	value = Convert.ToByte(_value);
            // }                     
            // catch (OverflowException) {
            // 	Debug.Log("Value is overflow to byte ");
            // }

            // return value;
            return Convert.ToByte(_value);
        }

        public static T ConvertStringToEnumData<T>( string _value )
        {
            return (T)Enum.Parse( typeof( T ), _value );
        }

        public static T ConvertIntToEnumData<T>(int _value)
        {
            return (T)Enum.ToObject(typeof(T), _value);
        }

        public static int ConvertCharToInt(char _value)
        {
            return Convert.ToInt32(_value);
        }

        public static T ConvertCharToEnumData<T>(char _value)
        {
            return (T)Enum.ToObject(typeof(T), _value);
        }

        public static char ConvertStringToChar(string _value)
        {
            return _value[0];
        }

        public static void InvokeAction(System.Action action)
        {
            if(action != null && action.GetInvocationList().Length > 0)
            {
                action();
            }
        }

        public static void InvokeAction(System.Action<object[]> action, params object[] parameters)
        {
            if (action != null && action.GetInvocationList().Length > 0)
            {
                action(parameters);
            }
        }

        public static void InvokeAction<T>(System.Action<T> action, T parameter)
        {
            if(action != null && action.GetInvocationList().Length > 0)
            {
                action(parameter);
            }
        }

        public static T GetCreatedObjectComponent<T>(UnityEngine.GameObject obj, UnityEngine.Transform parent = null) where T : UnityEngine.MonoBehaviour
        {
            UnityEngine.GameObject go = UnityEngine.MonoBehaviour.Instantiate<UnityEngine.GameObject>(obj.gameObject);
            
            if(parent != null)
                go.transform.SetParent(parent);

            go.transform.localPosition = UnityEngine.Vector3.zero;
            go.transform.localScale = UnityEngine.Vector3.one;

            T slotComponent = go.GetComponent<T>();
            return slotComponent;
        }

        public static string GetAppendedSentence(params string[] words)
        {
            if (words == null)
                return string.Empty;
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < words.Length; i++)
            {
                stringBuilder.Append(words[i]);
            }

            return stringBuilder.ToString();
        }
    }
}

