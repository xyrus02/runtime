using System;
using System.Linq;
using System.Runtime.InteropServices;
using Xunit;
using Xunit.Abstractions;

namespace XyrusWorx.Runtime.Data.Test
{
	public class UnmanagedBlockTests
	{
		private readonly ITestOutputHelper mOutput;

		public UnmanagedBlockTests(ITestOutputHelper output)
		{
			mOutput = output;
		}
		
		[Fact(DisplayName = "Copying a bunch of bytes from A to B")]
		public void Copy() => CopyAndAssertEquivalence(10, 0, 0, 10);
		
		[Fact(DisplayName = "Copying a bunch of bytes from A to B with source offset > 0")]
		public void CopyWithSourceOffsetGreaterThanZero() => CopyAndAssertEquivalence(10, 5, 0, 5);
		
		[Fact(DisplayName = "Copying a bunch of bytes from A to B with source offset > 0")]
		public void CopyWithTargetOffsetGreaterThanZero() => CopyAndAssertEquivalence(10, 0, 5, 5);
		
		private void CopyAndAssertEquivalence(int totalCount, int sourceOffset, int targetOffset, int copyCount)
		{
			IntPtr 
				adr1 = IntPtr.Zero, 
				adr2 = IntPtr.Zero;

			try
			{
				var input = Enumerable.Range(1, totalCount).Select(x => (byte)x).ToArray();
				
				adr1 = Load(input);
				adr2 = Load(new byte[totalCount]);
				
				UnmanagedBlock.Copy(adr1, adr2, sourceOffset, targetOffset, copyCount);

				var expected = new byte[totalCount];
				var actual = Fetch(adr2, input.Length);
				
				Array.Copy(input, sourceOffset, expected, targetOffset, copyCount);
				
				mOutput.WriteLine($" in: [ {string.Join(", ", input.Select(x => $"{x}"))} ]");
				mOutput.WriteLine($"out: [ {string.Join(", ", actual.Select(x => $"{x}"))} ]");
				mOutput.WriteLine($"ref: [ {string.Join(", ", expected.Select(x => $"{x}"))} ]");

				for (var i = 0; i < totalCount; i++)
				{
					Assert.Equal(expected[i], actual[i]);
				}
			}

			finally
			{
				if (adr1 != IntPtr.Zero) { Marshal.FreeHGlobal(adr1);}
				if (adr2 != IntPtr.Zero) { Marshal.FreeHGlobal(adr2);}
			}
		}
		private void CopyAndAssertEquivalence(int totalCount, int sourceOffset, int targetOffset, int copyCount, byte[] expected)
		{
			IntPtr 
				adr1 = IntPtr.Zero, 
				adr2 = IntPtr.Zero;

			try
			{
				var input = Enumerable.Range(1, totalCount).Select(x => (byte)x).ToArray();
				
				adr1 = Load(input);
				adr2 = Load(new byte[totalCount]);
				
				UnmanagedBlock.Copy(adr1, adr2, sourceOffset, targetOffset, copyCount);

				var actual = Fetch(adr2, input.Length);
				
				mOutput.WriteLine($" in: [ {string.Join(", ", input.Select(x => $"{x}"))} ]");
				mOutput.WriteLine($"out: [ {string.Join(", ", actual.Select(x => $"{x}"))} ]");

				for (var i = 0; i < totalCount; i++)
				{
					Assert.Equal(expected[i], actual[i]);
				}
			}

			finally
			{
				if (adr1 != IntPtr.Zero) { Marshal.FreeHGlobal(adr1);}
				if (adr2 != IntPtr.Zero) { Marshal.FreeHGlobal(adr2);}
			}
		}
		private void SimpleCopy(int totalCount, int sourceOffset, int targetOffset, int copyCount)
		{
			IntPtr 
				adr1 = IntPtr.Zero, 
				adr2 = IntPtr.Zero;

			try
			{
				var input = Enumerable.Range(1, totalCount).Select(x => (byte)x).ToArray();
				
				adr1 = Load(input);
				adr2 = Load(new byte[totalCount]);
				
				UnmanagedBlock.Copy(adr1, adr2, sourceOffset, targetOffset, copyCount);
			}

			finally
			{
				if (adr1 != IntPtr.Zero) { Marshal.FreeHGlobal(adr1);}
				if (adr2 != IntPtr.Zero) { Marshal.FreeHGlobal(adr2);}
			}
		}
		
		private IntPtr Load(byte[] data)
		{
			var ptr = Marshal.AllocHGlobal(data.Length);
			Marshal.Copy(data, 0, ptr, data.Length);
			return ptr;
		}
		private byte[] Fetch(IntPtr ptr, int count)
		{
			var bytes = new byte[count];
			Marshal.Copy(ptr, bytes, 0, count);
			return bytes;
		}
		private void Release(IntPtr ptr)
		{
			if (ptr == IntPtr.Zero)
			{
				return;
			}
			
			Marshal.FreeHGlobal(ptr);
		} 
	}
}
