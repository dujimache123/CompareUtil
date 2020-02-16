using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Util.Event;
using System.Threading;

namespace Compare
{
    class CompareUtil
    {
        static bool IsValidDir(string dirName)
        {
            string[] ignore = { ".git" };
            foreach(var s in ignore)
            {
                if (s == dirName)
                    return false;
            }
            return true;
        }

        public static void DeleteDir(string file)
        {
            try
            {
                //去除文件夹和子文件的只读属性
                //去除文件夹的只读属性
                System.IO.DirectoryInfo fileInfo = new DirectoryInfo(file);
                fileInfo.Attributes = FileAttributes.Normal & FileAttributes.Directory;

                //去除文件的只读属性
                System.IO.File.SetAttributes(file, System.IO.FileAttributes.Normal);

                //判断文件夹是否还存在
                if (Directory.Exists(file))
                {
                    foreach (string f in Directory.GetFileSystemEntries(file))
                    {
                        if (File.Exists(f))
                        {
                            //如果有子文件删除文件
                            File.Delete(f);
                            EventDispatcher.DispatchEvent(1001, "删除文件" + file);
                            Thread.Sleep(10);
                        }
                        else
                        {
                            //循环递归删除子文件夹
                            DeleteDir(f);
                        }
                    }

                    //删除空文件夹
                    Directory.Delete(file);
                }

            }
            catch (Exception ex) // 异常处理
            {
                
            }
        }

        public static void CopyDirectory(string sourceDir, string destDir)
        {
            if (Directory.Exists(destDir) == false)
                Directory.CreateDirectory(destDir);

            destDir += "\\";
            var files = Directory.GetFiles(sourceDir);
            foreach(var file in files)
            {
                FileInfo flinfo = new FileInfo(file);
                flinfo.CopyTo(destDir + flinfo.Name, true);
            }

            var dirs = Directory.GetDirectories(sourceDir);
            foreach(var path in dirs)
            {
                DirectoryInfo dirInfo = new DirectoryInfo(path);
                CompareUtil.CopyDirectory(path, destDir + dirInfo.Name);
            }
            
        }

        public static void CompareDirectory(string oldDir, string newDir, string destDir)
        {
            destDir += "\\";
            var dirs = Directory.GetDirectories(newDir);
            foreach (var dir in dirs)
            {
                var dirName = Path.GetFileName(dir);
                if(!IsValidDir(dirName))
                {
                    continue;
                }
                var path1 = oldDir + "//" + dirName;
                var path2 = destDir + dirName;
                if (!Directory.Exists(path1))
                {
                    CompareUtil.CopyDirectory(dir, path2);
                }
                else
                {
                    EventDispatcher.DispatchEvent(1001, dir);
                    Thread.Sleep(10);
                    CompareUtil.CompareDirectory(path1, dir, path2);  
                }
            }

            var files = Directory.GetFiles(newDir);
            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                var path1 = oldDir + "//" + fileName;
                var flag = false;
                if(!File.Exists(path1))
                {
                    flag = true;
                }
                else
                {
                    var hash1 = Md5HashUtil.GetMD5HashFromFile(file);
                    var hash2 = Md5HashUtil.GetMD5HashFromFile(path1);
                    if (hash1 != hash2)
                    {
                        flag = true;
                    }
                }

                if(flag)
                {
                    if (Directory.Exists(destDir) == false)
                        Directory.CreateDirectory(destDir);
                    FileInfo flinfo = new FileInfo(file);
                    flinfo.CopyTo(destDir + flinfo.Name, true);
                    EventDispatcher.DispatchEvent(1001, destDir + flinfo.Name);
                    Thread.Sleep(10);
                }
            }
        }
    }
}
