﻿//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace cms4seo.Common.Helpers
//{
//    public class ElementWithContext<T>
//    {
//        public T Previous { get; private set; }
//        public T Next { get; private set; }
//        public T Current { get; private set; }

//        public ElementWithContext(T current, T previous, T next)
//        {
//            Current = current;
//            Previous = previous;
//            Next = next;
//        }
//    }

//    public static class LinqExtensions
//    {
//        public static IEnumerable<ElementWithContext<T>>
//            ToNavigate<T>(this IEnumerable<T> source)
//        {
//            T previous = default(T);
//            T current = source.FirstOrDefault();

//            foreach (T next in source.Union(new[] { default(T) }).Skip(1))
//            {
//                yield return new ElementWithContext<T>(current, previous, next);
//                previous = current;
//                current = next;
//            }
//        }
//    }
//}
