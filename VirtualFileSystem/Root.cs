using System.Collections.Generic;
using System.Linq;
using VirtualFileSystem.FileSystems;

namespace VirtualFileSystem
{
    public sealed class Root: IFileSystem
    {
        private readonly LinkedList<MountedFileSystem> _root;

        public Root()
        {
            this._root = new LinkedList<MountedFileSystem>();
        }

        public void Mount(UPath mountPoint, IFileSystem fileSystem, bool append = true)
        {
            var fs = new MountedFileSystem(mountPoint, fileSystem);
            if (append)
            {
                this._root.AddLast(fs);
            }
            else
            {
                this._root.AddFirst(fs);
            }
        }

        public bool DirectoryExists(in UPath p)
        {
            var path = p;
            return this._root.Any(i => i.DirectoryExists(path));
        }

        public IFile OpenFile(in UPath p)
        {
            var path = p;
            return (from i in this._root where i.FileExists(path) select i.OpenFile(path)).FirstOrDefault();
        }

        public IDirectory OpenDirectory(in UPath p)
        {
            var path = p;
            return (from i in this._root where i.DirectoryExists(path) select i.OpenDirectory(path)).FirstOrDefault();
        }

        public IEnumerable<IFile> EnumerateFiles()
        {
            return this._root.SelectMany(i => i.EnumerateFiles());
        }

        public IEnumerable<IDirectory> EnumerateDirectories()
        {
            return this._root.SelectMany(i => i.EnumerateDirectories());
        }

        public bool FileExists(in UPath p)
        {
            var path = p;
            return this._root.Any(i => i.FileExists(path));
        }
    }
}