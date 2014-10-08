﻿using System;
using System.Diagnostics;

internal abstract class ILog
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

internal class Logger: ILog
{
	private static ILog logger = new Logger();

	private Logger()
	{
	}

	public static ILog GetLogger(string type)
	{
		return logger;
	}

	public static ILog GetLogger(Type type)
	{
		return logger;
	}

	#region ILog console implementation
	public override void Debug(string message)
	{
		Console.Out.WriteLine(message);
	}

	public override void Debug(string format, params object[] args)
	{
		Console.Out.WriteLine(format, args);
	}

	public override void Info(string message)
	{
		Console.Out.WriteLine(message);
	}

	public override void Info(string format, params object[] args)
	{
		Console.Out.WriteLine(format, args);
	}

	public override void Warn(string message)
	{
		Console.Out.WriteLine(message);
	}

	public override void Warn(string format, params object[] args)
	{
		Console.Out.WriteLine(format, args);
	}

	public override void Error(string message)
	{
		Console.Error.WriteLine(message);
	}

	public override void Error(string format, params object[] args)
	{
		Console.Out.WriteLine(format, args);
	}
	#endregion
}
