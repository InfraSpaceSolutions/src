/******************************************************************
 *  Filename : LOGCONSOLE.CS
 *  Project  : GENDEALS.EXE
 *  
 *  )|( Sanctuary Software Studio
 *  Copyright (c) 2013 - All rights reserved.
 *  
 *  Description :
 *  Log output to a file and to the console.  Uses the app.config
 *  file to determine if output should be sent to the console.
******************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Configuration;

namespace com.unitethiscity
{
    class LogConsole
    {
        private StreamWriter logstream;
        private bool lineopen;
        private bool consoleout;

        /// <summary>
        /// Opens the log stream to store to a text file based on todays date
        /// </summary>
        /// <returns></returns>
        public bool Open()
        {
            string logfilename;
            logfilename = "log-" + DateTime.Now.Year.ToString() + DateTime.Now.ToString("MM") + DateTime.Now.Day.ToString() + ".txt";
            logstream = new StreamWriter(ConfigurationManager.AppSettings["LOG_PATH"] + logfilename, true);
            lineopen = false;
            LogWriteLine("Application Started -------------------");
            consoleout = (ConfigurationManager.AppSettings["CONSOLE_OUT"] == "yes");
            return true;
        }

        /// <summary>
        /// Write to the log file without a carriage return at the end
        /// </summary>
        /// <param name="msg"></param>
        public void LogWrite(string msg)
        {
            // track the status of the line - if its not open, lead with the timestamp
            if (!lineopen)
            {
                logstream.Write(DateTime.Now.ToString() + ": ");
                lineopen = true;
            }
            logstream.Write(msg);
        }

        /// <summary>
        /// Write a line of text to the log file
        /// </summary>
        /// <param name="msg"></param>
        public void LogWriteLine(string msg)
        {
            // output the timestamp at the beginning of a line
            if (!lineopen)
            {
                logstream.Write(DateTime.Now.ToString() + ": ");
            }
            // indicate that we are at the start of a line
            lineopen = false;
            // output the message text as the end of the line
            logstream.WriteLine(msg);
            // flush the output so we dont lose everything on a crash
            logstream.Flush();
        }

        /// <summary>
        /// Write a message to the console and the log
        /// </summary>
        /// <param name="msg"></param>
        public void Write(string msg)
        {
            LogWrite(msg);
            if (consoleout)
            {
                Console.Write(msg);
            }
        }

        /// <summary>
        /// Write a message to the console and log with a carriage return
        /// </summary>
        /// <param name="msg"></param>
        public void WriteLine(string msg)
        {
            LogWriteLine(msg);
            if (consoleout)
            {
                Console.WriteLine(msg);
            }
        }

        /// <summary>
        /// Close the log file
        /// </summary>
        public void Close()
        {
            lineopen = false;
            LogWriteLine("Application Closed -------------------");
            logstream.Close();
        }
    }
}
