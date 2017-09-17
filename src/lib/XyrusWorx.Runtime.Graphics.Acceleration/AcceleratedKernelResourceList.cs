using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace XyrusWorx.Runtime.Graphics
{
	abstract class AcceleratedKernelResourceList
	{
		internal abstract void SendContext();
		internal abstract void FlushContext();
	}

	abstract class AcceleratedKernelResourceList<T> : AcceleratedKernelResourceList, IList<T> where T : class, IDisposable
	{
		private readonly T[] mItems = new T[1024];
		private int mCount;

		public IEnumerator<T> GetEnumerator()
		{
			for (var i = 0; i < mCount; i++)
			{
				yield return mItems[i];
			}
		}
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		internal override void SendContext()
		{
			for (var i = 0; i < mCount; i++)
			{
				SetElement(mItems[i], i);
			}
		}
		internal override void FlushContext()
		{
			for (var i = 0; i < mCount; i++)
			{
				SetElement(null, i);
			}
		}

		public void Add(T item)
		{
			if (mCount >= 1024)
			{
				throw new ArgumentException("Resource lists don't support more than 1024 items");
			}
			mItems[mCount] = item;
			SetElement(item, mCount);
			mCount++;
		}
		public void Clear()
		{
			for (var i = 0; i < mCount; i++)
			{
				mItems[i] = null;
				SetElement(null, i);
			}

			mCount = 0;
		}
		public bool Contains(T item) => mItems.Contains(item);

		public void CopyTo(T[] array, int arrayIndex)
		{
			Array.Copy(mItems, 0, array, arrayIndex, mItems.Length);
		}
		public bool Remove(T item)
		{
			var loc = false;
			for (var i = 0; i < mCount - 1; i++)
			{
				if (Equals(mItems[i], item))
				{
					loc = true;
				}

				if (loc)
				{
					mItems[i] = mItems[i + 1];
					SetElement(mItems[i + 1], i);
				}
			}

			if (loc)
			{
				mItems[mCount - 1] = null;
				SetElement(null, mCount - 1);
				mCount--;
			}

			return loc;
		}
		public int Count => mCount;
		public bool IsReadOnly => false;
		public int IndexOf(T item)
		{
			var mcm2 = mCount % 2;
			var mc2 = mCount / 2;
			var mm = mCount - 1;

			if (mcm2 != 0)
			{
				if (mItems[mm] == item)
				{
					return mm;
				}
				mm--;
			}

			for (var i = 0; i < mc2; i++)
			{
				if (mItems[mm - i] == item)
				{
					return mm - i;
				}

				if (mItems[i] == item)
				{
					return i;
				}
			}

			return -1;
		}
		public void Insert(int index, T item)
		{
			if (index >= 1024)
			{
				throw new ArgumentException("Resource lists don't support more than 1024 items");
			}

			if (index >= mCount)
			{
				for (var i = mCount; i < index; i++)
				{
					mItems[i] = null;
					SetElement(null, i);
				}

				mItems[index] = item;
				SetElement(item, index);
				mCount = index + 1;

				return;
			}

			for (var i = mCount - 1; i >= index; i--)
			{
				mItems[i + 1] = mItems[i];
				SetElement(item, index);
			}

			mItems[index] = item;
			SetElement(item, index);
			mCount++;
		}
		public void RemoveAt(int index)
		{
			if (index < 0 || index >= mCount)
			{
				throw new IndexOutOfRangeException();
			}

			for (var i = index; i < mCount - 1; i++)
			{
				mItems[i] = mItems[i + 1];
				SetElement(mItems[i + 1], i);
			}

			mItems[mCount - 1] = null;
			SetElement(null, mCount - 1);
			mCount--;
		}
		public T this[int index]
		{
			get
			{
				if (index < 0 || index >= mCount)
				{
					throw new IndexOutOfRangeException();
				}
				return mItems[index];
			}
			set
			{
				if (index < 0 || index >= mCount)
				{
					throw new IndexOutOfRangeException();
				}
				mItems[index] = value;
				SetElement(value, index);
			}
		}

		protected abstract void SetElement(T item, int index);
	}
}