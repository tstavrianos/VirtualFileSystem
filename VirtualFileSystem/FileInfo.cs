using System;

namespace VirtualFileSystem
{
    public readonly struct FileInfo: IEquatable<FileInfo>
    {
        public FileInfo(string name, UPath fullName, string actualName, long length, DateTime creationTime, DateTime lastAccessTime, DateTime lastWriteTime)
        {
            this.Name = name;
            this.FullName = fullName;
            this.ActualName = actualName;
            this.Length = length;
            this.CreationTime = creationTime;
            this.LastAccessTime = lastAccessTime;
            this.LastWriteTime = lastWriteTime;
        }

        public string Name { get; }
        public UPath FullName { get; }
        internal string ActualName { get; }
        public long Length { get; }
        public DateTime CreationTime { get; }
        public DateTime LastAccessTime { get; }
        public DateTime LastWriteTime { get; }

        internal FileInfo(in UPath path, System.IO.FileInfo fi)
        {
            this.Name = fi.Name;
            this.FullName = path;
            this.ActualName = fi.FullName;
            this.Length = fi.Length;
            this.CreationTime = fi.CreationTime;
            this.LastAccessTime = fi.LastAccessTime;
            this.LastWriteTime = fi.LastWriteTime;
        }
        
        internal FileInfo(in UPath path, FileInfo fi)
        {
            this.Name = fi.Name;
            this.FullName = path;
            this.ActualName = fi.ActualName;
            this.Length = fi.Length;
            this.CreationTime = fi.CreationTime;
            this.LastAccessTime = fi.LastAccessTime;
            this.LastWriteTime = fi.LastWriteTime;
        }

        public bool Equals(FileInfo other)
        {
            return this.FullName == other.FullName;
        }

        public override bool Equals(object obj)
        {
            return obj is FileInfo other && this.Equals(other);
        }

        public override int GetHashCode()
        {
            return (this.FullName != null ? this.FullName.GetHashCode() : 0);
        }
    }
}