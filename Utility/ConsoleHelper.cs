using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class ConsoleHelper
    {
        /// <summary>
        /// 写入控制台
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="message">消息</param>
        /// <param name="subTitle">副标题</param>
        /// <param name="consoleColor">颜色</param>
        public static void Write(string title, string message, ConsoleColor consoleColor)
        {
            Console.ForegroundColor = consoleColor;
            string dateNowTxt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            Console.Write( $"[{dateNowTxt}]{title}:{message}");
            Console.ForegroundColor = ConsoleColor.White;
        }
        /// <summary>
        /// 写入控制台
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="message">消息</param>
        /// <param name="subTitle">副标题</param>
        /// <param name="consoleColor">颜色</param>
        public static void WriteLine(string title, string message, ConsoleColor consoleColor=ConsoleColor.White)
        {
            Console.ForegroundColor = consoleColor;
            string dateNowTxt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            Console.WriteLine($"[{dateNowTxt}]{title}:{message}");
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static void WriteErrorLine(string title, string message, ConsoleColor consoleColor = ConsoleColor.Red)
        {
            Console.ForegroundColor = consoleColor;
            string dateNowTxt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            Console.WriteLine($"[{dateNowTxt}]{title}:{message}");
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static void ServerWriteError(Exception exception)
        {
            WriteErrorLine("Server Error -> ",GetErrorMessage(exception), ConsoleColor.Red);
        }
        #region 私有方法
        /// <summary>
        /// 获得错误消息
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        private static string GetErrorMessage(Exception exception)
        {
            string message = $"{exception.Message}";
            if (!string.IsNullOrEmpty(exception.StackTrace)) message += $"\r\n{exception.StackTrace}";
            if (exception is AggregateException aggregateException)
            {
                foreach (Exception innerException in aggregateException.InnerExceptions)
                {
                    Exception ex = innerException;
                    do
                    {
                        message += $"\r\n{GetErrorMessage(exception)}";
                        ex = innerException.InnerException;
                    } while (ex != null);
                }
            }
            else
            {
                while (exception.InnerException != null)
                {
                    exception = exception.InnerException;
                    message += $"\r\n{GetErrorMessage(exception)}";
                }
            }
            return message;
        }
        #endregion
    }
}
