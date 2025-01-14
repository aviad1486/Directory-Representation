using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DirectoryRepresentation
{
    public static class findingPathV2
    {
        public static string findPath(string inputPath, string baseUrl)
        {
            // Sanitize and retrieve the root class name
            var rootClassName = SanitizeName(new DirectoryInfo(inputPath).Name);
            if (string.IsNullOrEmpty(rootClassName))
            {
                rootClassName = "Root";
            }

            var classBuilder = new List<string>();
            classBuilder.Add($"public class {rootClassName}");
            classBuilder.Add("{");

            // Build properties for all files and folders in a flat structure
            BuildProperties(inputPath, baseUrl, classBuilder, inputPath);

            classBuilder.Add("}");
            return string.Join(Environment.NewLine, classBuilder);
        }

        private static void BuildProperties(string folderPath, string baseUrl, List<string> classBuilder, string rootPath, string prefix = "")
        {
            var subfolders = Directory.GetDirectories(folderPath);
            var files = Directory.GetFiles(folderPath);

            // Ignore this folder if it has no subfolders and no files
            if (!subfolders.Any() && !files.Any())
            {
                return;
            }

            // Add subfolder properties
            foreach (var subfolder in subfolders)
            {
                // Check if the subfolder is empty; skip if it is
                if (!Directory.GetDirectories(subfolder).Any() && !Directory.GetFiles(subfolder).Any())
                {
                    continue;
                }

                string folderName = SanitizeName(Path.GetFileName(subfolder));
                string propertyName = $"{prefix}{folderName}";
                string relativePath = GetRelativePath(subfolder, rootPath).Replace("\\", "/"); // Convert to URL-style paths
                classBuilder.Add($"    public readonly string {propertyName} = @\"{baseUrl}/{relativePath}\";");
                BuildProperties(subfolder, baseUrl, classBuilder, rootPath, $"{propertyName}_");
            }

            // Add file properties with extension suffix
            foreach (var file in files)
            {
                string fileName = SanitizeName(Path.GetFileNameWithoutExtension(file));
                string fileExtension = SanitizeName(Path.GetExtension(file).TrimStart('.'));
                string propertyName = $"{prefix}{fileName}_{fileExtension}";
                string relativePath = GetRelativePath(file, rootPath).Replace("\\", "/"); // Convert to URL-style paths
                classBuilder.Add($"    public readonly string {propertyName} = @\"{baseUrl}/{relativePath}\";");
            }
        }

        private static string GetRelativePath(string fullPath, string rootPath)
        {
            return Path.GetRelativePath(rootPath, fullPath);
        }

        private static string SanitizeName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return string.Empty;
            }

            // Reserved keywords in C#
            var reservedKeywords = new HashSet<string>
            {
            "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class",
            "const", "continue", "decimal", "default", "delegate", "do", "double", "else", "enum", "event",
            "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", "goto", "if",
            "implicit", "in", "int", "interface", "internal", "is", "lock", "long", "namespace", "new",
            "null", "object", "operator", "out", "override", "params", "private", "protected", "public",
            "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static",
            "string", "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong",
            "unchecked", "unsafe", "ushort", "using", "virtual", "void", "volatile", "while", "async",
            "await", "var", "dynamic", "yield"
            };

            // Replace invalid characters with underscores
            var invalidCharacters = new[]
            {
                '.', '-', ' ', '(', ')', '=', '{', '}', '[', ']', '+', ',', '#',
                '~', '!', '@', '$', '%', '^', '&', '*', ':', ';', '"', '<', '>', '/', '|', '\\', '\''
            };
            foreach (var ch in invalidCharacters)
            {
                name = name.Replace(ch, '_');
            }

            // Ensure the name does not start with a digit
            if (char.IsDigit(name[0]))
            {
                name = "_" + name;
            }

            if (reservedKeywords.Contains(name))
            {
                name = "_" + name;
            }

            return name;
        }
    }
}
