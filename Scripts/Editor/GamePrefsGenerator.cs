#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization;
using UnityEditor;
using UnityEngine;

namespace KulibinSpace.GamePrefs {

    public static class GamePrefsGenerator {

        [MenuItem("Tools/Kulibin.Space/Rebuild GamePrefs")]
        public static void Generate () {

            var prefs = GamePrefsRegistry.CollectAllPrefs().OrderBy(p => p.key).ToList();

            if (!prefs.Any()) {
                Debug.LogWarning("GamePrefsGenerator: Не найдено ни одного поля с [Pref]!");
                return;
            }

            foreach (var p in prefs) {
                if (p.defaultValue == null || p.defaultValue.GetType() != p.type) {
                    Debug.LogError($"GamePrefsGenerator: Неверное значение по умолчанию для '{p.fieldName}'. Поле имеет тип {p.type.Name}, а defaultValue имеет тип {(p.defaultValue == null ? "null" : p.defaultValue.GetType().Name)}.");
                    return;
                }
            }

            var duplicateKeys = prefs.GroupBy(p => p.key).Where(g => g.Count() > 1).ToList();
            if (duplicateKeys.Count > 0) {
                foreach (var g in duplicateKeys)
                    Debug.LogError($"GamePrefsGenerator: Дублирующийся ключ Pref '{g.Key}'.");
                return;
            }

            var duplicateNames = prefs.GroupBy(p => p.fieldName).Where(g => g.Count() > 1).ToList();
            if (duplicateNames.Count > 0) {
                foreach (var g in duplicateNames)
                    Debug.LogError($"GamePrefsGenerator: Дублирующееся имя поля '{g.Key}'.");
                return;
            }

            Debug.Log($"GamePrefsGenerator: найдено {prefs.Count} полей с [Pref].");

            var sb = new StringBuilder();
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine();
            sb.AppendLine("namespace KulibinSpace.GamePrefs { public static class GamePrefsCompiled {");

            var initLines = new StringBuilder();

            foreach (var p in prefs) {

                string key = p.key;
                string name = p.fieldName;
                string def = FormatDefaultValue(p.defaultValue);

                if (p.type == typeof(float)) {
                    sb.AppendLine($"    public static float {name} {{ get => Prefs.GetFloat(\"{key}\"); set => Prefs.SetFloat(\"{key}\", value); }}");
                    initLines.AppendLine($"        if (!Prefs.HasKey(\"{key}\")) Prefs.SetFloat(\"{key}\", {def}f);");
                }

                else if (p.type == typeof(int)) {
                    sb.AppendLine($"    public static int {name} {{ get => Prefs.GetInt(\"{key}\"); set => Prefs.SetInt(\"{key}\", value); }}");
                    initLines.AppendLine($"        if (!Prefs.HasKey(\"{key}\")) Prefs.SetInt(\"{key}\", {def});");
                }

                else if (p.type == typeof(bool)) {
                    int defBool = (bool)p.defaultValue ? 1 : 0;
                    sb.AppendLine($"    public static bool {name} {{ get => Prefs.GetInt(\"{key}\") == 1; set => Prefs.SetInt(\"{key}\", value ? 1 : 0); }}");
                    initLines.AppendLine($"        if (!Prefs.HasKey(\"{key}\")) Prefs.SetInt(\"{key}\", {defBool});");
                }

                else if (p.type == typeof(string)) {
                    sb.AppendLine($"    public static string {name} {{ get => Prefs.GetString(\"{key}\"); set => Prefs.SetString(\"{key}\", value); }}");
                    initLines.AppendLine($"        if (!Prefs.HasKey(\"{key}\")) Prefs.SetString(\"{key}\", \"{def}\");");
                }

                else {
                    Debug.LogError($"GamePrefsGenerator: Тип {p.type.Name} не поддерживается.");
                    return;
                }
            }

            sb.AppendLine();
            sb.AppendLine("    public static void LoadSettings() {");
            sb.Append(initLines);
            sb.AppendLine("        Prefs.Save();");
            sb.AppendLine("    }");

            sb.AppendLine();
            sb.AppendLine("    public static void SaveSettings() {");
            sb.AppendLine("        Prefs.Save();");
            sb.AppendLine("    }");

            sb.AppendLine("}}");

            string path = "Assets/Generated/GamePrefsCompiled.cs";
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
            AssetDatabase.Refresh();

            Debug.Log($"GamePrefsGenerator: Сгенерирован {path}");
        }

        static string FormatDefaultValue (object value) {
            if (value is float f) return f.ToString(CultureInfo.InvariantCulture);
            if (value is int i) return i.ToString();
            if (value is bool b) return b ? "true" : "false";
            if (value is string s) return s.Replace("\"", "\\\"");
            return "";
        }
    }
}
#endif
