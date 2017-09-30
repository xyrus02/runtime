using System;
using JetBrains.Annotations;
using SlimDX.Direct3D11;

namespace XyrusWorx.Runtime 
{
	class DelegatedHardwareResourceList<T> : IResourcePool<IWritable>, IResourcePool<IReadable> where T: class
	{
		private readonly AcceleratedKernel mParent;
		private readonly Action<DeviceContext, T, int> mResourceSetter;
		private T[] mElements = new T[1024];

		internal DelegatedHardwareResourceList([NotNull] AcceleratedKernel parent, [NotNull] Action<DeviceContext, T, int> resourceSetter)
		{
			if (parent == null)
			{
				throw new ArgumentNullException(nameof(parent));
			}
			
			if (resourceSetter == null)
			{
				throw new ArgumentNullException(nameof(resourceSetter));
			}
			
			mParent = parent;
			mResourceSetter = resourceSetter;
		}

		public void Submit()
		{
			for (var i = 0; i < 1024; i++)
			{
				mResourceSetter(mParent.Device.ImmediateContext, mElements[i], i);
			}
		}

		public void Reset()
		{
			for (var i = 0; i < 1024; i++)
			{
				mElements[i] = null;
			}
		}

		private void AssignToSlot(int slot, T item)
		{
			if (slot >= 1024)
			{
				throw new ArgumentOutOfRangeException(nameof(slot));
			}
			
			mResourceSetter(mParent.Device.ImmediateContext, item, slot);
			mElements[slot] = item;
		}
		
		IWritable IResourcePool<IWritable>.this[int slot]
		{
			set => AssignToSlot(slot, value as T);
		}
		IReadable IResourcePool<IReadable>.this[int slot]
		{
			set => AssignToSlot(slot, value as T);
		}
	}
}