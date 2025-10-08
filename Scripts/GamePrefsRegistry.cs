using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace KulibinSpace.GamePrefs {

    public static class GamePrefsRegistry {

        public struct PrefInfo {
            public string key;
            public string fieldName;
            public Type type;
            public object defaultValue;
        }

        public static IEnumerable<PrefInfo> CollectAllPrefs () {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach (var type in assembly.GetTypes()) {
                    var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
                    foreach (var field in fields) {
                        var attr = field.GetCustomAttribute<PrefAttribute>();
                        if (attr == null) continue;

                        yield return new PrefInfo {
                            key = attr.key,
                            fieldName = field.Name,
                            type = field.FieldType,
                            defaultValue = attr.defaultValue
                        };
                    }
                }
            }
        }
    }

}
