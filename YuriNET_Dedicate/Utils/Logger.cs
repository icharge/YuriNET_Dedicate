using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YuriNET.Dedicated.CoreServer;

namespace YuriNET.Utils {
    /// <summary>
    /// Print Any logs output by level
    /// </summary>
    public class Logger {

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        private Logger(string name) {
            className = name;
        }

        private string className;

        public enum Level {
            Fatal = 1,
            Error = 2,
            Warn = 4,
            Info = 8,
            Debug = 16,
            All = ~0
        };

        public static Level DisplayLevel = Level.All;
        public static bool WriteToFile = true;
        public static string LogFileName = Info.getProductName() + ".log";
        public static int LogFileSizeLimit = 0;

        private static StreamWriter Writer = File.AppendText(string.Format(@"{0}\{1}",
                Info.getAppFolder(), LogFileName));
        private static Logger _instance = new Logger("Log");

        public static Logger getInstance() {
            return _instance;
        }

        public static Logger getInstance(Type T) {
            return new Logger(T.Name);
        }

        public void ShowConsole() {
            AllocConsole();
        }
        public void info(Object str, params Object[] arg) {
            lock (this) {
                if ((DisplayLevel & Level.Info) == Level.Info) {
                    color(ConsoleColor.White).write("[" + className + "] ").color().write(DateTime.Now + " ")
                        .color(ConsoleColor.Green)
                        .write("[INFO]").color().writeLine(" : " + str, arg);
                }
            }
        }
        public void debug(Object str, params Object[] arg) {
            lock (this) {
                if ((DisplayLevel & Level.Debug) == Level.Debug) {
                    color(ConsoleColor.White).write("[" + className + "] ").color().write(DateTime.Now + " ")
                        .bgColor(ConsoleColor.Gray).color(ConsoleColor.Black)
                        .write("[DEBUG]").color().writeLine(" : " + str, arg);
                }
            }
        }
        public void warn(Object str, params Object[] arg) {
            lock (this) {
                if ((DisplayLevel & Level.Warn) == Level.Warn) {
                    color(ConsoleColor.White).write("[" + className + "] ").color().write(DateTime.Now + " ")
                        .bgColor(ConsoleColor.Yellow).color(ConsoleColor.Black)
                        .write("[WARN]").color().writeLine(" : " + str, arg);
                }
            }
        }
        public void error(Object str, params Object[] arg) {
            lock (this) {
                if ((DisplayLevel & Level.Error) == Level.Error) {
                    color(ConsoleColor.White).write("[" + className + "] ").color().write(DateTime.Now + " ")
                        .bgColor(ConsoleColor.Red).color(ConsoleColor.Black)
                        .write("[ERROR]").color().writeLine(" : " + str, arg);
                }
            }
        }
        public void fatal(Object str, params Object[] arg) {
            lock (this) {
                if ((DisplayLevel & Level.Fatal) == Level.Fatal) {
                    color(ConsoleColor.White).write("[" + className + "] ").color().color(ConsoleColor.Red).write(DateTime.Now + " ")
                        .bgColor(ConsoleColor.Red).color(ConsoleColor.Black)
                        .write("[FATAL]").color().color(ConsoleColor.Red).writeLine(" : " + str, arg);
                }
            }
        }
        public Logger write(Object str, params Object[] arg) {
            Console.Write("" + str, arg);
            (Writer as TextWriter).Write("" + str, arg);
            Writer.Flush();
            return this;
        }
        public Logger writeLine(Object str, params Object[] arg) {
            Console.WriteLine("" + str, arg);
            (Writer as TextWriter).WriteLine("" + str, arg);
            Writer.Flush();
            return this;
        }
        public Logger color() {
            Console.ResetColor();
            return this;
        }
        public Logger color(ConsoleColor color) {
            Console.ForegroundColor = color;
            return this;
        }
        public Logger bgColor(ConsoleColor color) {
            Console.BackgroundColor = color;
            return this;
        }
    }
}
