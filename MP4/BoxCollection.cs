//------------------------------------------------------------------------------
// Copyright © FRA & FV 2014
// All rights reserved
//------------------------------------------------------------------------------
// MPEG-4 boxes Framework
//
// SVN revision information:
//   $Revision$
//------------------------------------------------------------------------------
using System;
using System.Linq;
using System.Collections.Generic;

namespace MP4
{
	public abstract class Collection<TBase> : ICollection<TBase>
	{
		protected List<TBase> list;

		protected virtual IEnumerable<TBase> Elements { get { return this.list; } }

		#region Constructors
		protected Collection()
		{
			this.list = new List<TBase>();
		}

		protected Collection(List<TBase> collection)
		{
			this.list = collection;
		}
		#endregion

		#region ICollection implementation
		public virtual void Add(TBase item)
		{
			list.Add(item);
		}

		public virtual void Clear()
		{
			list.Clear();
		}

		public bool Contains(TBase item)
		{
			return Elements.Contains(item);
		}

		protected void CopyElementsTo<TElement>(TElement[] array, int arrayIndex)
			where TElement : TBase
		{
			Elements.Cast<TElement>().ToArray().CopyTo(array, arrayIndex);
		}

		void ICollection<TBase>.CopyTo(TBase[] array, int arrayIndex)
		{
			Elements.ToArray().CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return Elements.Count(); }
		}

		bool ICollection<TBase>.IsReadOnly
		{
			get { return false; }
		}

		public virtual bool Remove(TBase item)
		{
			return list.Remove(item);
		}

		IEnumerator<TBase> IEnumerable<TBase>.GetEnumerator()
		{
			return Elements.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return Elements.GetEnumerator();
		}
		#endregion
	}

	public sealed class BoxCollection : Collection<AtomicInfo>
	{
		#region Constructor
		public BoxCollection() : base(new List<AtomicInfo>()) { }
		#endregion
	}

	public sealed class BoxCollection<TBox> : Collection<AtomicInfo>
		where TBox : AtomicInfo
	{
		#region Constructor
		public BoxCollection() : base(new List<AtomicInfo>()) { }
		#endregion

		#region ICollection implementation
		public override void Add(AtomicInfo item)
		{
			if (!(item is TBox))
				throw new InvalidCastException("Expected box type is " + typeof(TBox).Name);
			base.Add(item);
		}

		public void Add(TBox box)
		{
			Add((AtomicInfo)box);
		}

		public bool Contains(TBox box)
		{
			return base.Contains(box);
		}

		public bool Remove(TBox box)
		{
			return base.Remove(box);
		}
		#endregion
	}

	public sealed class EntryCollection<TEntry, TOwner> : Collection<TEntry>
		where TEntry: IEntry<TOwner>
	{
		private TOwner owner;

		#region Constructor
		public EntryCollection(TOwner owner)
		{
			this.owner = owner;
		}

		public EntryCollection() { }
		#endregion

		#region ICollection implementation
		public override void Add(TEntry item)
		{
			item.Owner = owner;
			base.Add(item);
		}
		#endregion
	}
}
