using System;
using System.Collections.Generic;

namespace VirtualFileSystem.SharpZipLib
{
    public sealed class ZipFileSystem: IFileSystem, IDisposable
    {
        static ZipFileSystem()
        {
            ArchiveDirectories.Register(".zip", ZipDirectory.FromFile);
        }
        
        private readonly ICSharpCode.SharpZipLib.Zip.ZipFile _file;
        private readonly bool _ownArchive;
        
        public ZipFileSystem(ICSharpCode.SharpZipLib.Zip.ZipFile file, bool ownArchive = true)
        {
            this._file = file;
            this._ownArchive = ownArchive;
        }
        
        public bool DirectoryExists(in UPath path)
        {
            var actualPath = ConvertPathToInternal(path);
            var e = this._file.GetEntry(actualPath);
            if (e == null) return false;
            return e.IsDirectory || ArchiveDirectories.Supported(e.Name);
        }

        public bool FileExists(in UPath path)
        {
            var actualPath = ConvertPathToInternal(path);
            var e = this._file.GetEntry(actualPath);
            return e?.IsFile == true;
        }

        public IFile OpenFile(in UPath path)
        {
            var actualPath = ConvertPathToInternal(path);
            var e = this._file.GetEntry(actualPath);
            return e?.IsDirectory != false ? null : new ZipFile(this._file, e, path.Name, path);
        }

        public IDirectory OpenDirectory(in UPath path)
        {
            var actualPath = ConvertPathToInternal(path);
            var e = this._file.GetEntry(actualPath);
            if (e == null) return null;
            if(e.IsDirectory) return new ZipDirectory(this._file, path.Name, path);

            var p = ConvertPathFromInternal(e.Name);
            return ArchiveDirectories.Supported(e.Name) ? ArchiveDirectories.Open(new ZipFile(this._file, e, p.Name, p)) : null;
        }

        public IEnumerable<IFile> EnumerateFiles()
        {
            for (var i = 0; i < this._file.Count; i++)
            {
                var e = this._file[i];
                var p = ConvertPathFromInternal(e.Name);
                if(!p.IsInDirectory(UPath.Root, false)) continue;
                
                if (e.IsFile)
                {
                    yield return new ZipFile(this._file, e, p.Name, p);
                }
            }
        }

        public IEnumerable<IDirectory> EnumerateDirectories()
        {
            for (var i = 0; i < this._file.Count; i++)
            {
                var e = this._file[i];
                var p = ConvertPathFromInternal(e.Name);
                if(!p.IsInDirectory(UPath.Root, false)) continue;
                
                if (e.IsDirectory)
                {
                    yield return new ZipDirectory(this._file, p.Name, p);
                }

                if (e.IsFile && ArchiveDirectories.Supported(e.Name))
                {
                    yield return ArchiveDirectories.Open(new ZipFile(this._file, e, p.Name, p));
                }
            }
        }

        internal static string ConvertPathToInternal(in UPath path) => path.FullName.TrimStart('/');

        internal static UPath ConvertPathFromInternal(in string systemPath) => new UPath('/' + systemPath);

        public void Dispose()
        {
            if(this._ownArchive)((IDisposable) this._file)?.Dispose();
        }
    }
}