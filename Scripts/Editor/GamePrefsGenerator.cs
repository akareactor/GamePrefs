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
        public static void Generate() {

            var prefs = GamePrefsRegistry.CollectAllPrefs().ToList();

            if (!prefs.Any()) {
                Debug.LogWarning("⚠️ GamePrefsGenerator: Не найдено ни одного поля с [Pref]!");
                return;
            }

            Debug.Log($"GamePrefsGenerator: найдено {prefs.Count} полей с [Pref]!");

            // ===== Вывод подробной информации =====
            foreach (var p in prefs) {
                Debug.Log(
                    $"Pref найден:\n" +
                    $"  Key: {p.key}\n" +
                    $"  Field: {p.declaringType}.{p.fieldName}\n" +
                    $"  Type: {p.type.Name}\n" +
                    $"  Assembly: {p.assemblyName}\n" +
                    $"  File: {p.assetPath}\n"
                );
            }

            // ===== Проверка дубликатов ключей =====
            var duplicates = prefs
                .GroupBy(p => p.key)
                .Where(g => g.Count() > 1);

            foreach (var group in duplicates) {

                var msg = new StringBuilder();
                msg.AppendLine($"❌ Дубликат ключа '{group.Key}' найден в:");

                foreach (var p in group) {
                    msg.AppendLine($"   - {p.declaringType}.{p.fieldName} ({p.assetPath})");
                }

                Debug.LogError(msg.ToString());
            }

            // ===== Генерация файла =====
            var sb = new StringBuilder();
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine();
            sb.AppendLine("namespace KulibinSpace.GamePrefs {");
            sb.AppendLine("public static class GamePrefs {");

            var saveCalls = new StringBuilder();
            var loadLines = new StringBuilder();

            foreach (var p in prefs) {

                string key = p.key;
                string name = p.fieldName;
                string def = FormatDefaultValue(p.defaultValue);

                if (p.type == typeof(float)) {
                    sb.AppendLine(
                        $"    public static float {name} {{ get {{ if (!Prefs.HasKey(\"{key}\")) Prefs.SetFloat(\"{key}\", {def}f); return Prefs.GetFloat(\"{key}\"); }} set => Prefs.SetFloat(\"{key}\", value); }}");
                    saveCalls.AppendLine($"        Prefs.SetFloat(\"{key}\", {name});");
                    loadLines.AppendLine($"        {name} = Prefs.HasKey(\"{key}\") ? Prefs.GetFloat(\"{key}\") : {def}f;");
                }
                else if (p.type == typeof(int)) {
                    sb.AppendLine(
                        $"    public static int {name} {{ get {{ if (!Prefs.HasKey(\"{key}\")) Prefs.SetInt(\"{key}\", {def}); return Prefs.GetInt(\"{key}\"); }} set => Prefs.SetInt(\"{key}\", value); }}");
                    saveCalls.AppendLine($"        Prefs.SetInt(\"{key}\", {name});");
                    loadLines.AppendLine($"        {name} = Prefs.HasKey(\"{key}\") ? Prefs.GetInt(\"{key}\") : {def};");
                }
                else if (p.type == typeof(bool)) {
                    string defBool = (bool)p.defaultValue ? "true" : "false";
                    sb.AppendLine(
                        $"    public static bool {name} {{ get {{ if (!Prefs.HasKey(\"{key}\")) Prefs.SetInt(\"{key}\", {defBool} ? 1 : 0); return Prefs.GetInt(\"{key}\") == 1; }} set => Prefs.SetInt(\"{key}\", value ? 1 : 0); }}");
                    saveCalls.AppendLine($"        Prefs.SetInt(\"{key}\", {name} ? 1 : 0);");
                    loadLines.AppendLine($"        {name} = Prefs.HasKey(\"{key}\") ? Prefs.GetInt(\"{key}\") == 1 : {defBool};");
                }
                else if (p.type == typeof(string)) {
                    sb.AppendLine(
                        $"    public static string {name} {{ get {{ if (!Prefs.HasKey(\"{key}\")) Prefs.SetString(\"{key}\", \"{def}\"); return Prefs.GetString(\"{key}\"); }} set => Prefs.SetString(\"{key}\", value); }}");
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
            sb.AppendLine("        Prefs.Save();");
            sb.AppendLine("    }");

            sb.AppendLine("}}");

            string path = "Assets/GamePrefs/GamePrefs.cs";
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);

            AssetDatabase.Refresh();

            Debug.Log("✅ GamePrefs.cs regenerated at " + path);
        }

        static string FormatDefaultValue(object value) {
            if (value is float f) return f.ToString(CultureInfo.InvariantCulture);
            if (value is int i) return i.ToString();
            if (value is bool b) return b ? "true" : "false";
            if (value is string s) return s.Replace("\"", "\\\"");
            return "0";
        }
    }
}
#endif
