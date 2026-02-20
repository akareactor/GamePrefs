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

        // ===== НАСТРОЙКИ ГЕНЕРАЦИИ =====
        const string GeneratedNamespace = "KulibinSpace.GamePrefs";
        const string GeneratedClassName = "GamePrefsCompiled";
        const string GeneratedFolder = "Assets/Generated";

        [MenuItem("Tools/Kulibin.Space/Rebuild GamePrefs")]
        public static void Generate() {

            var prefs = GamePrefsRegistry
                .CollectAllPrefs()
                .ToList();

            if (prefs.Count == 0) {
                Debug.LogWarning("⚠️ GamePrefsGenerator: Не найдено ни одного поля с [Pref]!");
                return;
            }

            Debug.Log($"GamePrefsGenerator: найдено {prefs.Count} полей с [Pref]!");

            var sb = new StringBuilder();
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine();
            sb.AppendLine($"namespace {GeneratedNamespace} {{");
            sb.AppendLine($"    public static class {GeneratedClassName} {{");

            var saveCalls = new StringBuilder();
            var loadLines = new StringBuilder();

            foreach (var p in prefs) {

                string key = p.key;
                string name = p.fieldName;
                string def = FormatDefaultValue(p.defaultValue);

                if (p.type == typeof(float)) {

                    sb.AppendLine(
                        $"        public static float {name} {{ get {{ if (!Prefs.HasKey(\"{key}\")) Prefs.SetFloat(\"{key}\", {def}f); return Prefs.GetFloat(\"{key}\"); }} set => Prefs.SetFloat(\"{key}\", value); }}");

                    saveCalls.AppendLine($"            Prefs.SetFloat(\"{key}\", {name});");
                    loadLines.AppendLine($"            {name} = Prefs.HasKey(\"{key}\") ? Prefs.GetFloat(\"{key}\") : {def}f;");
                }
                else if (p.type == typeof(int)) {

                    sb.AppendLine(
                        $"        public static int {name} {{ get {{ if (!Prefs.HasKey(\"{key}\")) Prefs.SetInt(\"{key}\", {def}); return Prefs.GetInt(\"{key}\"); }} set => Prefs.SetInt(\"{key}\", value); }}");

                    saveCalls.AppendLine($"            Prefs.SetInt(\"{key}\", {name});");
                    loadLines.AppendLine($"            {name} = Prefs.HasKey(\"{key}\") ? Prefs.GetInt(\"{key}\") : {def};");
                }
                else if (p.type == typeof(bool)) {

                    string defBool = (bool)p.defaultValue ? "1" : "0";

                    sb.AppendLine(
                        $"        public static bool {name} {{ get {{ if (!Prefs.HasKey(\"{key}\")) Prefs.SetInt(\"{key}\", {defBool}); return Prefs.GetInt(\"{key}\") == 1; }} set => Prefs.SetInt(\"{key}\", value ? 1 : 0); }}");

                    saveCalls.AppendLine($"            Prefs.SetInt(\"{key}\", {name} ? 1 : 0);");
                    loadLines.AppendLine($"            {name} = Prefs.HasKey(\"{key}\") ? Prefs.GetInt(\"{key}\") == 1 : {(defBool == "1" ? "true" : "false")};");
                }
                else if (p.type == typeof(string)) {

                    sb.AppendLine(
                        $"        public static string {name} {{ get {{ if (!Prefs.HasKey(\"{key}\")) Prefs.SetString(\"{key}\", \"{def}\"); return Prefs.GetString(\"{key}\"); }} set => Prefs.SetString(\"{key}\", value); }}");

                    saveCalls.AppendLine($"            Prefs.SetString(\"{key}\", {name});");
                    loadLines.AppendLine($"            {name} = Prefs.HasKey(\"{key}\") ? Prefs.GetString(\"{key}\") : \"{def}\";");
                }
            }

            sb.AppendLine();
            sb.AppendLine("        public static void LoadSettings() {");
            sb.Append(loadLines);
            sb.AppendLine("        }");

            sb.AppendLine();
            sb.AppendLine("        public static void SaveSettings() {");
            sb.Append(saveCalls);
            sb.AppendLine("            Prefs.Save();");
            sb.AppendLine("        }");

            sb.AppendLine("    }");
            sb.AppendLine("}");

            Directory.CreateDirectory(GeneratedFolder);

            string filePath = Path.Combine(GeneratedFolder, GeneratedClassName + ".cs");

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);

            AssetDatabase.Refresh();

            Debug.Log($"✅ {GeneratedClassName}.cs regenerated at {filePath}");
        }

        static string FormatDefaultValue(object value) {

            if (value is float f)
                return f.ToString(CultureInfo.InvariantCulture);

            if (value is int i)
                return i.ToString();

            if (value is bool b)
                return b ? "true" : "false";

            if (value is string s)
                return s.Replace("\"", "\\\"");

            return "0";
        }
    }
}
#endif
