using System;
using System.IO;
using VirtualFileSystem.FileSystems;
using VirtualFileSystem.SharpZipLib;
using ZipFile = ICSharpCode.SharpZipLib.Zip.ZipFile;

namespace VirtualFileSystem.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var d1 = new PhysicalFileSystem(Path.Combine(Environment.CurrentDirectory, "Assets"));
            var d2 = new ZipFileSystem(new ZipFile(Path.Combine(Environment.CurrentDirectory, "Assets/test.zip")));
            var vfs = new Root();
            vfs.Mount("/test", d1);
            vfs.Mount("/test", d2);

            Console.WriteLine("Folders:");
            foreach(var d in vfs.EnumerateDirectories())
            {
                Console.WriteLine($"{d.FullPath} => {d.Name}");
            }
            Console.WriteLine("");
            Console.WriteLine("Files:");
            var dir = vfs.OpenDirectory("/test/test.zip");
            foreach(var d in dir.EnumerateFiles())
            {
                Console.WriteLine($"{d.FullPath} => {d.Name}");
            }        
        }
    }
}