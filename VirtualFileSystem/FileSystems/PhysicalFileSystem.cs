using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace VirtualFileSystem.FileSystems
{
    public sealed class PhysicalFileSystem: IFileSystem
    {
        private static readonly bool IsOnWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        public PhysicalFileSystem(string location)
        {
            var path = new UPath(location);
            this.Root = new PhysicalDirectory(this, path.Name, path);
        }

        private IDirectory Root { get; }

        public bool DirectoryExists(in UPath path)
        {
            var actualPath = this.ConvertPathToInternal(path);
            return Directory.Exists(actualPath) || (this.FileExists(path) && ArchiveDirectories.Supported(actualPath));
        }

        public bool FileExists(in UPath path)
        {
            var actualPath = this.ConvertPathToInternal(path);
            return File.Exists(actualPath);
        }

        public IFile OpenFile(in UPath path)
        {
            var actualPath = this.ConvertPathToInternal(path);
            return !File.Exists(actualPath) ? null : new PhysicalFile(this, Path.GetFileName(actualPath), actualPath);
        }

        public IDirectory OpenDirectory(in UPath path)
        {
            var actualPath = this.ConvertPathToInternal(path);
            if (Directory.Exists(actualPath))
                return new PhysicalDirectory(this, Path.GetFileName(actualPath), actualPath);

            if (File.Exists(actualPath) && ArchiveDirectories.Supported(actualPath))
            {
                return ArchiveDirectories.Open(this.OpenFile(path));
            }

            return null;
        }

        public IEnumerable<IFile> EnumerateFiles()
        {
            return this.Root.EnumerateFiles();
        }

        public IEnumerable<IDirectory> EnumerateDirectories()
        {
            return this.Root.EnumerateDirectories();
        }

        internal string ConvertPathToInternal(in UPath path)
        {
            if (path.FullName.Length == 0) return this.Root.FullPath.FullName;
            var absolutePath = (this.Root.FullPath / path.FullName.Substring(1)).FullName;
            if (IsOnWindows)
            {
                absolutePath = absolutePath.Replace(UPath.DirectorySeparator, '\\');
            }

            return absolutePath;
        }

        internal UPath ConvertPathFromInternal(in string innerPath)
        {
            var newPath = new UPath(innerPath);
            return new UPath(newPath.FullName.Remove(0, this.Root.FullPath.FullName.Length));
        }
    }
}