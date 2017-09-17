using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Native
{
	[PublicAPI]
	public abstract class HardwareResource : IDisposable
	{
		private bool mDisposedValue;

		~HardwareResource()
		{
			Dispose(false);
		}

		protected virtual void OnCleanup()
		{
		}
		protected virtual void OnFinalize()
		{
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		private void Dispose(bool disposing)
		{
			if (!mDisposedValue)
			{
				if (disposing)
				{
					try
					{
						OnCleanup();
					}
					catch (Exception e)
					{
						Trace.TraceError(e.ToString());
						Debug.Assert(false);
					}
				}

				try
				{
					OnFinalize();
				}
				catch (Exception e)
				{
					Trace.TraceError(e.ToString());
					Debug.Assert(false);
				}
				mDisposedValue = true;
			}
		}
	}
}