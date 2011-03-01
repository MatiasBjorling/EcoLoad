using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using log4net;
using log4net.Config;

[assembly : XmlConfigurator]
[assembly : Repository]

namespace EcoManager.Shared.Tools
{
    /// <summary>
    /// Implements simple centralized logging for DRF.
    /// </summary>
    public class Logger
    {
        /// <summary>
        /// Instance of the current logger.
        /// </summary>
        private static volatile Logger _current = null;

        private static readonly object SyncRoot = new object();
        
        /// <summary>
        /// Logger object from Log4net. Simple usage.
        /// </summary>
        private readonly ILog log = LogManager.GetLogger("Logging");

        private Logger() {}

        /// <summary>
        /// Gets current logger instance.
        /// </summary>
        public static Logger Current
        {
            get
            {
                if (_current == null)
                {
                    lock (SyncRoot)
                    {
                        if (_current == null)
                            _current = new Logger();
                    }
                }

                return _current;
            }
        }

        /// <summary>
        /// Puts string as debugging information to logging.
        /// </summary>
        /// <param name="message"></param>
        public static void Debug(string message)
        {
            if (Current.log.IsDebugEnabled)
                Current.log.Debug("[" + DateTime.Now + "] " + ExtractInfo(message));
        }

        /// <summary>
        /// /// Puts string as info information to logging.
        /// </summary>
        /// <param name="message"></param>
        public static void Info(string message)
        {
            Console.WriteLine("[" + DateTime.Now + "] " + ExtractInfo(message));
            if (Current.log.IsInfoEnabled)
                Current.log.Info("[" + DateTime.Now + "] " + ExtractInfo(message));
        }

        /// <summary>
        /// Puts string as error information to logging.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="e"></param>
        public static void Error(string message, System.Exception e)
        {
            if (Current.log.IsErrorEnabled)
                Current.log.Error("[" + DateTime.Now + "] " + ExtractInfo(message), e);
        }

        /// <summary>
        /// Puts string as debugging information to logging.
        /// </summary>
        /// <param name="message"></param>
        public static void Error(string message)
        {
            if (Current.log.IsErrorEnabled)
                Current.log.Error("[" + DateTime.Now + "] " + ExtractInfo(message));
        }

        /// <summary>
        /// Gets the second pointer in the stack of the problem. That holds the information what class it called from.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private static string ExtractInfo(string message)
        {
            StackFrame frame1 = new StackFrame(2, true);

            string methodName = frame1.GetMethod().ToString();
            string fileName = Path.GetFileName(frame1.GetFileName());
            string[] textArray1 = new string[6] {"File:", fileName, " - Method:", methodName, " - ", message};

            return string.Concat(textArray1);
        }

        /// <summary>
        /// Prints a message box to the screen.
        /// </summary>
        /// <param name="msg"></param>
        /// <remarks>Made this way if some other compatible would be showed instead in some far far away future.</remarks>
        public static void Message(string msg)
        {
            MessageBox.Show(msg);
        }
    }
}