using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;

namespace Social
{

    /// <summary>
    /// UnityEngine.PlayerPrefs wrapper for WebGL LocalStorage
    /// </summary>
    public static class PlayerPrefs
    {

        public static void DeleteKey(string key)
        {
#if UNITY_EDITOR
            UnityEngine.PlayerPrefs.DeleteKey(key: key);
#else            
#if UNITY_WEBGL
            if (LocalStorageIsActive()==1) {
                RemoveFromLocalStorage(key: key);
            }
            else
            {
                UnityEngine.PlayerPrefs.DeleteKey(key: key);
            }
#else
            UnityEngine.PlayerPrefs.DeleteKey(key: key);
#endif
#endif
        }

        public static bool HasKey(string key)
        {
#if UNITY_EDITOR
    return (UnityEngine.PlayerPrefs.HasKey(key: key));
#else

#if UNITY_WEBGL
            if (LocalStorageIsActive() == 1)
            {
                return (HasKeyInLocalStorage(key) == 1);
            }
            else
            {
                return (UnityEngine.PlayerPrefs.HasKey(key: key));
            }
#else
        return (UnityEngine.PlayerPrefs.HasKey(key: key));
#endif
#endif

        }

        public static string GetString(string key,string defaultValue="")
        {
#if UNITY_EDITOR
return (UnityEngine.PlayerPrefs.GetString(key: key,defaultValue:defaultValue));
#else

#if UNITY_WEBGL
            if (LocalStorageIsActive() == 1)
            {
                if (HasKey(key))
                {
                    return LoadFromLocalStorage(key: key);
                }
                else
                {
                    return defaultValue;
                }
            }
            else
            {
                return (UnityEngine.PlayerPrefs.GetString(key: key, defaultValue: defaultValue));
            }
#else
        return (UnityEngine.PlayerPrefs.GetString(key: key,defaultValue:defaultValue));
#endif
#endif
        }

        public static int GetInt(string key, int defaultValue = 0)
        {
#if UNITY_EDITOR
return (UnityEngine.PlayerPrefs.GetInt(key: key,defaultValue:defaultValue));
#else
#if UNITY_WEBGL
            if (LocalStorageIsActive() == 1)
            {
                if (HasKey(key))
                {
                    return int.Parse(LoadFromLocalStorage(key: key));
                }
                else
                {
                    return defaultValue;
                }
            }
            else
            {
                return (UnityEngine.PlayerPrefs.GetInt(key: key, defaultValue: defaultValue));
            }
#else
                return (UnityEngine.PlayerPrefs.GetInt(key: key,defaultValue:defaultValue));
#endif
#endif
        }
        public static float GetFloat(string key, float defaultValue = 0f)
        {
#if UNITY_EDITOR
return (UnityEngine.PlayerPrefs.GetFloat(key: key,defaultValue:defaultValue));
#else
#if UNITY_WEBGL
            if (LocalStorageIsActive() == 1)
            {
                if (HasKey(key))
                {
                    return float.Parse(LoadFromLocalStorage(key: key));
                }
                else
                {
                    return defaultValue;
                }
            }
            else
            {
                return (UnityEngine.PlayerPrefs.GetFloat(key: key, defaultValue: defaultValue));
            }
#else
                return (UnityEngine.PlayerPrefs.GetFloat(key: key,defaultValue:defaultValue));
#endif
#endif
        }

        public static void SetString(string key, string value)
        {
#if UNITY_EDITOR
UnityEngine.PlayerPrefs.SetString(key: key, value: value);
#else
#if UNITY_WEBGL
            if (LocalStorageIsActive() == 1)
            {
                SaveToLocalStorage(key: key, value: value);
            }
            else
            {
                UnityEngine.PlayerPrefs.SetString(key: key, value: value);
            }
#else
        UnityEngine.PlayerPrefs.SetString(key: key, value: value);
#endif
#endif
        }


        public static void SetInt(string key, int value)
        {
#if UNITY_EDITOR
UnityEngine.PlayerPrefs.SetInt(key: key, value: value);
#else
#if UNITY_WEBGL
            if (LocalStorageIsActive() == 1)
            {
                SaveToLocalStorage(key: key, value: value.ToString());
            }
            else
            {
                UnityEngine.PlayerPrefs.SetInt(key: key, value: value);
            }
#else
        UnityEngine.PlayerPrefs.SetInt(key: key, value: value);
#endif
#endif

        }
        public static void SetFloat(string key, float value)
        {
#if UNITY_EDITOR
UnityEngine.PlayerPrefs.SetFloat(key: key, value: value);
#else
#if UNITY_WEBGL
            if (LocalStorageIsActive() == 1)
            {
                SaveToLocalStorage(key: key, value: value.ToString());
            }
            else
            {
                UnityEngine.PlayerPrefs.SetFloat(key: key, value: value);
            }
#else
        UnityEngine.PlayerPrefs.SetFloat(key: key, value: value);
#endif
#endif
        }

        public static void Save()
        {
#if !UNITY_WEBGL
        UnityEngine.PlayerPrefs.Save();
#endif
        }

        public static void DeleteAll()
        {
#if UNITY_EDITOR
UnityEngine.PlayerPrefs.DeleteAll();
#else
#if UNITY_WEBGL
            if (LocalStorageIsActive() == 1)
            {
                ClearLocalStorage();
            }
            else
            {
                UnityEngine.PlayerPrefs.DeleteAll();
            }
#else
        UnityEngine.PlayerPrefs.DeleteAll();
#endif
#endif
        }


#if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void SaveToLocalStorage(string key, string value);

        [DllImport("__Internal")]
        private static extern string LoadFromLocalStorage(string key);

        [DllImport("__Internal")]
        private static extern void RemoveFromLocalStorage(string key);

        [DllImport("__Internal")]
        private static extern void ClearLocalStorage();

        [DllImport("__Internal")]
        private static extern int HasKeyInLocalStorage(string key);

        [DllImport("__Internal")]
        private static extern int LocalStorageIsActive();

#endif
    }
}
