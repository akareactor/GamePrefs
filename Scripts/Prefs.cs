
#define social

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.InputSystem.Interactions;

// просто чтобы легче было контролировать вызовы обёртки
public static class Prefs {
#if social
    public static float GetFloat (string key) { return Social.PlayerPrefs.GetFloat(key); }
    public static void SetFloat (string key, float value) { Social.PlayerPrefs.SetFloat(key, value); }
    public static int GetInt (string key) { return Social.PlayerPrefs.GetInt(key); }
    public static void SetInt (string key, int value) { Social.PlayerPrefs.SetInt(key, value); }
    public static string GetString (string key) { return Social.PlayerPrefs.GetString(key); }
    public static void SetString (string key, string value) { Social.PlayerPrefs.SetString(key, value); }
    public static bool HasKey (string key) { return Social.PlayerPrefs.HasKey(key); }
    public static void Save () { Social.PlayerPrefs.Save(); }
    public static void DeleteKey (string key) { Social.PlayerPrefs.DeleteKey(key); }
#else
	public static float GetFloat (string key) { return PlayerPrefs.GetFloat(key); }
	public static void SetFloat (string key, float value) { PlayerPrefs.SetFloat(key, value); }
	public static int GetInt (string key) { return PlayerPrefs.GetInt(key); }
	public static void SetInt (string key, int value) { PlayerPrefs.SetInt(key, value); }
	public static string GetString (string key) { return PlayerPrefs.GetString(key); }
	public static void SetString (string key, string value) { PlayerPrefs.SetString(key, value); }
	public static bool HasKey (string key) { return PlayerPrefs.HasKey(key); }
	public static void Save () { PlayerPrefs.Save(); }
	public static void DeleteKey (string key) { PlayerPrefs.DeleteKey(key); }
#endif
}
