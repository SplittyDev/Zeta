using System;
using System.Collections.Generic;
using System.IO;

namespace libzeta {

    /// <summary>
    /// Logging extensions.
    /// </summary>
    public static class Logging {

        /// <summary>
        /// The logging colors.
        /// </summary>
        static readonly Dictionary<LoggingLevel, ConsoleColor> colors
            = new Dictionary<LoggingLevel, ConsoleColor> {
            [LoggingLevel.INFO] = ConsoleColor.Gray,
            [LoggingLevel.WARN] = ConsoleColor.Yellow,
            [LoggingLevel.ERROR] = ConsoleColor.Red
        };

        /// <summary>
        /// The synchronization root.
        /// </summary>
        static readonly object syncRoot = new object ();

        /// <summary>
        /// Log the specified mesage.
        /// </summary>
        /// <param name="level">Level.</param>
        /// <param name="message">Message.</param>
        public static void Log (LoggingLevel level, string message, bool writeLine = true) {
#if DEBUG
            LogInternal (level, message, writeLine);
#endif
        }

        /// <summary>
        /// Log the specified message.
        /// </summary>
        /// <param name="level">Level.</param>
        /// <param name="message">Message.</param>
        /// <param name="callback">Callback.</param>
        public static void Log (LoggingLevel level, string message, Action callback) {
            var result = "OK";
            var rethrow = false;
            var ex = default (Exception);
            Log (level, message);
            try {
                callback ();
            } catch (Exception e) {
                level = LoggingLevel.ERROR;
                result = "FAIL";
                ex = e;
                rethrow = true;
            }
            Log (level, $"{message} {result}");
            if (ex != default (Exception)) {
                Log (LoggingLevel.ERROR, ex.Message);
                Log (LoggingLevel.INFO, ex.StackTrace);
            }
            if (rethrow) {
                Console.ReadKey ();
                Environment.Exit (1);
            }
        }

        /// <summary>
        /// Log the specified message.
        /// </summary>
        /// <param name="dummy">Dummy.</param>
        /// <param name="level">Level.</param>
        /// <param name="message">Message.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static void Log<T> (this T dummy, LoggingLevel level, string message) where T : class {
            Log (level, message);
        }

        /// <summary>
        /// Log the specified message.
        /// </summary>
        /// <param name="dummy">Dummy.</param>
        /// <param name="level">Level.</param>
        /// <param name="message">Message.</param>
        /// <param name="callback">Callback.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static void Log<T> (this T dummy, LoggingLevel level, string message, Action callback) where T : class {
            Log (level, message, callback);
        }

        static TextWriter GetWriter (LoggingLevel level) {
            TextWriter writer;
            switch (level) {
            case LoggingLevel.INFO:
                writer = Console.Out;
                break;
            case LoggingLevel.WARN:
                writer = Console.Out;
                break;
            case LoggingLevel.ERROR:
                writer = Console.Error;
                break;
            default:
                writer = Console.Out;
                break;
            }
            return writer;
        }

        static void LogInternal (LoggingLevel level, string message, bool writeLine = true) {
            var color = Console.ForegroundColor;
            var logcolor = colors [level];
            var writer = GetWriter (level);
            lock (syncRoot) {
                Console.ForegroundColor = logcolor;
                writer.Write ($"{level,-5}");
                Console.ForegroundColor = color;
                writer.Write ($" :: {message}{(writeLine ? "\n" : "")}");
            }
        }

    }
}

