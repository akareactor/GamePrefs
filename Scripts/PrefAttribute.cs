using System;
using UnityEngine;

namespace KulibinSpace.GamePrefs {

    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class PrefAttribute : System.Attribute {
        public string key;
        public object defaultValue;
        public PrefAttribute (string key, object defaultValue) { this.key = key; this.defaultValue = defaultValue; }
    }
}
