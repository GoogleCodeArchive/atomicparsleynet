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
	public abstract class TypedList<TBase>: Collection<TBase>
		where TBase: class
	{
		private static readonly ILog log = Logger.GetLogger(typeof(TypedList<TBase>));

		private class TypeBag
		{
			Type type;
			bool multi;
			int count;

			public TypeBag(Type type, bool multi)
			{
				this.type = type;
				this.multi = multi;
			}

			protected virtual bool IsAssignable(Type element)
			{
				return type.IsAssignableFrom(element);
			}

			protected virtual bool IsEqual(Type element)
			{
				return element == type;
			}

			protected virtual void Add(List<TBase> list, int index, TBase element)
			{
				if (count > 0 && !multi)
					throw new InvalidOperationException(String.Format("Box type {0} already exists", type));
				list.Insert(index + count, element);
				count++;
			}

			protected bool Remove(List<TBase> list, int index, TBase element)
			{
				int k = list.IndexOf(element, index, count);
				if (k < 0) return false;
				list.RemoveAt(k);
				count--;
				return true;
			}

			public void Clear()
			{
				count = 0;
			}

			private static bool FindPlace(TypeBag[] types, Type type, out int index, out TypeBag place)
			{
				TypeBag bag, bag2;
				for (int i = 0, k = 0; i < types.Length; i++, k += bag.count)
				{
					bag = types[i];
					if (bag.IsAssignable(type))
					{
						for (int i2 = i, k2 = k; i2 < types.Length; i2++, k2 += bag2.count)
						{
							bag2 = types[i2];
							if (bag2.IsEqual(type))
							{
								place = bag2;
								index = k2;
								return true;
							}
						}
						place = bag;
						index = k;
						return true;
					}
				}
				place = null;
				index = 0;
				return false;
			}

			public static TBase Get(List<TBase> list, TypeBag[] types, Type type, bool create)
			{
				int k;
				TypeBag bag;
				if (!FindPlace(types, type, out k, out bag))
					return null;
				if (bag.multi)
					throw new InvalidOperationException("Single type isn't allowed for " + type);
				if (bag.count == 1)
					return list[k];
				if (!create) return null;
				var item = (TBase)Activator.CreateInstance(type);
				bag.Add(list, k, item);
				return item;
			}

			public static IEnumerable<TBase> OfType(List<TBase> list, TypeBag[] types, Type type)
			{
				int k;
				TypeBag bag;
				if (!FindPlace(types, type, out k, out bag))
					yield break;
				for (int i = 0; i < bag.count; i++)
					yield return list[k + i];
			}

			public static void Set(List<TBase> list, TypeBag[] types, Type type, TBase item)
			{
				if (type != null && !type.IsAssignableFrom(item.GetType()))
					throw new ArgumentException("Expected type is " + type.Name);
				int k;
				TypeBag bag;
				if (!FindPlace(types, type, out k, out bag))
					new InvalidOperationException(String.Format("Box {0} unknown type", item.GetType()));
				bag.Add(list, k, item);
			}

			public static bool Remove(List<TBase> list, TypeBag[] types, TBase item)
			{
				int k = list.IndexOf(item);
				if (k < 0) return false;
				int index = k;
				foreach (var bag in types)
				{
					if (index < bag.count)
						return bag.Remove(list, k - index, item);
					index -= bag.count;
				}
				return false;
			}
		}

		private class UnknownTypeBag : TypeBag
		{
			public UnknownTypeBag() : base(typeof(object), true) { }

			protected override bool IsAssignable(Type element)
			{
				return true;
			}

			protected override bool IsEqual(Type element)
			{
				return false;
			}

			protected override void Add(List<TBase> list, int index, TBase element)
			{
				log.Warn("{0} has unknown type for {1}", element.GetType(), element);
				base.Add(list, index, element);
			}
		}

		private class AnyTypeBag : TypeBag
		{
			private Type[] types;

			public AnyTypeBag(Type[] types, bool multi)
				: base(null, multi)
			{
				this.types = types;
			}

			protected override bool IsAssignable(Type element)
			{
				return types.Any(type => type.IsAssignableFrom(element));
			}

			protected override bool IsEqual(Type element)
			{
				return types.Contains(element);
			}
		}

		public abstract class OrBase
		{
			internal OrBase() { }
		}

		public abstract class AnyBase
		{
			internal AnyBase() { }
		}

		private readonly TypeBag[] types;

		public TBase this[Type type]
		{
			get { return TypeBag.Get(list, types, type, true); }
			set { TypeBag.Set(list, types, type, value); }
		}

		public TBase this[Type type, bool create]
		{
			get { return TypeBag.Get(list, types, type, create); }
		}

		#region Constructor
		public TypedList(bool unknown, params Type[] order)
		{
			var types = new List<TypeBag>();
			foreach (var type in order)
			{
				if (type.IsArray)
					types.Add(new TypeBag(type.GetElementType(), true));
				else if (typeof(OrBase).IsAssignableFrom(type))
					types.Add(new AnyTypeBag(type.GetGenericArguments(), false));
				else if (typeof(AnyBase).IsAssignableFrom(type))
					types.Add(new AnyTypeBag(type.GetGenericArguments(), true));
				else
					types.Add(new TypeBag(type, false));
			}
			types.Add(new UnknownTypeBag());
			this.types = types.ToArray();
		}
		#endregion

		#region Box accessors
		public TItem Get<TItem>(bool create = true) where TItem : TBase, new()
		{
			return (TItem)TypeBag.Get(list, types, typeof(TItem), create);
		}

		public TItem[] ArrayOfType<TItem>() where TItem : TBase
		{
			return TypeBag.OfType(list, types, typeof(TItem)).Cast<TItem>().ToArray();
		}

		public IEnumerable<TItem> OfType<TItem>() where TItem : TBase
		{
			return TypeBag.OfType(list, types, typeof(TItem)).Cast<TItem>();
		}

		public void Set<TItem>(TItem item) where TItem : TBase
		{
			TypeBag.Set(list, types, typeof(TItem), item);
		}
		#endregion

		#region ICollection implementation
		public override void Add(TBase item)
		{
			TypeBag.Set(list, types, item.GetType(), item);
		}

		public override void Clear()
		{
			list.Clear();
			foreach (var bag in types)
			{
				bag.Clear();
			}
		}

		public override bool Remove(TBase item)
		{
			return TypeBag.Remove(list, types, item);
		}
		#endregion
	}

	public sealed class TypedBoxList : TypedList<AtomicInfo>
	{
		#region Or statements
		public sealed class Or<TBox1, TBox2> : OrBase
		{
			private Or() { }
		}

		public sealed class Or<TBox1, TBox2, TBox3> : OrBase
		{
			private Or() { }
		}

		public sealed class Or<TBox1, TBox2, TBox3, TBox4> : OrBase
		{
			private Or() { }
		}

		public sealed class Or<TBox1, TBox2, TBox3, TBox4, TBox5> : OrBase
		{
			private Or() { }
		}
		#endregion

		#region Any statements
		public sealed class Any<TBox1, TBox2> : AnyBase
		{
			private Any() { }
		}

		public sealed class Any<TBox1, TBox2, TBox3> : AnyBase
		{
			private Any() { }
		}

		public sealed class Any<TBox1, TBox2, TBox3, TBox4> : AnyBase
		{
			private Any() { }
		}

		public sealed class Any<TBox1, TBox2, TBox3, TBox4, TBox5> : AnyBase
		{
			private Any() { }
		}

		public sealed class Any<TBox1, TBox2, TBox3, TBox4, TBox5, TBox6> : AnyBase
		{
			private Any() { }
		}
		#endregion

		#region Constructors
		public TypedBoxList(bool unknown, params Type[] order)
			: base(unknown, order) { }

		public static TypedBoxList Create<TBox1>(bool unknown = true)
		{
			return new TypedBoxList(unknown, typeof(TBox1));
		}

		public static TypedBoxList Create<TBox1, TBox2>(bool unknown = true)
		{
			return new TypedBoxList(unknown, typeof(TBox1), typeof(TBox2));
		}

		public static TypedBoxList Create<TBox1, TBox2, TBox3>(bool unknown = true)
		{
			return new TypedBoxList(unknown, typeof(TBox1), typeof(TBox2), typeof(TBox3));
		}

		public static TypedBoxList Create<TBox1, TBox2, TBox3, TBox4>(bool unknown = true)
		{
			return new TypedBoxList(unknown, typeof(TBox1), typeof(TBox2), typeof(TBox3), typeof(TBox4));
		}

		public static TypedBoxList Create<TBox1, TBox2, TBox3, TBox4, TBox5>(bool unknown = true)
		{
			return new TypedBoxList(unknown, typeof(TBox1), typeof(TBox2), typeof(TBox3), typeof(TBox4), typeof(TBox5));
		}

		public static TypedBoxList Create<TBox1, TBox2, TBox3, TBox4, TBox5, TBox6>(bool unknown = true)
		{
			return new TypedBoxList(unknown, typeof(TBox1), typeof(TBox2), typeof(TBox3), typeof(TBox4), typeof(TBox5), typeof(TBox6));
		}

		public static TypedBoxList Create<TBox1, TBox2, TBox3, TBox4, TBox5, TBox6, TBox7>(bool unknown = true)
		{
			return new TypedBoxList(unknown, typeof(TBox1), typeof(TBox2), typeof(TBox3), typeof(TBox4), typeof(TBox5), typeof(TBox6), typeof(TBox7));
		}

		public static TypedBoxList Create<TBox1, TBox2, TBox3, TBox4, TBox5, TBox6, TBox7, TBox8>(bool unknown = true)
		{
			return new TypedBoxList(unknown, typeof(TBox1), typeof(TBox2), typeof(TBox3), typeof(TBox4), typeof(TBox5), typeof(TBox6), typeof(TBox7), typeof(TBox8));
		}

		public static TypedBoxList Create<TBox1, TBox2, TBox3, TBox4, TBox5, TBox6, TBox7, TBox8, TBox9>(bool unknown = true)
		{
			return new TypedBoxList(unknown, typeof(TBox1), typeof(TBox2), typeof(TBox3), typeof(TBox4), typeof(TBox5), typeof(TBox6), typeof(TBox7), typeof(TBox8), typeof(TBox9));
		}

		public static TypedBoxList Create<TBox1, TBox2, TBox3, TBox4, TBox5, TBox6, TBox7, TBox8, TBox9, TBox10>(bool unknown = true)
		{
			return new TypedBoxList(unknown, typeof(TBox1), typeof(TBox2), typeof(TBox3), typeof(TBox4), typeof(TBox5), typeof(TBox6), typeof(TBox7), typeof(TBox8), typeof(TBox9), typeof(TBox10));
		}

		public static TypedBoxList Create<TBox1, TBox2, TBox3, TBox4, TBox5, TBox6, TBox7, TBox8, TBox9, TBox10, TBox11>(bool unknown = true)
		{
			return new TypedBoxList(unknown, typeof(TBox1), typeof(TBox2), typeof(TBox3), typeof(TBox4), typeof(TBox5), typeof(TBox6), typeof(TBox7), typeof(TBox8), typeof(TBox9), typeof(TBox10), typeof(TBox11));
		}

		public static TypedBoxList Create<TBox1, TBox2, TBox3, TBox4, TBox5, TBox6, TBox7, TBox8, TBox9, TBox10, TBox11, TBox12>(bool unknown = true)
		{
			return new TypedBoxList(unknown, typeof(TBox1), typeof(TBox2), typeof(TBox3), typeof(TBox4), typeof(TBox5), typeof(TBox6), typeof(TBox7), typeof(TBox8), typeof(TBox9), typeof(TBox10), typeof(TBox11), typeof(TBox12));
		}

		public static TypedBoxList Create<TBox1, TBox2, TBox3, TBox4, TBox5, TBox6, TBox7, TBox8, TBox9, TBox10, TBox11, TBox12, TBox13>(bool unknown = true)
		{
			return new TypedBoxList(unknown, typeof(TBox1), typeof(TBox2), typeof(TBox3), typeof(TBox4), typeof(TBox5), typeof(TBox6), typeof(TBox7), typeof(TBox8), typeof(TBox9), typeof(TBox10), typeof(TBox11), typeof(TBox12), typeof(TBox13));
		}

		public static TypedBoxList Create<TBox1, TBox2, TBox3, TBox4, TBox5, TBox6, TBox7, TBox8, TBox9, TBox10, TBox11, TBox12, TBox13, TBox14>(bool unknown = true)
		{
			return new TypedBoxList(unknown, typeof(TBox1), typeof(TBox2), typeof(TBox3), typeof(TBox4), typeof(TBox5), typeof(TBox6), typeof(TBox7), typeof(TBox8), typeof(TBox9), typeof(TBox10), typeof(TBox11), typeof(TBox12), typeof(TBox13), typeof(TBox14));
		}

		public static TypedBoxList Create<TBox1, TBox2, TBox3, TBox4, TBox5, TBox6, TBox7, TBox8, TBox9, TBox10, TBox11, TBox12, TBox13, TBox14, TBox15>(bool unknown = true)
		{
			return new TypedBoxList(unknown, typeof(TBox1), typeof(TBox2), typeof(TBox3), typeof(TBox4), typeof(TBox5), typeof(TBox6), typeof(TBox7), typeof(TBox8), typeof(TBox9), typeof(TBox10), typeof(TBox11), typeof(TBox12), typeof(TBox13), typeof(TBox14), typeof(TBox15));
		}
		#endregion
	}
}
