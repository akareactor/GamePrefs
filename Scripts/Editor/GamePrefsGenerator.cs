#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Globalization;
using UnityEditor;
using UnityEngine;

namespace KulibinSpace.GamePrefs {

    public static class GamePrefsGenerator {

        [MenuItem("Tools/Kulibin.Space/Rebuild GamePrefs")]
        public static void Generate () {

            var prefs = GamePrefsRegistry.CollectAllPrefs();

            // ChatGPT вхуячил поиск по всем сборкам и он перестал работать.
            //var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t => t.IsClass && !t.IsAbstract);
            //var prefs = types.SelectMany(t => t.GetFields(BindingFlags.Public | BindingFlags.Static)).Where(f => f.IsDefined(typeof(PrefAttribute), false)).Select(f => (f, attr: (PrefAttribute)Attribute.GetCustomAttribute(f, typeof(PrefAttribute))));

            if (!prefs.Any()) {
                Debug.Log("⚠️ GamePrefsGenerator: Не найдено ни одного поля с [Pref]!");
            } else {
                Debug.Log($"GamePrefsGenerator: найдено {prefs.Count()} полей с [Pref]!");
            }

            var sb = new StringBuilder();
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine();
            sb.AppendLine("namespace KulibinSpace.GamePrefs { public static class GamePrefsCompiled {");

            var saveCalls = new StringBuilder();
            var loadLines = new StringBuilder();

            foreach (var p in prefs) {
                var type = p.type;
                string key = p.key;
                string name = p.fieldName;
                string def = FormatDefaultValue(p.defaultValue);
                if (type == typeof(float)) {
                    sb.AppendLine($"    public static float {name} {{ get {{ if (!Prefs.HasKey(\"{key}\")) Prefs.SetFloat(\"{key}\", {def}f); return Prefs.GetFloat(\"{key}\"); }} set => Prefs.SetFloat(\"{key}\", value); }}");
                    saveCalls.AppendLine($"        Prefs.SetFloat(\"{key}\", {name});");
                    loadLines.AppendLine($"        {name} = Prefs.HasKey(\"{key}\") ? Prefs.GetFloat(\"{key}\") : {def}f;");
                } else if (type == typeof(int)) {
                    sb.AppendLine($"    public static int {name} {{ get {{ if (!Prefs.HasKey(\"{key}\")) Prefs.SetInt(\"{key}\", {def}); return Prefs.GetInt(\"{key}\"); }} set => Prefs.SetInt(\"{key}\", value); }}");
                    saveCalls.AppendLine($"        Prefs.SetInt(\"{key}\", {name});");
                    loadLines.AppendLine($"        {name} = Prefs.HasKey(\"{key}\") ? Prefs.GetInt(\"{key}\") : {def};");
                } else if (type == typeof(bool)) {
                    string defBool = (bool)p.defaultValue ? "true" : "false";
                    sb.AppendLine($"    public static bool {name} {{ get {{ if (!Prefs.HasKey(\"{key}\")) Prefs.SetInt(\"{key}\", {defBool} ? 1 : 0); return Prefs.GetInt(\"{key}\") == 1; }} set => Prefs.SetInt(\"{key}\", value ? 1 : 0); }}");
                    saveCalls.AppendLine($"        Prefs.SetInt(\"{key}\", {name} ? 1 : 0);");
                    loadLines.AppendLine($"        {name} = Prefs.HasKey(\"{key}\") ? Prefs.GetInt(\"{key}\") == 1 ? true : false : true;");
                } else if (type == typeof(string)) {
                    sb.AppendLine($"    public static string {name} {{ get {{ if (!Prefs.HasKey(\"{key}\")) Prefs.SetString(\"{key}\", \"{def}\"); return Prefs.GetString(\"{key}\"); }} set => Prefs.SetString(\"{key}\", value); }}");
                    saveCalls.AppendLine($"        Prefs.SetString(\"{key}\", {name});");
                    loadLines.AppendLine($"        {name} = Prefs.HasKey(\"{key}\") ? Prefs.GetString(\"{key}\") : \"{def}\";");
                }
            }

            sb.AppendLine();
            sb.AppendLine("    public static void LoadSettings() {");
            sb.Append(loadLines);
            sb.AppendLine("    }");

            sb.AppendLine();
            sb.AppendLine("    public static void SaveSettings() {");
            sb.Append(saveCalls);
            sb.AppendLine("        Prefs.Save(); }");
            sb.AppendLine("}}");

            string path = "Assets/Generated/GamePrefsCompiled.cs";
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
            AssetDatabase.Refresh();
            Debug.Log("✅ GamePrefs.cs regenerated at " + path);
        }

        static string FormatDefaultValue (object value) {
            if (value is float f) return f.ToString(CultureInfo.InvariantCulture);
            if (value is int i) return i.ToString();
            if (value is bool b) return b ? "true" : "false";
            if (value is string s) return s.Replace("\"", "\\\"");
            return "0";
        }
    }
}
#endif
