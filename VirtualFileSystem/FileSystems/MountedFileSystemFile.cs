using System.IO;

namespace VirtualFileSystem.FileSystems
{
    public sealed class MountedFileSystemFile: IFile
    {
        public string Name { get; }
        public UPath FullPath { get; }
        private readonly IFile _file;

        internal MountedFileSystemFile(IFile file, in string name, in UPath fullname)
        {
            this._file = file;
            this.Name = name;
            this.FullPath = fullname;
        }
        
        public Stream OpenRead()
        {
            return this._file.OpenRead();
        }

        public FileInfo GetFileInfo()
        {
            return new FileInfo(this.FullPath, this._file.GetFileInfo());
        }
    }
}