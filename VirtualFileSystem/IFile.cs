using System.IO;

namespace VirtualFileSystem
{
    public interface IFile
    {
        string Name { get; }
        UPath FullPath { get; }
        Stream OpenRead();
        FileInfo GetFileInfo();
    }
}