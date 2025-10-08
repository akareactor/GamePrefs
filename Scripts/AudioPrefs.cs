using UnityEngine;

namespace KulibinSpace.GamePrefs {

    public static class AudioPrefs {
        [Pref("masterVolume", 1f)] public static float masterVolume;
        [Pref("effectsVolume", 0.7f)] public static float effectsVolume;
        [Pref("musicVolume", 0.7f)] public static float musicVolume;
        [Pref("audioOn", true)] public static bool audioOn;
    }

}
