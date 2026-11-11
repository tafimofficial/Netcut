using System;

namespace NetStalker;

public class CustomExceptions
{
	public class NoScanStartedException : Exception
	{
		public NoScanStartedException()
		{
		}

		public NoScanStartedException(string message)
			: base(message)
		{
		}
	}

	public class NoDeviceSelectedException : Exception
	{
		public NoDeviceSelectedException()
		{
		}

		public NoDeviceSelectedException(string message)
			: base(message)
		{
		}
	}

	public class ArpStateFalse : Exception
	{
		public ArpStateFalse()
		{
		}

		public ArpStateFalse(string message)
			: base(message)
		{
		}
	}

	public class OperationInProgressException : Exception
	{
		public OperationInProgressException()
		{
		}

		public OperationInProgressException(string message)
			: base(message)
		{
		}
	}

	public class RedirectionNotActiveException : Exception
	{
		public RedirectionNotActiveException()
		{
		}

		public RedirectionNotActiveException(string message)
			: base(message)
		{
		}
	}

	public class LimiterActiveException : Exception
	{
		public LimiterActiveException()
		{
		}

		public LimiterActiveException(string message)
			: base(message)
		{
		}
	}

	public class DeviceNotInListException : Exception
	{
		public DeviceNotInListException()
		{
		}

		public DeviceNotInListException(string message)
			: base(message)
		{
		}
	}

	public class LocalHostTargeted : Exception
	{
		public LocalHostTargeted()
		{
		}

		public LocalHostTargeted(string message)
			: base(message)
		{
		}
	}

	public class GatewayTargeted : Exception
	{
		public GatewayTargeted()
		{
		}

		public GatewayTargeted(string message)
			: base(message)
		{
		}
	}
}
