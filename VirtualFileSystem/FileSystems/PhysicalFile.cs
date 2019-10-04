using System.IO;

namespace VirtualFileSystem.FileSystems
{
    public sealed class PhysicalFile: IFile
    {
        public string Name { get; }
        public UPath FullPath { get; }
        private readonly PhysicalFileSystem _fileSystem;

        internal PhysicalFile(PhysicalFileSystem fileSystem, in string name, in UPath fullname)
        {
            this._fileSystem = fileSystem;
            this.Name = name;
            this.FullPath = fullname;
        }
        
        public Stream OpenRead()
        {
            var actualPath = this.FullPath.FullName;
            return !File.Exists(actualPath) ? null : File.OpenRead(actualPath);        
        }

        public FileInfo GetFileInfo()
        {
            var actualPath = this._fileSystem.ConvertPathToInternal(this.FullPath);
            return !File.Exists(actualPath) ? default : new FileInfo(this.FullPath, new System.IO.FileInfo(actualPath));
        }
    }
}