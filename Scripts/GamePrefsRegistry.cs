using System;
using System.Collections.Generic;
using System.Reflection;

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

            public string declaringType;     // Namespace.Type
            public string assemblyName;      // Assembly-CSharp
            public string assetPath;         // Assets/...
        }

        public static IEnumerable<PrefInfo> CollectAllPrefs() {

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies) {

                // Ограничим поиск только игровыми сборками
                if (!assembly.FullName.StartsWith("Assembly-CSharp"))
                    continue;

                Type[] types;

                try {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException e) {
                    types = e.Types;
                }

                foreach (var type in types) {
                    if (type == null) continue;

                    var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);

                    foreach (var field in fields) {

                        var attr = field.GetCustomAttribute<PrefAttribute>();
                        if (attr == null) continue;

#if UNITY_EDITOR
                        string assetPath = GetScriptPath(type);
#else
                        string assetPath = "Runtime";
#endif

                        yield return new PrefInfo {
                            key = attr.key,
                            fieldName = field.Name,
                            type = field.FieldType,
                            defaultValue = attr.defaultValue,
                            declaringType = type.FullName,
                            assemblyName = assembly.GetName().Name,
                            assetPath = assetPath
                        };
                    }
                }
            }
        }

#if UNITY_EDITOR
        static string GetScriptPath(Type type) {

            // Ищем MonoScript по имени класса
            var guids = AssetDatabase.FindAssets($"t:MonoScript {type.Name}");

            foreach (var guid in guids) {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);

                if (script != null && script.GetClass() == type)
                    return path;
            }

            return "Unknown path";
        }
#endif
    }
}
