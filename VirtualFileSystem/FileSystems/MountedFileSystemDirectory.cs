using System.Collections.Generic;
using System.Linq;

namespace VirtualFileSystem.FileSystems
{
    public sealed class MountedFileSystemDirectory: IDirectory
    {
        public string Name { get; }
        public UPath FullPath { get; }
        private readonly IDirectory _directory;

        internal MountedFileSystemDirectory(IDirectory directory, in string name, in UPath fullname)
        {
            this._directory = directory;
            this.Name = name;
            this.FullPath = fullname;
        }
        
        public IEnumerable<IFile> EnumerateFiles()
        {
            return this._directory.EnumerateFiles().Select(it => new MountedFileSystemFile(it, it.Name, this.FullPath / it.Name));
        }

        public IEnumerable<IDirectory> EnumerateDirectories()
        {
            return this._directory.EnumerateDirectories().Select(it => new MountedFileSystemDirectory(it, it.Name, this.FullPath / it.Name));
        }
        
        public bool DirectoryExists(in UPath path)
        {
            return path.IsRelative && this._directory.DirectoryExists(path);
        }

        public bool FileExists(in UPath path)
        {
            return path.IsRelative && this._directory.FileExists(path);
        }

        public IFile OpenFile(in UPath path)
        {
            return !path.IsRelative ? null : this._directory.OpenFile(path);
        }

        public IDirectory OpenDirectory(in UPath path)
        {
            return !path.IsRelative ? null : this._directory.OpenDirectory(path);
        }
    }
}