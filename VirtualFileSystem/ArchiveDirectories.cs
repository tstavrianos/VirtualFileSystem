using System;
using System.Collections.Generic;
using System.IO;

namespace VirtualFileSystem
{
    public static class ArchiveDirectories
    {
        private static readonly Dictionary<string, Func<IFile, IDirectory>> Map = new Dictionary<string, Func<IFile, IDirectory>>(StringComparer.OrdinalIgnoreCase);

        public static void Register(in string extension, Func<IFile, IDirectory> constructor)
        {
            Map[extension] = constructor;
        }

        public static bool Supported(in string path)
        {
            return Map.ContainsKey(Path.GetExtension(path));
        }

        public static IDirectory Open(IFile file)
        {
            return !Supported(file.Name) ? null : Map[Path.GetExtension(file.Name)].Invoke(file);
        }
    }
}