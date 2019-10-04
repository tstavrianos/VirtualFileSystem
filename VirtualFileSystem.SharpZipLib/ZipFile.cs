using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace VirtualFileSystem.SharpZipLib
{
    public sealed class ZipFile: IFile
    {
        public string Name { get; }
        public UPath FullPath { get; }
        private readonly ZipEntry _entry;
        private readonly ICSharpCode.SharpZipLib.Zip.ZipFile _archive;

        internal ZipFile(ICSharpCode.SharpZipLib.Zip.ZipFile archive, ZipEntry entry, in string name, in UPath fullname)
        {
            this.Name = name;
            this.FullPath = fullname;
            this._entry = entry;
            this._archive = archive;
        }
        
        public Stream OpenRead()
        {
            return this._archive.GetInputStream(this._entry);
        }

        public FileInfo GetFileInfo()
        {
            return new FileInfo(this.Name, this.FullPath, this.FullPath.FullName, this._entry.Size, this._entry.DateTime, this._entry.DateTime, this._entry.DateTime);
        }
    }
}