using System;
using JetBrains.Annotations;
using SlimDX.Direct3D11;

namespace XyrusWorx.Runtime 
{
	class DelegatedHardwareResourceList<T> : DelegatedResourceList<T>, IResourcePool<IWritable>, IResourcePool<IReadable> where T: class
	{
		private readonly AcceleratedKernel mParent;
		private readonly Action<DeviceContext, T, int> mResourceSetter;

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

		protected override void SetElement(T item, int index)
		{
			mResourceSetter(mParent.Device.ImmediateContext, item, index);
		}
		
		IWritable IResourcePool<IWritable>.this[int slot]
		{
			set => SetElement(value as T, slot);
		}
		IReadable IResourcePool<IReadable>.this[int slot]
		{
			set => SetElement(value as T, slot);
		}
	}
}