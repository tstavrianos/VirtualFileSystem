### Virtual File System
Clone of [PhysicsFS]( https://icculus.org/physfs/) and [ttvfs](https://github.com/fgenesis/ttvfs) written in C#.

#### Features:
* linux-like mounting of filesystems to directory paths.
* Multiple filesystems can be mapped to the same path (or to parts of the same tree).
  * Files and directories are resolved in a first come first serve fashion.
* Able to automatically treat archives as directories, without needing to mount them first.
  See the static constructor of VirtualFileSystem.SharpZipLib.ZipFileSystem for a reference on how this should be setup in custom archive formats.
* Native and Zip filesystems included.
  * ZipFileSystem is using [SharpZipLib](https://www.nuget.org/packages/SharpZipLib/) internally. 
  
 #### Todo:
 * Write support.
   * Aim is to have it work in the same way as [PhysicsFS]( https://icculus.org/physfs/), have a writeable filesystem that is meant to be the head of the resolve tree
     * How would this work with filesystems mounted in different root folders?
     * How to deal with files that get removed or moved around?
       * Have a list in the writeable filesystem that has all the files that were marked as deleted, while did not actually exist in the writeable filesystem in the first place(but the resolve function will still report as deleted).
* Add a Tar filesystem.
* Add a 7zip filesystem.
* Add wildcards/filtering to the enumerate functions.
* Add a DirectoryInfo struct (mimicking System.IO.DirectoryInfo).
  * Maybe merge FileInfo and IFile (and DirectoryInfo and IDirectory) at a second stage.

#### Credits:
(in no particular order)
* [PhysicsFS]( https://icculus.org/physfs/), overall idea for the class.
* [ttvfs](https://github.com/fgenesis/ttvfs), inspired the use of separate File and Directory objects for each filesystem.
* [sharpfilesystem](https://github.com/bobvanderlinden/sharpfilesystem) for ideas regarding the Zip filesystem.
* [zio](https://github.com/xoofx/zio) for UPath.