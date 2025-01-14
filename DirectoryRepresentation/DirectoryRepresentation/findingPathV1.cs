using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace DirectoryRepresentation
{
    public static class findingPathV1
    {
        private static bool IsDirectoryEmpty(string path)
        {
            // A directory is considered empty if it has no files and all its subdirectories are empty
            var files = Directory.GetFiles(path);
            if (files.Length > 0) return false;

            var dirs = Directory.GetDirectories(path);
            return dirs.All(dir => IsDirectoryEmpty(dir));
        }

        public static string findPath(string rootPath, string baseUrl)
        {
            if (string.IsNullOrWhiteSpace(rootPath))
                throw new ArgumentException("Root path cannot be empty", nameof(rootPath));

            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentException("Base URL cannot be empty", nameof(baseUrl));

            if (!Directory.Exists(rootPath))
                throw new DirectoryNotFoundException($"Directory not found: {rootPath}");

            rootPath = Path.GetFullPath(rootPath).TrimEnd(Path.DirectorySeparatorChar);
            baseUrl = baseUrl.TrimEnd('/');

            var rootName = Path.GetFileName(rootPath);
            var sanitizedRootName = SanitizeName(rootName);

            var state = new GeneratorState
            {
                RootPath = rootPath,
                BaseUrl = baseUrl,
                ClassBuilder = new StringBuilder(),
                FolderCounter = 1,
                FolderNameMap = new Dictionary<string, string>()
            };
            // Add the namespace
            state.ClassBuilder.AppendLine("using System;");
            state.ClassBuilder.AppendLine();
            state.ClassBuilder.AppendLine("namespace DirectoryStructure");
            state.ClassBuilder.AppendLine("{");

            // Add the root class
            state.ClassBuilder.AppendLine($"    public class {sanitizedRootName}");
            state.ClassBuilder.AppendLine("    {");

            ProcessDirectory(state, rootPath, 2, new Stack<string>());

            state.ClassBuilder.AppendLine("    }");
            state.ClassBuilder.AppendLine("}");

            return state.ClassBuilder.ToString();
        }

        private class GeneratorState
        {
            public string RootPath { get; set; }
            public string BaseUrl { get; set; }
            public StringBuilder ClassBuilder { get; set; }
            public int FolderCounter { get; set; }
            public Dictionary<string, string> FolderNameMap { get; set; }
        }

        private static bool ProcessDirectory(GeneratorState state, string currentPath, int indent, Stack<string> classStack)
        {
            try
            {
                var files = Directory.GetFiles(currentPath).OrderBy(f => f).ToList();
                var directories = Directory.GetDirectories(currentPath)
                    .Where(dir => !IsDirectoryEmpty(dir))
                    .OrderBy(d => d)
                    .ToList();

                if (files.Count == 0 && directories.Count == 0)
                {
                    return false; // skip empty directory
                }

                // Process all non-empty subdirectories first to create their classes
                var processedDirs = new List<string>();
                foreach (var dir in directories)
                {
                    var dirName = Path.GetFileName(dir);
                    var folderClassName = $"folder_{state.FolderCounter:D3}";
                    state.FolderCounter++;

                    AppendIndent(state.ClassBuilder, indent);
                    state.ClassBuilder.AppendLine($"public class {folderClassName}");
                    AppendIndent(state.ClassBuilder, indent);
                    state.ClassBuilder.AppendLine("{");

                    classStack.Push(folderClassName);
                    if (ProcessDirectory(state, dir, indent + 1, classStack))
                    {
                        state.FolderNameMap[dir] = folderClassName;
                        processedDirs.Add(dir);
                    }
                    classStack.Pop();

                    AppendIndent(state.ClassBuilder, indent);
                    state.ClassBuilder.AppendLine("}");
                }

                // Create member variables for non-empty subdirectories
                foreach (var dir in processedDirs)
                {
                    var dirName = Path.GetFileName(dir);
                    var sanitizedName = SanitizeName(dirName);
                    var folderClassName = state.FolderNameMap[dir];

                    AppendIndent(state.ClassBuilder, indent);
                    state.ClassBuilder.AppendLine($"public readonly {folderClassName} {sanitizedName} = new {folderClassName}();");
                }

                // Process all files
                foreach (var file in files)
                {
                    var fileName = Path.GetFileName(file);
                    var sanitizedName = SanitizeName(fileName);
                    var relativePath = GetRelativePath(state.RootPath, file);
                    var url = $"{state.BaseUrl}/{relativePath.Replace('\\', '/')}";

                    AppendIndent(state.ClassBuilder, indent);
                    state.ClassBuilder.AppendLine($"public readonly string {sanitizedName} = \"{url}\";");
                }

                return true; // Directory has content
            }
            catch (Exception ex)
            {
                AppendIndent(state.ClassBuilder, indent);
                state.ClassBuilder.AppendLine($"// Error processing directory: {ex.Message}");
                return false;
            }
        }

        private static string GetRelativePath(string rootPath, string fullPath)
        {
            string relativePath = fullPath.Substring(rootPath.Length).TrimStart('\\', '/');
            return relativePath;
        }

        private static void AppendIndent(StringBuilder builder, int indent)
        {
            builder.Append(new string(' ', indent * 4));
        }

        private static string SanitizeName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return "empty";

            // Reserved keywords in C#
            var reservedKeywords = new HashSet<string>
        {
            "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked",
            "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else",
            "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for",
            "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock",
            "long", "namespace", "new", "null", "object", "operator", "out", "override", "params",
            "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed",
            "short", "sizeof", "stackalloc", "static", "string", "struct", "switch", "this",
            "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort",
            "using", "virtual", "void", "volatile", "while", "async", "await", "var", "dynamic", "yield"
        };

            var invalidCharacters = new[]
            {
            '.', '-', ' ', '(', ')', '=', '{', '}', '[', ']', '+', ',', '#', '~', '!', '@', '$',
            '%', '^', '&', '*', ':', ';', '"', '<', '>', '/', '|', '\\', '\''
        };

            foreach (var ch in invalidCharacters)
            {
                name = name.Replace(ch, '_');
            }

            // Handle empty string after sanitization
            if (string.IsNullOrEmpty(name))
                return "empty";

            // Handle digits at the start
            if (char.IsDigit(name[0]))
                name = "_" + name;

            // Handle reserved keywords
            if (reservedKeywords.Contains(name))
                name = "_" + name;

            return name;
        }
    }
}
