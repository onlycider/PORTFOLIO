using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace MunchProject
{
    public static class Utils
    {

        public static void InvokeAction(System.Action action)
        {
            if (action != null && action.GetInvocationList().Length > 0)
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
            if (action != null && action.GetInvocationList().Length > 0)
            {
                action(parameter);
            }
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
