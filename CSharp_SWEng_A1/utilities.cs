//Created by Vitaly Dekhtyarev 01.02.2022.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

class Utilities
{
    public class ctrl
    {
        //The class is used to control parallel threads.
        public double lt = 0;
        public double st = 0;
        public string path = "";
        public int iCtrl=0;
    }
    public class rep
    {
        //The class is used to obtain output from parallel threads.
        public DateTime startTime;
        public DateTime endTimeSamp;
        public DateTime endTimeLog;
        public int iRep = 0;
        public double dRep = 0;
        public string sRep = "";
    }
    public static string bReadFile(string path)
    {
        //Method to read data from files as bytes.
        if (!File.Exists(path))
        {
            //return $"File does not exist:{path}";
            return null;
        }
        /*
        //Version 1
        var finfo = new FileInfo(path);

        using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
            byte[] b = new byte[finfo.Length];

            using (BinaryReader br = new BinaryReader(fs))
            {
                for (int i = 0; i < finfo.Length; i++)
                {
                    b[i] = br.ReadByte();
                }
                UTF8Encoding temp = new UTF8Encoding(true);
                return temp.GetString(b);
            }
        }
        */
        /*
        //Version 2
        var finfo = new FileInfo(path);

        string outStr = "";

        using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
            using (BinaryReader br = new BinaryReader(fs))
            {
                for (int i = 0; i < finfo.Length; i++)
                {
                    outStr += (char)br.ReadByte();
                }
                return outStr;
            }
        }
        */
        //Version 3
        //return File.ReadAllText(path);

        //Version 4
        UTF8Encoding temp = new UTF8Encoding(true);
        return temp.GetString(File.ReadAllBytes(path));
        

    }
    static public int bWriteFile(string path, string msg)
    {
        //Method to write bytes to files.
        int written = 0;
        if (!File.Exists(path))
        {
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        for (written = 0; written < msg.Length; written++)
                        {
                            bw.Write(msg[written]);
                        }
                    }
                }
            }
            catch (System.UnauthorizedAccessException e)
            {
                written = -1;
            }
            return written;
        }
        else
        {
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write))
                {
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        for (written = 0; written < msg.Length; written++)
                        {
                            bw.Write(msg[written]);
                        }
                    }
                }
            }
            catch (System.UnauthorizedAccessException e)
            {
                written = -1;
            }
            return written;

        }
    }
    static public int sensCount(ISen[] sen)
    {
        //Method to sount number of sensors in array with sensors.
        int sensCount = 0;
        for (int i = 0; i < sen.Length; i++)
        {
            if (sen[i] != null)
            {
                sensCount++;
            }
        }
        return sensCount;
    }
}


