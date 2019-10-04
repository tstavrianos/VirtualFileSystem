using System.Collections.Generic;

namespace VirtualFileSystem
{
    public interface IDirectory
    {
        string Name { get; }
        UPath FullPath { get; }
        
        bool DirectoryExists(in UPath path);
        IDirectory OpenDirectory(in UPath path);
        IEnumerable<IDirectory> EnumerateDirectories();
        
        bool FileExists(in UPath path);
        IFile OpenFile(in UPath path);
        IEnumerable<IFile> EnumerateFiles();
    }
}