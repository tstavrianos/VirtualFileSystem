using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VirtualFileSystem.FileSystems
{
    public sealed class PhysicalDirectory: IDirectory
    {
        public string Name { get; }
        public UPath FullPath { get; }
        private readonly PhysicalFileSystem _fileSystem;

        internal PhysicalDirectory(PhysicalFileSystem fileSystem, in string name, in UPath fullname)
        {
            this._fileSystem = fileSystem;
            this.Name = name;
            this.FullPath = fullname;
        }
        
        public IEnumerable<IFile> EnumerateFiles()
        {
            var actualPath = this.FullPath.FullName;
            if (!Directory.Exists(actualPath)) yield break;
            foreach (var fi in Directory.EnumerateFiles(actualPath).Select(Path.GetFileName))
            {
                yield return new PhysicalFile(this._fileSystem, fi, this.FullPath / fi);
            }        
        }

        public IEnumerable<IDirectory> EnumerateDirectories()
        {
            var actualPath = this.FullPath.FullName;
            if (!Directory.Exists(actualPath)) yield break;
            foreach (var fi in Directory.EnumerateDirectories(actualPath).Select(Path.GetFileName))
            {
                yield return new PhysicalDirectory(this._fileSystem, fi, this.FullPath / fi);
            }

            foreach (var fi in Directory.EnumerateFiles(actualPath).Select(Path.GetFileName).Where(fi => ArchiveDirectories.Supported(fi)))
            {
                yield return ArchiveDirectories.Open(new PhysicalFile(this._fileSystem, fi, this.FullPath / fi));
            }
        }

        public bool DirectoryExists(in UPath path)
        {
            return path.IsRelative && this._fileSystem.DirectoryExists(this.FullPath / path);
        }

        public bool FileExists(in UPath path)
        {
            return path.IsRelative && this._fileSystem.FileExists(this.FullPath / path);
        }

        public IFile OpenFile(in UPath path)
        {
            return !path.IsRelative ? null : this._fileSystem.OpenFile(this.FullPath / path);
        }

        public IDirectory OpenDirectory(in UPath path)
        {
            return !path.IsRelative ? null : this._fileSystem.OpenDirectory(this.FullPath / path);
        }
    }
}