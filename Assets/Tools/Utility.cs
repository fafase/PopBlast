using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Net;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.Localization.Components;

namespace Tools 
{
    public static class JsonUtility 
    {
        // https://stackoverflow.com/questions/14977848/how-to-make-sure-that-string-is-valid-json-using-json-net
        public static bool IsValidJson(string strInput)
        {
            if (string.IsNullOrWhiteSpace(strInput)) { return false; }
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    Console.WriteLine(jex.Message);
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
    public static class EditorUtility 
    {
        public static T LoadScriptableObject<T>() where T : ScriptableObject
        {
            string scriptableObjectName = "Localizer";
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(Localization)} {scriptableObjectName}");
            if (guids.Length == 0)
            {
                Debug.LogWarning($"No {nameof(Localization)} found named {scriptableObjectName}");
                return null;
            }

            if (guids.Length > 1)
                Debug.LogWarning($"More than one {nameof(Localization)} found named {scriptableObjectName}, taking first one");

            return (T)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[0]), typeof(T));
        }
        public static TextAsset LoadTextAsset(string name)
        {
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(TextAsset)} {name}");
            if (guids.Length == 0)
            {
                Debug.LogWarning($"No {nameof(Localization)} found named {name}");
                return null;
            }

            if (guids.Length > 1)
                Debug.LogWarning($"More than one {nameof(Localization)} found named {name}, taking first one");

            return (TextAsset)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[0]), typeof(TextAsset));
        }
    }
    public static class MathUtility 
    {
        public static Vector3 CatmullRom(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float t)
        {
            Vector3 a = 0.5f * (2f * p2);
            Vector3 b = 0.5f * (p3 - p1);
            Vector3 c = 0.5f * (2f * p1 - 5f * p2 + 4f * p3 - p4);
            Vector3 d = 0.5f * (-p1 + 3f * p2 - 3f * p3 + p4);

            Vector3 pos = a + (b * t) + (c * t * t) + (d * t * t * t);

            return pos;
        }

        public static Vector2 CatmullRom(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, float t)
        {
            Vector2 a = 0.5f * (2f * p2);
            Vector2 b = 0.5f * (p3 - p1);
            Vector2 c = 0.5f * (2f * p1 - 5f * p2 + 4f * p3 - p4);
            Vector2 d = 0.5f * (-p1 + 3f * p2 - 3f * p3 + p4);

            Vector2 pos = a + (b * t) + (c * t * t) + (d * t * t * t);

            return pos;
        }
    }

    public static class UniTaskUtility
    {
        public static UniTask<InitializationResult> GetUniTaskCompletion(bool result, object obj)
        {
            var ucts = new UniTaskCompletionSource<InitializationResult>();
            ucts.TrySetResult(new InitializationResult(result, obj.GetType().ToString()));
            return ucts.Task;
        }
    }

    public static class HttpUtility 
    {
        public static bool CheckForInternetConnection(int timeoutMs = 10000, string url = null)
        {
            try
            {
                url ??= CultureInfo.InstalledUICulture switch
                {
                    { Name: var n } when n.StartsWith("fa") => // Iran
                        "http://www.aparat.com",
                    { Name: var n } when n.StartsWith("zh") => // China
                        "http://www.baidu.com",
                    _ =>
                        "http://www.gstatic.com/generate_204",
                };

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.KeepAlive = false;
                request.Timeout = timeoutMs;
                using (var response = (HttpWebResponse)request.GetResponse())
                    return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public static class LocalizationUtility 
    {
        public static void SetArgument(this LocalizeStringEvent lse, string key, string value) 
        {
            lse.StringReference.Add(key, new StringVariable { Value = value });
            lse.RefreshString();
        }
    }
}
