module IOUtils =
  //Function composition
  open System.IO
  
  let rec private filesUnderFolder rootFolder = 
      seq {
          for file in Directory.GetFiles(rootFolder) do
              yield file
          for dir in Directory.GetDirectories(rootFolder) do
              yield! filesUnderFolder dir 
      }
  
  let private fileInfo filename = new FileInfo(filename)
  
  // Gets the file size from a FileInfo object
  let private fileSize (fileinfo : FileInfo) = fileinfo.Length
  
  // Converts a byte count to MB
  let private bytesToMB bytes = bytes / (1024L * 1024L)
  
  let getFolderSize = 
      filesUnderFolder 
      >> Seq.map fileInfo 
      >> Seq.map fileSize 
      >> Seq.fold (+) 0L 
      >> bytesToMB

module Awesome =
  open IOUtils
  getFolderSize @"C:\temp"