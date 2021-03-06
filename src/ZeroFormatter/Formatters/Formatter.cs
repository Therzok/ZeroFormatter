﻿using System;
using ZeroFormatter.Internal;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using ZeroFormatter.Segments;
using ZeroFormatter.Comparers;

namespace ZeroFormatter.Formatters
{
    // raw layer of serialization.

    public interface IFormatter
    {
        int? GetLength();
    }

    public abstract class Formatter<T> : IFormatter
    {
        static Formatter<T> defaultFormatter;
        public static Formatter<T> Default
        {
            get
            {
                return defaultFormatter;
            }
            private set
            {
                defaultFormatter = value;
            }
        }

        static Formatter()
        {
            object formatter = null;

            var t = typeof(T);
            var ti = t.GetTypeInfo();

            try
            {
                // Primitive
                if (t == typeof(Int16))
                {
                    formatter = new Int16Formatter();
                }
                else if (t == typeof(Int32))
                {
                    formatter = new Int32Formatter();
                }
                else if (t == typeof(Int64))
                {
                    formatter = new Int64Formatter();
                }
                else if (t == typeof(UInt16))
                {
                    formatter = new UInt16Formatter();
                }
                else if (t == typeof(UInt32))
                {
                    formatter = new UInt32Formatter();
                }
                else if (t == typeof(UInt64))
                {
                    formatter = new UInt64Formatter();
                }
                else if (t == typeof(Single))
                {
                    formatter = new SingleFormatter();
                }
                else if (t == typeof(Double))
                {
                    formatter = new DoubleFormatter();
                }
                else if (t == typeof(bool))
                {
                    formatter = new BooleanFormatter();
                }
                else if (t == typeof(byte))
                {
                    formatter = new ByteFormatter();
                }
                else if (t == typeof(sbyte))
                {
                    formatter = new SByteFormatter();
                }
                else if (t == typeof(decimal))
                {
                    formatter = new DecimalFormatter();
                }
                else if (t == typeof(TimeSpan))
                {
                    formatter = new TimeSpanFormatter();
                }
                else if (t == typeof(DateTime))
                {
                    formatter = new DateTimeFormatter();
                }
                else if (t == typeof(DateTimeOffset))
                {
                    formatter = new DateTimeOffsetFormatter();
                }
                // Nulllable
                else if (t == typeof(Nullable<Int16>))
                {
                    formatter = new NullableInt16Formatter();
                }
                else if (t == typeof(Nullable<Int32>))
                {
                    formatter = new NullableInt32Formatter();
                }
                else if (t == typeof(Nullable<Int64>))
                {
                    formatter = new NullableInt64Formatter();
                }
                else if (t == typeof(Nullable<UInt16>))
                {
                    formatter = new NullableUInt16Formatter();
                }
                else if (t == typeof(Nullable<UInt32>))
                {
                    formatter = new NullableUInt32Formatter();
                }
                else if (t == typeof(Nullable<UInt64>))
                {
                    formatter = new NullableUInt64Formatter();
                }
                else if (t == typeof(Nullable<Single>))
                {
                    formatter = new NullableSingleFormatter();
                }
                else if (t == typeof(Nullable<Double>))
                {
                    formatter = new NullableDoubleFormatter();
                }
                else if (t == typeof(Nullable<bool>))
                {
                    formatter = new NullableBooleanFormatter();
                }
                else if (t == typeof(Nullable<byte>))
                {
                    formatter = new NullableByteFormatter();
                }
                else if (t == typeof(Nullable<sbyte>))
                {
                    formatter = new NullableSByteFormatter();
                }
                else if (t == typeof(Nullable<decimal>))
                {
                    formatter = new NullableDecimalFormatter();
                }
                else if (t == typeof(Nullable<TimeSpan>))
                {
                    formatter = new NullableTimeSpanFormatter();
                }
                else if (t == typeof(Nullable<DateTime>))
                {
                    formatter = new NullableDateTimeFormatter();
                }
                else if (t == typeof(Nullable<DateTimeOffset>))
                {
                    formatter = new NullableDateTimeOffsetFormatter();
                }

                // Others
                else if (t == typeof(String))
                {
                    formatter = new StringFormatter();
                }
                else if (t == typeof(Char))
                {
                    formatter = new CharFormatter();
                }
                else if (t == typeof(Char?))
                {
                    formatter = new NullableCharFormatter();
                }
                else if (t == typeof(byte[]))
                {
                    formatter = new ByteArrayFormatter();
                }
                else if (t == typeof(IList<int>)) // known generic formatter...
                {
                    formatter = new ListFormatter<int>();
                }

#if !UNITY

                else if (ti.IsGenericType && t.GetGenericTypeDefinition() == typeof(IList<>))
                {
                    var formatterType = typeof(ListFormatter<>).MakeGenericType(ti.GetGenericArguments());
                    formatter = (Formatter<T>)Activator.CreateInstance(formatterType);
                }

                else if (ti.IsGenericType && t.GetGenericTypeDefinition() == typeof(IReadOnlyList<>))
                {
                    var formatterType = typeof(ReadOnlyListFormatter<>).MakeGenericType(ti.GetGenericArguments());
                    formatter = (Formatter<T>)Activator.CreateInstance(formatterType);
                }

                else if (ti.IsEnum)
                {
                    var underlyingType = Enum.GetUnderlyingType(t);
                    switch (Type.GetTypeCode(underlyingType))
                    {
                        case TypeCode.SByte:
                            formatter = new SByteEnumFormatter<T>();
                            break;
                        case TypeCode.Byte:
                            formatter = new ByteEnumFormatter<T>();
                            break;
                        case TypeCode.Int16:
                            formatter = new Int16EnumFormatter<T>();
                            break;
                        case TypeCode.UInt16:
                            formatter = new UInt16EnumFormatter<T>();
                            break;
                        case TypeCode.Int32:
                            formatter = new Int32EnumFormatter<T>();
                            break;
                        case TypeCode.UInt32:
                            formatter = new UInt32EnumFormatter<T>();
                            break;
                        case TypeCode.Int64:
                            formatter = new Int64EnumFormatter<T>();
                            break;
                        case TypeCode.UInt64:
                            formatter = new UInt64EnumFormatter<T>();
                            break;
                        default:
                            throw new Exception();
                    }
                }

                else if (ti.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>) && ti.GetGenericArguments()[0].GetTypeInfo().IsEnum)
                {
                    var underlyingType = Enum.GetUnderlyingType(ti.GetGenericArguments()[0]);
                    switch (Type.GetTypeCode(underlyingType))
                    {
                        case TypeCode.SByte:
                            {
                                var formatterType = typeof(NullableSByteEnumFormatter<>).MakeGenericType(ti.GetGenericArguments()[0]);
                                formatter = (Formatter<T>)Activator.CreateInstance(formatterType);
                            }
                            break;
                        case TypeCode.Byte:
                            {
                                var formatterType = typeof(NullableByteEnumFormatter<>).MakeGenericType(ti.GetGenericArguments()[0]);
                                formatter = (Formatter<T>)Activator.CreateInstance(formatterType);
                            }
                            break;
                        case TypeCode.Int16:
                            {
                                var formatterType = typeof(NullableInt16EnumFormatter<>).MakeGenericType(ti.GetGenericArguments()[0]);
                                formatter = (Formatter<T>)Activator.CreateInstance(formatterType);
                            }
                            break;
                        case TypeCode.UInt16:
                            {
                                var formatterType = typeof(NullableUInt16EnumFormatter<>).MakeGenericType(ti.GetGenericArguments()[0]);
                                formatter = (Formatter<T>)Activator.CreateInstance(formatterType);
                            }
                            break;
                        case TypeCode.Int32:
                            {
                                var formatterType = typeof(NullableInt32EnumFormatter<>).MakeGenericType(ti.GetGenericArguments()[0]);
                                formatter = (Formatter<T>)Activator.CreateInstance(formatterType);
                            }
                            break;
                        case TypeCode.UInt32:
                            {
                                var formatterType = typeof(NullableUInt32EnumFormatter<>).MakeGenericType(ti.GetGenericArguments()[0]);
                                formatter = (Formatter<T>)Activator.CreateInstance(formatterType);
                            }
                            break;
                        case TypeCode.Int64:
                            {
                                var formatterType = typeof(NullableInt64EnumFormatter<>).MakeGenericType(ti.GetGenericArguments()[0]);
                                formatter = (Formatter<T>)Activator.CreateInstance(formatterType);
                            }
                            break;
                        case TypeCode.UInt64:
                            {
                                var formatterType = typeof(NullableUInt64EnumFormatter<>).MakeGenericType(ti.GetGenericArguments()[0]);
                                formatter = (Formatter<T>)Activator.CreateInstance(formatterType);
                            }
                            break;
                        default:
                            throw new Exception();
                    }
                }

                else if (ti.IsGenericType)
                {
                    if (t.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                    {
                        var formatterType = typeof(DictionaryFormatter<,>).MakeGenericType(ti.GetGenericArguments());
                        formatter = (Formatter<T>)Activator.CreateInstance(formatterType);
                    }
                    else if (t.GetGenericTypeDefinition() == typeof(ILazyDictionary<,>))
                    {
                        var formatterType = typeof(LazyDictionaryFormatter<,>).MakeGenericType(ti.GetGenericArguments());
                        formatter = (Formatter<T>)Activator.CreateInstance(formatterType);
                    }
                    else if (t.GetGenericTypeDefinition() == typeof(IReadOnlyDictionary<,>))
                    {
                        var formatterType = typeof(ReadOnlyDictionaryFormatter<,>).MakeGenericType(ti.GetGenericArguments());
                        formatter = (Formatter<T>)Activator.CreateInstance(formatterType);
                    }
                    else if (t.GetGenericTypeDefinition() == typeof(ILazyReadOnlyDictionary<,>))
                    {
                        var formatterType = typeof(LazyReadOnlyDictionaryFormatter<,>).MakeGenericType(ti.GetGenericArguments());
                        formatter = (Formatter<T>)Activator.CreateInstance(formatterType);
                    }
                    else if (t.GetGenericTypeDefinition() == typeof(DictionaryEntry<,>))
                    {
                        var formatterType = typeof(DictionaryEntryFormatter<,>).MakeGenericType(ti.GetGenericArguments());
                        formatter = (Formatter<T>)Activator.CreateInstance(formatterType);
                    }

                    else if (t.GetGenericTypeDefinition() == typeof(ILookup<,>))
                    {
                        var formatterType = typeof(LookupFormatter<,>).MakeGenericType(ti.GetGenericArguments());
                        formatter = (Formatter<T>)Activator.CreateInstance(formatterType);
                    }
                    else if (t.GetGenericTypeDefinition() == typeof(ILazyLookup<,>))
                    {
                        var formatterType = typeof(LazyLookupFormatter<,>).MakeGenericType(ti.GetGenericArguments());
                        formatter = (Formatter<T>)Activator.CreateInstance(formatterType);
                    }
                    else if (t.GetGenericTypeDefinition() == typeof(GroupingSegment<,>))
                    {
                        var formatterType = typeof(GroupingSegmentFormatter<,>).MakeGenericType(ti.GetGenericArguments());
                        formatter = (Formatter<T>)Activator.CreateInstance(formatterType);
                    }

                    else if (ti.GetInterfaces().Any(x => x == typeof(IKeyTuple)))
                    {
                        Type tupleFormatterType = null;
                        switch (ti.GetGenericArguments().Length)
                        {
                            case 1:
                                tupleFormatterType = typeof(KeyTupleFormatter<>);
                                break;
                            case 2:
                                tupleFormatterType = typeof(KeyTupleFormatter<,>);
                                break;
                            case 3:
                                tupleFormatterType = typeof(KeyTupleFormatter<,,>);
                                break;
                            case 4:
                                tupleFormatterType = typeof(KeyTupleFormatter<,,,>);
                                break;
                            case 5:
                                tupleFormatterType = typeof(KeyTupleFormatter<,,,,>);
                                break;
                            case 6:
                                tupleFormatterType = typeof(KeyTupleFormatter<,,,,,>);
                                break;
                            case 7:
                                tupleFormatterType = typeof(KeyTupleFormatter<,,,,,,>);
                                break;
                            case 8:
                                tupleFormatterType = typeof(KeyTupleFormatter<,,,,,,,>);
                                break;
                            default:
                                break;
                        }

                        var formatterType = tupleFormatterType.MakeGenericType(ti.GetGenericArguments());
                        formatter = (Formatter<T>)Activator.CreateInstance(formatterType);
                    }

                    else if (t.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                    {
                        throw new InvalidOperationException("Dictionary does not support in ZeroFormatter because Dictionary have to deserialize all objects. You can use IDictionary<TK, TV> instead of Dictionary.");
                    }

                    // custom nullable struct
                    else if (t.GetGenericTypeDefinition() == typeof(Nullable<>) && ti.GetGenericArguments()[0].GetTypeInfo().IsValueType)
                    {
                        formatter = DynamicStructFormatter.Create<T>();
                    }
                }

                else if (t.IsArray)
                {
                    throw new InvalidOperationException("Array does not support in ZeroFormatter(except byte[]) because Array have to deserialize all objects. You can use IList<T> instead of T[].");
                }

                else if (ti.GetCustomAttributes(typeof(ZeroFormattableAttribute), true).FirstOrDefault() != null)
                {
                    if (ti.IsValueType)
                    {
                        formatter = DynamicStructFormatter.Create<T>();
                    }
                    else
                    {
                        formatter = DynamicObjectFormatter.Create<T>();
                    }
                }

#endif
            }
            catch (InvalidOperationException ex)
            {
                formatter = new ErrorFormatter<T>(ex);
            }
            catch (Exception ex)
            {
                formatter = new ErrorFormatter<T>(ex);
            }

            // resolver
            if (formatter == null || formatter is ErrorFormatter<T>)
            {
                try
                {
                    var resolvedFormatter = Formatter.ResolveFormatter<T>();
                    if (resolvedFormatter != null)
                    {
                        formatter = (Formatter<T>)resolvedFormatter;
                    }
                    else if (formatter == null)
                    {
                        formatter = new ErrorFormatter<T>();
                    }
                }
                catch (Exception ex)
                {
                    formatter = new ErrorFormatter<T>(ex);
                }
            }

            Default = (Formatter<T>)formatter;
        }

        public Formatter()
        {
        }

        public static void Register(Formatter<T> formatter)
        {
            defaultFormatter = formatter;
        }

        public abstract int? GetLength();

        public abstract int Serialize(ref byte[] bytes, int offset, T value);
        public abstract T Deserialize(ref byte[] bytes, int offset, DirtyTracker tracker, out int byteSize);
    }

    public static class Formatter
    {
        static List<Func<Type, object>> formatterResolver = new List<Func<Type, object>>();

        public static void AppendFormatterResolver(Func<Type, object> formatterResolver)
        {
            Formatter.formatterResolver.Add(formatterResolver);
        }

        internal static object ResolveFormatter<T>()
        {
            var t = typeof(T);
            foreach (var item in formatterResolver)
            {
                var resolved = item(t);
                if (resolved != null)
                {
                    return resolved;
                }
            }
            return null;
        }

        public static void RegisterLookup<TKey, TElement>()
        {
            if (Formatter<ILookup<TKey, TElement>>.Default is IErrorFormatter)
            {
                Formatter<ILookup<TKey, TElement>>.Register(new LookupFormatter<TKey, TElement>());
            }
            if (Formatter<IList<TElement>>.Default is IErrorFormatter)
            {
                Formatter<IList<TElement>>.Register(new ListFormatter<TElement>());
            }
        }

        public static void RegisterLazyLookup<TKey, TElement>()
        {
            if (Formatter<ILazyLookup<TKey, TElement>>.Default is IErrorFormatter)
            {
                Formatter<ILazyLookup<TKey, TElement>>.Register(new LazyLookupFormatter<TKey, TElement>());
            }
            if (Formatter<GroupingSegment<TKey, TElement>>.Default is IErrorFormatter)
            {
                Formatter<GroupingSegment<TKey, TElement>>.Register(new GroupingSegmentFormatter<TKey, TElement>());
            }
            if (Formatter<IList<TKey>>.Default is IErrorFormatter)
            {
                Formatter<IList<TKey>>.Register(new ListFormatter<TKey>());
            }
            if (Formatter<IList<GroupingSegment<TKey, TElement>>>.Default is IErrorFormatter)
            {
                Formatter<IList<GroupingSegment<TKey, TElement>>>.Register(new ListFormatter<GroupingSegment<TKey, TElement>>());
            }
            if (Formatter<IList<IList<GroupingSegment<TKey, TElement>>>>.Default is IErrorFormatter)
            {
                Formatter<IList<IList<GroupingSegment<TKey, TElement>>>>.Register(new ListFormatter<IList<GroupingSegment<TKey, TElement>>>());
            }
            if (Formatter<IList<TElement>>.Default is IErrorFormatter)
            {
                Formatter<IList<TElement>>.Register(new ListFormatter<TElement>());
            }
        }

        public static void RegisterDictionary<TKey, TValue>()
        {
            if (Formatter<IDictionary<TKey, TValue>>.Default is IErrorFormatter)
            {
                Formatter<IDictionary<TKey, TValue>>.Register(new DictionaryFormatter<TKey, TValue>());
            }
        }

#if !UNITY

        public static void RegisterReadOnlyDictionary<TKey, TValue>()
        {
            if (Formatter<IReadOnlyDictionary<TKey, TValue>>.Default is IErrorFormatter)
            {
                Formatter<IReadOnlyDictionary<TKey, TValue>>.Register(new ReadOnlyDictionaryFormatter<TKey, TValue>());
            }
        }

#endif

        public static void RegisterLazyDictionary<TKey, TValue>()
        {
            if (Formatter<ILazyDictionary<TKey, TValue>>.Default is IErrorFormatter)
            {
                Formatter<ILazyDictionary<TKey, TValue>>.Register(new LazyDictionaryFormatter<TKey, TValue>());
            }
            if (Formatter<DictionaryEntry<TKey, TValue>>.Default is IErrorFormatter)
            {
                Formatter<DictionaryEntry<TKey, TValue>>.Register(new DictionaryEntryFormatter<TKey, TValue>());
            }
            if (Formatter<IList<DictionaryEntry<TKey, TValue>>>.Default is IErrorFormatter)
            {
                Formatter<IList<DictionaryEntry<TKey, TValue>>>.Register(new ListFormatter<DictionaryEntry<TKey, TValue>>());
            }
        }

        public static void RegisterList<T>()
        {
            if (Formatter<IList<T>>.Default is IErrorFormatter)
            {
                Formatter<IList<T>>.Register(new ListFormatter<T>());
            }
        }

#if !UNITY

        public static void RegisterReadOnlyList<T>()
        {
            if (Formatter<IReadOnlyList<T>>.Default is IErrorFormatter)
            {
                Formatter<IReadOnlyList<T>>.Register(new ReadOnlyListFormatter<T>());
            }
        }

#endif

        public static void RegisterKeyTuple<T1>()
        {
            if (Formatter<KeyTuple<T1>>.Default is IErrorFormatter)
            {
                Formatter<KeyTuple<T1>>.Register(new KeyTupleFormatter<T1>());
            }
            if (Formatter<KeyTuple<T1>?>.Default is IErrorFormatter)
            {
                Formatter<KeyTuple<T1>?>.Register(new NullableKeyTupleFormatter<T1>());
            }
            if (ZeroFormatterEqualityComparer<KeyTuple<T1>>.Default is IErrorEqualityComparer)
            {
                ZeroFormatterEqualityComparer<KeyTuple<T1>>.Register(new KeyTupleEqualityComparer<T1>());
            }
        }

        public static void RegisterKeyTuple<T1, T2>()
        {
            if (Formatter<KeyTuple<T1, T2>>.Default is IErrorFormatter)
            {
                Formatter<KeyTuple<T1, T2>>.Register(new KeyTupleFormatter<T1, T2>());
            }
            if (Formatter<KeyTuple<T1, T2>?>.Default is IErrorFormatter)
            {
                Formatter<KeyTuple<T1, T2>?>.Register(new NullableKeyTupleFormatter<T1, T2>());
            }
            if (ZeroFormatterEqualityComparer<KeyTuple<T1, T2>>.Default is IErrorEqualityComparer)
            {
                ZeroFormatterEqualityComparer<KeyTuple<T1, T2>>.Register(new KeyTupleEqualityComparer<T1, T2>());
            }
        }

        public static void RegisterKeyTuple<T1, T2, T3>()
        {
            if (Formatter<KeyTuple<T1, T2, T3>>.Default is IErrorFormatter)
            {
                Formatter<KeyTuple<T1, T2, T3>>.Register(new KeyTupleFormatter<T1, T2, T3>());
            }
            if (Formatter<KeyTuple<T1, T2, T3>?>.Default is IErrorFormatter)
            {
                Formatter<KeyTuple<T1, T2, T3>?>.Register(new NullableKeyTupleFormatter<T1, T2, T3>());
            }
            if (ZeroFormatterEqualityComparer<KeyTuple<T1, T2, T3>>.Default is IErrorEqualityComparer)
            {
                ZeroFormatterEqualityComparer<KeyTuple<T1, T2, T3>>.Register(new KeyTupleEqualityComparer<T1, T2, T3>());
            }
        }

        public static void RegisterKeyTuple<T1, T2, T3, T4>()
        {
            if (Formatter<KeyTuple<T1, T2, T3, T4>>.Default is IErrorFormatter)
            {
                Formatter<KeyTuple<T1, T2, T3, T4>>.Register(new KeyTupleFormatter<T1, T2, T3, T4>());
            }
            if (Formatter<KeyTuple<T1, T2, T3, T4>?>.Default is IErrorFormatter)
            {
                Formatter<KeyTuple<T1, T2, T3, T4>?>.Register(new NullableKeyTupleFormatter<T1, T2, T3, T4>());
            }
            if (ZeroFormatterEqualityComparer<KeyTuple<T1, T2, T3, T4>>.Default is IErrorEqualityComparer)
            {
                ZeroFormatterEqualityComparer<KeyTuple<T1, T2, T3, T4>>.Register(new KeyTupleEqualityComparer<T1, T2, T3, T4>());
            }
        }

        public static void RegisterKeyTuple<T1, T2, T3, T4, T5>()
        {
            if (Formatter<KeyTuple<T1, T2, T3, T4, T5>>.Default is IErrorFormatter)
            {
                Formatter<KeyTuple<T1, T2, T3, T4, T5>>.Register(new KeyTupleFormatter<T1, T2, T3, T4, T5>());
            }
            if (Formatter<KeyTuple<T1, T2, T3, T4, T5>?>.Default is IErrorFormatter)
            {
                Formatter<KeyTuple<T1, T2, T3, T4, T5>?>.Register(new NullableKeyTupleFormatter<T1, T2, T3, T4, T5>());
            }
            if (ZeroFormatterEqualityComparer<KeyTuple<T1, T2, T3, T4, T5>>.Default is IErrorEqualityComparer)
            {
                ZeroFormatterEqualityComparer<KeyTuple<T1, T2, T3, T4, T5>>.Register(new KeyTupleEqualityComparer<T1, T2, T3, T4, T5>());
            }
        }

        public static void RegisterKeyTuple<T1, T2, T3, T4, T5, T6>()
        {
            if (Formatter<KeyTuple<T1, T2, T3, T4, T5, T6>>.Default is IErrorFormatter)
            {
                Formatter<KeyTuple<T1, T2, T3, T4, T5, T6>>.Register(new KeyTupleFormatter<T1, T2, T3, T4, T5, T6>());
            }
            if (Formatter<KeyTuple<T1, T2, T3, T4, T5, T6>?>.Default is IErrorFormatter)
            {
                Formatter<KeyTuple<T1, T2, T3, T4, T5, T6>?>.Register(new NullableKeyTupleFormatter<T1, T2, T3, T4, T5, T6>());
            }
            if (ZeroFormatterEqualityComparer<KeyTuple<T1, T2, T3, T4, T5, T6>>.Default is IErrorEqualityComparer)
            {
                ZeroFormatterEqualityComparer<KeyTuple<T1, T2, T3, T4, T5, T6>>.Register(new KeyTupleEqualityComparer<T1, T2, T3, T4, T5, T6>());
            }
        }

        public static void RegisterKeyTuple<T1, T2, T3, T4, T5, T6, T7>()
        {
            if (Formatter<KeyTuple<T1, T2, T3, T4, T5, T6, T7>>.Default is IErrorFormatter)
            {
                Formatter<KeyTuple<T1, T2, T3, T4, T5, T6, T7>>.Register(new KeyTupleFormatter<T1, T2, T3, T4, T5, T6, T7>());
            }
            if (Formatter<KeyTuple<T1, T2, T3, T4, T5, T6, T7>?>.Default is IErrorFormatter)
            {
                Formatter<KeyTuple<T1, T2, T3, T4, T5, T6, T7>?>.Register(new NullableKeyTupleFormatter<T1, T2, T3, T4, T5, T6, T7>());
            }
            if (ZeroFormatterEqualityComparer<KeyTuple<T1, T2, T3, T4, T5, T6, T7>>.Default is IErrorEqualityComparer)
            {
                ZeroFormatterEqualityComparer<KeyTuple<T1, T2, T3, T4, T5, T6, T7>>.Register(new KeyTupleEqualityComparer<T1, T2, T3, T4, T5, T6, T7>());
            }
        }

        public static void RegisterKeyTuple<T1, T2, T3, T4, T5, T6, T7, T8>()
        {
            if (Formatter<KeyTuple<T1, T2, T3, T4, T5, T6, T7, T8>>.Default is IErrorFormatter)
            {
                Formatter<KeyTuple<T1, T2, T3, T4, T5, T6, T7, T8>>.Register(new KeyTupleFormatter<T1, T2, T3, T4, T5, T6, T7, T8>());
            }
            if (Formatter<KeyTuple<T1, T2, T3, T4, T5, T6, T7, T8>?>.Default is IErrorFormatter)
            {
                Formatter<KeyTuple<T1, T2, T3, T4, T5, T6, T7, T8>?>.Register(new NullableKeyTupleFormatter<T1, T2, T3, T4, T5, T6, T7, T8>());
            }
            if (ZeroFormatterEqualityComparer<KeyTuple<T1, T2, T3, T4, T5, T6, T7, T8>>.Default is IErrorEqualityComparer)
            {
                ZeroFormatterEqualityComparer<KeyTuple<T1, T2, T3, T4, T5, T6, T7, T8>>.Register(new KeyTupleEqualityComparer<T1, T2, T3, T4, T5, T6, T7, T8>());
            }
        }
    }

    internal interface IErrorFormatter
    {
        InvalidOperationException GetException();
    }

    internal class ErrorFormatter<T> : Formatter<T>, IErrorFormatter
    {
        readonly InvalidOperationException exception;

        public ErrorFormatter()
        {
            string message;
            var t = typeof(T);
            var ti = t.GetTypeInfo();
            if (ti.IsGenericType)
            {
                // depth 1 only:)
                message = t.Name + "<" + string.Join(", ", ti.GetGenericArguments().Select(x => x.Name).ToArray()) + ">";
            }
            else
            {
                message = t.Name;
            }

            this.exception = new InvalidOperationException("Type is not supported, please register " + message);
        }

        public ErrorFormatter(Exception ex)
        {
            this.exception = new InvalidOperationException("Type is not supported, occurs invalid error: " + typeof(T).Name + " InnerException:" + ex.ToString());
        }

        public ErrorFormatter(InvalidOperationException exception)
        {
            this.exception = exception;
        }

        public InvalidOperationException GetException()
        {
            return exception;
        }

        public override T Deserialize(ref byte[] bytes, int offset, DirtyTracker tracker, out int byteSize)
        {
            throw exception;
        }

        public override int? GetLength()
        {
            throw exception;
        }

        public override int Serialize(ref byte[] bytes, int offset, T value)
        {
            throw exception;
        }
    }
}