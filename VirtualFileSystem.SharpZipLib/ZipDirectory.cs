using System;
using System.Collections.Generic;

namespace VirtualFileSystem.SharpZipLib
{
    public sealed class ZipDirectory: IDirectory, IDisposable
    {
        public string Name { get; }
        public UPath FullPath { get; }
        private readonly ICSharpCode.SharpZipLib.Zip.ZipFile _archive;
        private readonly bool _ownArchive;

        internal ZipDirectory(ICSharpCode.SharpZipLib.Zip.ZipFile file, in string pathName, in UPath path, bool ownArchive = false)
        {
            this._archive = file;
            this.Name = pathName;
            this.FullPath = path;
            this._ownArchive = ownArchive;
        }

        public IEnumerable<IFile> EnumerateFiles()
        {
            for (var i = 0; i < this._archive.Count; i++)
            {
                var e = this._archive[i];
                var p = ZipFileSystem.ConvertPathFromInternal(e.Name);
                if(!p.IsInDirectory(this.FullPath, false)) continue;
                
                if (e.IsFile)
                {
                    yield return new ZipFile(this._archive, e, p.Name, p);
                }
            }
        }

        public IEnumerable<IDirectory> EnumerateDirectories()
        {
            for (var i = 0; i < this._archive.Count; i++)
            {
                var e = this._archive[i];
                var p = ZipFileSystem.ConvertPathFromInternal(e.Name);
                if(!p.IsInDirectory(this.FullPath, false)) continue;
                
                if (e.IsDirectory)
                {
                    yield return new ZipDirectory(this._archive, p.Name, p);
                }
                
                if (e.IsFile && ArchiveDirectories.Supported(e.Name))
                {
                    yield return ArchiveDirectories.Open(new ZipFile(this._archive, e, p.Name, p));
                }
            }
        }

        public void Dispose()
        {
            if(this._ownArchive) ((IDisposable) this._archive)?.Dispose();
        }

        internal static ZipDirectory FromFile(IFile file)
        {
            return new ZipDirectory(new ICSharpCode.SharpZipLib.Zip.ZipFile(file.OpenRead(), false), file.Name, UPath.Root, true);
        }
        
        public bool DirectoryExists(in UPath path)
        {
            if (!path.IsRelative) return false;

            var actualPath = ZipFileSystem.ConvertPathToInternal(this.FullPath / path);
            var e = this._archive.GetEntry(actualPath);
            if (e == null) return false;
            return e.IsDirectory || ArchiveDirectories.Supported(e.Name);
        }

        public bool FileExists(in UPath path)
        {
            if (!path.IsRelative) return false;

            var actualPath = ZipFileSystem.ConvertPathToInternal(this.FullPath / path);
            var e = this._archive.GetEntry(actualPath);
            return e?.IsFile == true;
        }

        public IFile OpenFile(in UPath path)
        {
            if (!path.IsRelative) return null;

            var actualPath = ZipFileSystem.ConvertPathToInternal(this.FullPath / path);
            var e = this._archive.GetEntry(actualPath);
            return e?.IsDirectory != false ? null : new ZipFile(this._archive, e, path.Name, this.FullPath / path);
        }

        public IDirectory OpenDirectory(in UPath path)
        {
            if (!path.IsRelative) return null;

            var actualPath = ZipFileSystem.ConvertPathToInternal(this.FullPath / path);
            var e = this._archive.GetEntry(actualPath);
            if (e == null) return null;
            if(e.IsDirectory) return new ZipDirectory(this._archive, path.Name, this.FullPath / path);

            var p = ZipFileSystem.ConvertPathFromInternal(e.Name);
            return ArchiveDirectories.Supported(e.Name) ? ArchiveDirectories.Open(new ZipFile(this._archive, e, p.Name, p)) : null;
        }
    }
}