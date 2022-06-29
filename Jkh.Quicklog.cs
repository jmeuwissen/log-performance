using System;
using System.Diagnostics;	//EventLog
using System.IO;	//StreamWriter
using System.Reflection;	//Assembly


namespace Jkh.Logging
{
	public class QuickLog
	{
		public enum LogLevel
		{
			Debug = 0,	// I know, 0 is assumed anyway
			Info,
			Warning,
			Exception,
			Error,
			Fatal
		};

		protected static string _logFilePath = null;
		public static string LogFilePath
		{
			get
			{
				if(string.IsNullOrEmpty(_logFilePath))
					_logFilePath = GenerateDefaultLogFileName();
				return _logFilePath;
			}
			set
			{
				_logFilePath = value;
			}
		}

		public static string GenerateDefaultLogFileName()
		{
			//return Assembly.GetExecutingAssembly().Location + "-" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + ".log";
			// works in Forms or console, as a DLL it would get the EXE's path (not your DLLs!)
			return System.Reflection.Assembly.GetEntryAssembly().Location + "-" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + ".log";
			//return Application.ExecutablePath + "-" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + ".log";
			//return AppDomain.CurrentDomain.BaseDirectory + "\\" + DateTime.Now.Year + "-" + DateTime.Now.Month + "_" + DateTime.Now.Day + ".log";
		}

		public static void WriteToLogFormat(LogLevel logLevel, string text, params object[] obj)
		{
			if(string.IsNullOrEmpty(_logFilePath))
			{
				_logFilePath = GenerateDefaultLogFileName();
				Assembly ThisAssembly = System.Reflection.Assembly.GetEntryAssembly();	//Get the name of the EXE
				AssemblyName ThisAssemblyName = ThisAssembly.GetName();
				WriteToLogFormat(LogLevel.Info, "Startup: {0}, ver={1}", ThisAssembly.Location, ThisAssemblyName.Version.ToString());
			}

			Debug.WriteLine("LOG>" + string.Format(text, obj));
			try
			{
				using(StreamWriter s = File.AppendText(_logFilePath))
				{
					s.WriteLine(DateTime.Now.ToString("HH:mm:ss") + "\t" + ((int)logLevel).ToString() + "\t" + string.Format(text, obj).Replace("\r\n", "\\r\\n"));
				}
			}
			catch(Exception exc)
			{
				Debug.WriteLine(exc.Message);
			}
		}
		public static void WriteToLogFormatMS(LogLevel logLevel, string text, params object[] obj)
		{
			if (string.IsNullOrEmpty(_logFilePath))
			{
				_logFilePath = GenerateDefaultLogFileName();
				Assembly ThisAssembly = System.Reflection.Assembly.GetEntryAssembly();  //Get the name of the EXE
				AssemblyName ThisAssemblyName = ThisAssembly.GetName();
				WriteToLogFormat(LogLevel.Info, "Startup: {0}, ver={1}", ThisAssembly.Location, ThisAssemblyName.Version.ToString());
			}

			Debug.WriteLine("LOG>" + string.Format(text, obj));
			try
			{
				using (StreamWriter s = File.AppendText(_logFilePath))
				{
					s.WriteLine(DateTime.Now.ToString("HH:mm:ss:fff") + "\t" + ((int)logLevel).ToString() + "\t" + string.Format(text, obj).Replace("\r\n", "\\r\\n"));
				}
			}
			catch (Exception exc)
			{
				Debug.WriteLine(exc.Message);
			}
		}
		public static void WriteToLogFormatMicro(LogLevel logLevel, string text, params object[] obj)
		{
			if (string.IsNullOrEmpty(_logFilePath))
			{
				_logFilePath = GenerateDefaultLogFileName();
				Assembly ThisAssembly = System.Reflection.Assembly.GetEntryAssembly();  //Get the name of the EXE
				AssemblyName ThisAssemblyName = ThisAssembly.GetName();
				WriteToLogFormat(LogLevel.Info, "Startup: {0}, ver={1}", ThisAssembly.Location, ThisAssemblyName.Version.ToString());
			}

			Debug.WriteLine("LOG>" + string.Format(text, obj));
			try
			{
				using (StreamWriter s = File.AppendText(_logFilePath))
				{
					s.WriteLine(DateTime.Now.ToString("HH:mm:ss:ffffff") + "\t" + ((int)logLevel).ToString() + "\t" + string.Format(text, obj).Replace("\r\n", "\\r\\n"));
				}
			}
			catch (Exception exc)
			{
				Debug.WriteLine(exc.Message);
			}
		}


		public static void WriteToLog(LogLevel logLevel, string text)
		{
			if(string.IsNullOrEmpty(_logFilePath))
				_logFilePath = GenerateDefaultLogFileName();

			Debug.WriteLine("LOG>" + text);
			try
			{
				using(StreamWriter s = File.AppendText(_logFilePath))
				{
					s.WriteLine(DateTime.Now.ToString("HH:mm:ss") + "\t" + ((int)logLevel).ToString() + "\t" + text.Replace("\r\n", "\\r\\n"));
				}
			}
			catch(Exception exc)
			{
				Debug.WriteLine(exc.Message);
			}
		}

		public static void WriteMethod(Exception exc)
		{
			StackTrace stackTrace = new StackTrace();
			MethodBase methodBase = stackTrace.GetFrame(1).GetMethod();
			string method = methodBase.DeclaringType.Name + "." + methodBase.Name;
			if(exc != null)
			{
				WriteToLogFormat(LogLevel.Exception, "{0}: {1}", method, exc.Message);
			}
			else
			{
				WriteToLogFormat(LogLevel.Exception, null, method + ": NULL exception?");
				Debug.Assert(false);	// Developer: why?
			}
		}

		#region MAIN writer
		public static void Write(LogLevel ll, string text)
		{
			WriteToLog(ll, text);
		}

		public static void Write(LogLevel ll, string text, params object[] objs)
		{
			WriteToLogFormat(ll, text, objs);
		}
		public static void WriteMS(LogLevel ll, string text, params object[] objs)
		{
			WriteToLogFormatMS(ll, text, objs);
		}
		public static void WriteMicro(LogLevel ll, string text, params object[] objs)
		{
			WriteToLogFormatMicro(ll, text, objs);
		}

		#endregion MAIN writer

		#region Various Accessors
		public static void Write(Exception exc, LogLevel ll, string text)
		{
			Write(ll, text + ": " + exc.Message);
		}

		public static void Write(Exception exc, LogLevel ll, string text, params object[] objs)
		{
            string formattedText = string.Format(text, objs);

			string exceptionText = string.Format("{0} message={1} {2}", exc.GetType().ToString(), exc.Message, exc.StackTrace.ToString());

			Write(ll, formattedText + ": " + exceptionText);
		}

		public static void Write(string text, params object[] objs)
		{
			WriteToLogFormat(LogLevel.Info, text, objs);
		}

		public static void Write(Exception exc, string text, params object[] objs)
		{
			Write(exc, LogLevel.Exception, text, objs);
		}

		public static void ErrorException(Exception exc, string text, params object[] objs)
		{
			Write(exc, LogLevel.Error, text, objs);
		}

		public static void WriteFatal(string text, params object[] objs)
		{
			Write(LogLevel.Fatal, text, objs);
		}

		public static void WriteError(string text, params object[] objs)
		{
			Write(LogLevel.Error, text, objs);
		}

		public static void WriteWarning(string text, params object[] objs)
		{
			Write(LogLevel.Warning, text, objs);
		}

		public static void WriteInfo(string text, params object[] objs)
		{
			Write(LogLevel.Info, text, objs);
		}
		public static void WriteMSInfo(string text, params object[] objs)
		{
			WriteMS(LogLevel.Info, text, objs);
		}
		public static void WriteMicroInfo(string text, params object[] objs)
		{
			WriteMicro(LogLevel.Info, text, objs);
		}
		public static void WriteDebug(string text, params object[] objs)
		{
			Write(LogLevel.Debug, text, objs);
		}
		#endregion Various Accessors

		/// <summary>Writes a message to the application event log</summary>
		/// <param name="Source">Source is the source of the message usually you will want this to be the application name</param>
		/// <param name="Message">message to be written</param>
		/// <param name="EntryType">the entry type to use to categorize the message like for example error or information</param>
		public static void WriteToEventLog(string Source, string Message, EventLogEntryType EntryType)
		{
			try
			{
				if(!EventLog.SourceExists(Source))
					EventLog.CreateEventSource(Source, "Application");
				EventLog.WriteEntry(Source, Message, EntryType);
			}
			catch(Exception exc)
			{
				Debug.WriteLine(exc.Message);
			}
		}
	}
}
