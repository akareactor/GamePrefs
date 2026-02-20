using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

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
            public string declaringType;
            public string assemblyName;
            public string assetPath;
        }

        public static IEnumerable<PrefInfo> CollectAllPrefs() {

            var targetAsm = typeof(PrefAttribute).Assembly.GetName().Name;
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies) {

                var asmName = assembly.GetName().Name;
                if (asmName.StartsWith("Unity") || asmName.StartsWith("System") || asmName.StartsWith("mscorlib") || asmName.StartsWith("netstandard") || asmName.StartsWith("nunit")) continue;
                if (asmName != targetAsm && !assembly.GetReferencedAssemblies().Any(a => a.Name == targetAsm)) continue;

                Type[] types;
                try { types = assembly.GetTypes(); }
                catch (ReflectionTypeLoadException e) { types = e.Types.Where(t => t != null).ToArray(); }

                foreach (var type in types) {
                    var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
                    foreach (var field in fields) {
                        var attr = field.GetCustomAttribute<PrefAttribute>();
                        if (attr == null) continue;
#if UNITY_EDITOR
                        string path = GetScriptPath(type);
#else
                        string path = "Runtime";
#endif
                        yield return new PrefInfo {
                            key = attr.key,
                            fieldName = field.Name,
                            type = field.FieldType,
                            defaultValue = attr.defaultValue,
                            declaringType = type.FullName,
                            assemblyName = asmName,
                            assetPath = path
                        };
                    }
                }
            }
        }

#if UNITY_EDITOR
        static string GetScriptPath(Type type) {
            var guids = AssetDatabase.FindAssets($"t:MonoScript {type.Name}");
            foreach (var guid in guids) {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                if (script != null && script.GetClass() == type) return path;
            }
            return "Unknown";
        }
#endif
    }
}
