using System;
using System.Diagnostics;

public abstract class ILog
{
	[Conditional("DEBUG")]
	public abstract void Debug(string message);
	[Conditional("DEBUG")]
	public abstract void Debug(string format, params object[] args);
	public abstract void Info(string message);
	public abstract void Info(string format, params object[] args);
	public abstract void Warn(string message);
	public abstract void Warn(string format, params object[] args);
	public abstract void Error(string message);
	public abstract void Error(string format, params object[] args);
}

public class Logger: ILog
{
	private string logger;

	private Logger(string type)
	{
		logger = type;
	}

	public static ILog GetLogger(string type)
	{
		return new Logger(type);
	}

	public static ILog GetLogger(Type type)
	{
		return new Logger(type.Namespace);
	}

	public static ILog GetLogger<TSource>()
	{
		return new Logger(typeof(TSource).Namespace);
	}

	#region ILog console implementation
	public override void Debug(string message)
	{
		Console.ForegroundColor = ConsoleColor.DarkGray;
		Console.Out.WriteLine(logger + ": " + message);
		Console.ResetColor();
	}

	public override void Debug(string format, params object[] args)
	{
		Console.ForegroundColor = ConsoleColor.DarkGray;
		Console.Out.WriteLine(logger + ": " + String.Format(format, args));
		Console.ResetColor();
	}

	public override void Info(string message)
	{
		Console.Out.WriteLine(logger + ": " + message);
	}

	public override void Info(string format, params object[] args)
	{
		Console.Out.WriteLine(logger + ": " + String.Format(format, args));
	}

	public override void Warn(string message)
	{
		Console.ForegroundColor = ConsoleColor.Yellow;
		Console.Out.WriteLine(logger + ": " + message);
		Console.ResetColor();
	}

	public override void Warn(string format, params object[] args)
	{
		Console.ForegroundColor = ConsoleColor.Yellow;
		Console.Out.WriteLine(logger + ": " + String.Format(format, args));
		Console.ResetColor();
	}

	public override void Error(string message)
	{
		Console.ForegroundColor = ConsoleColor.Red;
		Console.Error.WriteLine(logger + ": " + message);
		Console.ResetColor();
	}

	public override void Error(string format, params object[] args)
	{
		Console.ForegroundColor = ConsoleColor.Red;
		Console.Out.WriteLine(logger + ": " + String.Format(format, args));
		Console.ResetColor();
	}
	#endregion
}
