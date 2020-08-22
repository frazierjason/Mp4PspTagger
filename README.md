# Mp4PspTagger
Sample C# .NET application that can read and update the title, date and encoder tag strings inside a Memory Stick Video compliant .MP4 file for Sony PSP menu display and ordering.

Mp4PspTagger and MSVTagger v0.11
Released 15-May-2005

This is a sample C# .NET application that can read and update the title, date and encoder tag strings inside a Memory Stick Video compliant .MP4 file.  The sample consists of an MSVTagger .DLL component that does all the actual work, and a front end Mp4PspTagger .EXE that shows how to use the .DLL to read and update the tags.  It is designed so that anyone may use the DLL's functionality within their own applications.

This sample and its source is freeware and anyone may use it in any way they wish.  It has been moderately tested with .MP4 files encoded by FFMPEG, 3GP Converter, Image Converter 2, Elecard, and Nero.  Please note though that there may be bugs in the implementation, and that this app is totally capable of trashing your .MP4 file if something goes awry.  I take no responsibility for any damage done to your files, software or hardware as a result of using this sample.

At the time in 2005, I had not been writing code for very long, so the code quality may not be that great.  But it worked for me, and that's all I really wanted.  The MSVTagger library was also reused in PSPVideo9:
http://www.pspvideo9.com/

Thanks,
Jason Frazier
