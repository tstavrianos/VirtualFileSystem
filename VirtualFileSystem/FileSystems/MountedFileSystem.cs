using System.Collections.Generic;
using System.Linq;

namespace VirtualFileSystem.FileSystems
{
    internal sealed class MountedFileSystem: IFileSystem
    {
        private readonly IFileSystem _fileSystem;

        internal MountedFileSystem(UPath mountPoint, IFileSystem fileSystem)
        {
            this.Root =  mountPoint;
            this._fileSystem = fileSystem;
        }

        private UPath Root { get; }

        public bool DirectoryExists(in UPath path)
        {
            var actualPath = path.FullName.Remove(0, this.Root.FullName.Length);
            return this._fileSystem.DirectoryExists(actualPath);
        }

        public IFile OpenFile(in UPath path)
        {
            var actualPath = path.FullName.Remove(0, this.Root.FullName.Length);
            var file = this._fileSystem.OpenFile(actualPath);
            return file != null ? new MountedFileSystemFile(file, file.Name, path) : null;
        }

        public IDirectory OpenDirectory(in UPath path)
        {
            var actualPath = path.FullName.Remove(0, this.Root.FullName.Length);
            var dir = this._fileSystem.OpenDirectory(actualPath);
            return dir != null ? new MountedFileSystemDirectory(dir, dir.Name, path) : null;
        }

        public IEnumerable<IFile> EnumerateFiles()
        {
            return this._fileSystem.EnumerateFiles().Select(file => new MountedFileSystemFile(file, file.Name, this.Root / file.Name));
        }

        public IEnumerable<IDirectory> EnumerateDirectories()
        {
            return this._fileSystem.EnumerateDirectories().Select(file => new MountedFileSystemDirectory(file, file.Name, this.Root / file.Name));
        }

        public bool FileExists(in UPath path)
        {
            var actualPath = path.FullName.Remove(0, this.Root.FullName.Length);
            return this._fileSystem.FileExists(actualPath);
        }
    }
}