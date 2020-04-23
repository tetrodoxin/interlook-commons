#region license

//MIT License

//Copyright(c) 2013-2020 Andreas Hübner

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

#endregion 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Interlook.Functional
{
    /// <summary>
    /// Definiert Hilfsmethoden für Sequenzen und LINQ, analog zu Haskell Operationen
    /// </summary>
    public static class HaskellListOps
    {
        /// <summary>
        /// Gibt das erste Element einer Sequenz zurück.
        /// Eine Ausnahme wird ausgelöst, falls die Sequenz leer ist.
        /// </summary>
        /// <typeparam name="T">Datentyp der Sequenz</typeparam>
        /// <param name="source">Die Sequenz, deren Startelement zurückgegeben werden soll.</param>
        /// <returns>Das erste Element der Sequenz.</returns>
        public static T Head<T>(this IEnumerable<T> source)
        {
            Contract.Requires(source != null);

            return source.First();
        }

        /// <summary>
        /// Gibt den Rest einer Sequenz nach ihrem ersten Element zurück.
        /// Es wird eine Sequenz erzeugt, der das erste Element der Original-Sequenz entfernt wurde.
        /// Für leere Sequenzen wird eine Exception ausgelöst.
        /// </summary>
        /// <typeparam name="T">Datentyp der Sequenz</typeparam>
        /// <param name="source">Die Sequenz, deren Rest zurückgegeben werden soll.</param>
        /// <returns>
        /// Der Rest der Sequenz ohne das erste Element. Kann leer sein.
        /// </returns>
        public static IEnumerable<T> Tail<T>(this IEnumerable<T> source)
        {
            Contract.Requires(source != null);

            var src = source.ToArray();
#if NETCORE
            if (src.Length == 0) return Array.Empty<T>();
#else
            if (src.Length == 0) return new T[0];
#endif

            return source.Skip(1);
        }

        /// <summary>
        /// Gibt eine Sequenz bis vor das letzte Element zurück.
        /// Es wird also eine neue Sequenz erzeugt, der das letzte Element der Originalsequenz fehlt.
        /// </summary>
        /// <typeparam name="T">Datentyp der Sequenz</typeparam>
        /// <param name="source">Die Sequenz, deren Anfang zurückgegeben werden soll.</param>
        /// <returns>
        /// Die Sequenz ohne das letzte Element. Kann leer sein.
        /// </returns>
        public static IEnumerable<T> Init<T>(this IEnumerable<T> source)
        {
            Contract.Requires(source != null);

            var src = source.ToArray();
#if NETCORE
            if (src.Length == 0) return Array.Empty<T>();
#else
            if (src.Length == 0) return new T[0];
#endif

            return src.Take(src.Length - 1);
        }

        /// <summary>
        /// Überspringt eine bestimmte Anzahl von Elementen der Sequenz.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">Die Eingabesequenz.</param>
        /// <param name="count">Die Anzahl der zu überspringenden Elemente.</param>
        /// <returns></returns>
        public static IEnumerable<T> Drop<T>(this IEnumerable<T> source, int count)
        {
            Contract.Requires(source != null);

            return source.Skip(count);
        }

        /// <summary>
        /// Fügt das Objekt in eine neue Sequenz vom gleichen Datentyp ein.
        /// </summary>
        /// <typeparam name="T">Der Datentyp der neuen Sequenz</typeparam>
        /// <param name="o">Das einzufügende Objekt.</param>
        /// <returns>Eine neue Sequenz, die nur das angegebene Objekt enthält.</returns>
        public static IEnumerable<T> Cons<T>(this T o)
        {
            return new T[1] { o };
        }

        /// <summary>
        /// Fügt das Objekt an die erste Stelle einer Sequenz ein.
        /// </summary>
        /// <typeparam name="T">Datentyp der Sequenz und des Objekts</typeparam>
        /// <param name="o">Das einzufügende Objekt.</param>
        /// <param name="list">Die Sequenz, der das Objekt vorangestellt werden soll.</param>
        /// <returns>Eine Sequenz, an deren erster Stelle das gegebene Objekt steht.</returns>
        public static IEnumerable<T> Cons<T>(this T o, IEnumerable<T> list)
        {
            Contract.Requires(list != null);
            yield return o;
            foreach (var e in list)
            {
                yield return e;
            }
        }

        /// <summary>
        /// Erzeugt eine Endlossequenz aus einer Sequenz.
        /// Dabei wird wiederholt über die Quellsequenz iteriert.
        /// </summary>
        /// <typeparam name="T">Datentyp der Sequenz.</typeparam>
        /// <param name="source">Die Quellsequenz..</param>
        /// <returns>Eine Sequenz, deren Enumerator (siehe <see cref="IEnumerator"/>) nie beendet.
        /// Nach dem letzten Element der Quellsequenz wird stets wieder mit dem Anfang begonnen.
        /// </returns>
        public static IEnumerable<T> Cycle<T>(this IEnumerable<T> source)
        {
            Contract.Requires(source != null);

            while (true)
            {
                foreach (var e in source)
                {
                    yield return e;
                }
            }
        }

        /// <summary>
        /// Ordnet jedem Element einer Sequenz eine Projektionsfunktion zu.
        /// Gleicht <see cref="Enumerable.Select{TSource, TResult}(IEnumerable{TSource}, Func{TSource, int, TResult})"/>
        /// </summary>
        /// <typeparam name="TSource">Der Datentyp der Quellsequenz.</typeparam>
        /// <typeparam name="TResult">Der Datentyp der Zielsequenz.</typeparam>
        /// <param name="source">Die Quellsequenz.</param>
        /// <param name="mapper">Die Projektionsfunktion.</param>
        /// <returns>Eine Sequenz mit den projizierten Elementen.</returns>
        public static IEnumerable<TResult> Map<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> mapper)
        {
            Contract.Requires(source != null);
            Contract.Requires(mapper != null);

            return source.Select(mapper);
        }

        /// <summary>
        /// Prüft, ob die Sequenz einen bestimmten Wert enthält.
        /// Entspricht <see cref="Enumerable.Contains{TSource}(IEnumerable{TSource}, TSource)"/>
        /// </summary>
        /// <typeparam name="TSource">Zugrunde liegender Datentyp.</typeparam>
        /// <param name="source">Die Quellsequenz.</param>
        /// <param name="value">Die zu prüfende Wert.</param>
        /// <returns></returns>
        public static bool Elem<TSource>(this IEnumerable<TSource> source, TSource value)
        {
            Contract.Requires(source != null);

            return source.Contains(value);
        }

        /// <summary>
        /// Prüft, dass die Sequent einen bestimmten Wert nicht enthält.
        /// </summary>
        /// <typeparam name="TSource">Zugrunde liegender Datentyp.</typeparam>
        /// <param name="source">Die Quellsequenz.</param>
        /// <param name="value">Die zu prüfende Wert.</param>
        /// <returns></returns>
        public static bool NotElem<TSource>(this IEnumerable<TSource> source, TSource value)
        {
            Contract.Requires(source != null);

            return !source.Contains(value);
        }

        /// <summary>
        /// Erzeugt eine endlose Sequenz, deren einziges Element das angegebene Objekt ist.
        /// </summary>
        /// <typeparam name="T">Datentyp des Objekts und der Sequenz</typeparam>
        /// <param name="value">Das Objekt/der Wert, welches/welcher die Sequenz ausmachen soll.</param>
        /// <returns>Eine Sequenz, die nur das angegebene Objekt zum Inhalt hat und
        /// deren Iterator (siehe <see cref="IEnumerator"/>) niemals beendet.</returns>
        public static IEnumerable<T> Repeat<T>(this T value)
        {
            while (true)
            {
                yield return value;
            }
        }

        /// <summary>
        /// Erzeugt eine Sequenz, deren einziges Element das angegebene Objekt ist
        /// und die eine maximale Länge besitzt.
        /// Ähnelt <see cref="Repeat{T}(T)"/>, mit dem Unterschied, dass <see cref="Replicate{T}(T, int)"/>
        /// keine endlose Sequenz erzeugt.
        /// </summary>
        /// <typeparam name="T">Datentyp der zu erzeugenden Sequenz.</typeparam>
        /// <param name="value">Der Inhaltswert, der Sequenz.</param>
        /// <param name="count">Die maximale Länge der Sequenz.</param>
        /// <returns></returns>
        public static IEnumerable<T> Replicate<T>(this T value, int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return value;
            }
        }

        /// <summary>
        /// Führt zwei Sequenzen zusammen, indem eine neue Sequent mit Paaren von Elementen erzeugt wird.
        /// </summary>
        /// <typeparam name="T1">Datentyp der ersten Sequenz.</typeparam>
        /// <typeparam name="T2">Datentyp der zweiten Sequenz.</typeparam>
        /// <param name="first">Erste Sequenz.</param>
        /// <param name="second">Zweite Sequenz.</param>
        /// <returns>Eine neue Sequenz von <see cref="Tuple{T1, T2}"/>, die maximal so viele
        /// Elemente beinhaltet, wie die kürzeste der beiden angegebenen Sequenzen.</returns>
        public static IEnumerable<(T1, T2)> Zip<T1, T2>(this IEnumerable<T1> first, IEnumerable<T2> second)
        {
            Contract.Requires(first != null);
            Contract.Requires(second != null);

            using (var se1 = first.GetEnumerator())
            {
                using (var se2 = second.GetEnumerator())
                {
                    while (se1.MoveNext() && se2.MoveNext())
                    {
                        yield return (se1.Current, se2.Current);
                    }
                }
            }
        }

        /// <summary>
        /// Führt zwei Sequenzen mit der angegebenen Prädikatfunktion zusammen.
        /// Entspricht <see cref="Enumerable.Zip{TFirst, TSecond, TResult}(IEnumerable{TFirst}, IEnumerable{TSecond}, Func{TFirst, TSecond, TResult})"/>
        /// </summary>
        /// <typeparam name="T1">Der Typ der Elemente der ersten Eingabesequenz.</typeparam>
        /// <typeparam name="T2">Der Typ der Elemente der zweiten Eingabesequenz.</typeparam>
        /// <typeparam name="TResult">Der Typ der Elemente der Ergebnissequenz.</typeparam>
        /// <param name="source">Erste Eingabesequenz.</param>
        /// <param name="second">Zweite Eingabesequenz.</param>
        /// <param name="resultSelector">Eine Funktion, die angibt, wie die Elemente der zwei Sequenzen zusammengeführt werden sollen..</param>
        /// <returns>Ein <see cref="IEnumerable{T}"/>, das die zusammengeführten Elemente der beiden Eingabesequenzen enthält.</returns>
        public static IEnumerable<TResult> ZipWith<T1, T2, TResult>(this IEnumerable<T1> source, IEnumerable<T2> second, Func<T1, T2, TResult> resultSelector)
        {
            Contract.Requires(source != null);
            Contract.Requires(second != null);
            Contract.Requires(resultSelector != null);

            return source.Zip(second, resultSelector);
        }

        /// <summary>
        /// Führt drei Sequenzen zusammen, indem eine neue Sequent mit Tripeln von Elementen erzeugt wird.
        /// </summary>
        /// <typeparam name="T1">Der Typ der Elemente der ersten Eingabesequenz.</typeparam>
        /// <typeparam name="T2">Der Typ der Elemente der zweiten Eingabesequenz.</typeparam>
        /// <typeparam name="T3">Der Typ der Elemente der dritten Eingabesequenz.</typeparam>
        /// <param name="source">Die erste Sequenz, die zusammengeführt werden soll.</param>
        /// <param name="second">Die zweite Sequenz, die zusammengeführt werden soll.</param>
        /// <param name="third">Die dritte Sequenz, die zusammengeführt werden soll.</param>
        /// <returns>Ein <see cref="IEnumerable{T}"/>, das die zusammengeführten Elemente der drei Eingabesequenzen
        /// als <see cref="Tuple{T1, T2, T3}"/> enthält und maximal so viele Elemente beinhaltet,
        /// wie die kürzeste Eingabesequenz.</returns>
        public static IEnumerable<(T1, T2, T3)> Zip3<T1, T2, T3>(this IEnumerable<T1> source, IEnumerable<T2> second, IEnumerable<T3> third)
        {
            Contract.Requires(source != null);
            Contract.Requires(second != null);
            Contract.Requires(third != null);

            using (var se1 = source.GetEnumerator())
            {
                using (var se2 = second.GetEnumerator())
                {
                    using (var se3 = third.GetEnumerator())
                    {
                        while (se1.MoveNext() && se2.MoveNext() && se3.MoveNext())
                        {
                            yield return (se1.Current, se2.Current, se3.Current);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Führt drei Sequenzen mit der angegebenen Prädikatfunktion zusammen.
        /// </summary>
        /// <typeparam name="T1">Der Typ der Elemente der ersten Eingabesequenz.</typeparam>
        /// <typeparam name="T2">Der Typ der Elemente der zweiten Eingabesequenz.</typeparam>
        /// <typeparam name="T3">Der Typ der Elemente der dritten Eingabesequenz.</typeparam>
        /// <typeparam name="TResult">Der Typ der Elemente der Ergebnissequenz.</typeparam>
        /// <param name="source">Erste Eingabesequenz.</param>
        /// <param name="second">Zweite Eingabesequenz.</param>
        /// <param name="third">Dritte Eingabesequenz.</param>
        /// <param name="resultSelector">Eine Funktion, die angibt, wie die Elemente der drei Sequenzen zusammengeführt werden sollen..</param>
        /// <returns>
        /// Ein <see cref="IEnumerable{T}" />, das die zusammengeführten Elemente der drei Eingabesequenzen enthält.
        /// </returns>
        public static IEnumerable<TResult> ZipWith3<T1, T2, T3, TResult>(this IEnumerable<T1> source, IEnumerable<T2> second, IEnumerable<T3> third, Func<T1, T2, T3, TResult> resultSelector)
        {
            Contract.Requires(source != null);
            Contract.Requires(second != null);
            Contract.Requires(third != null);
            Contract.Requires(resultSelector != null);

            using (var se1 = source.GetEnumerator())
            {
                using (var se2 = second.GetEnumerator())
                {
                    using (var se3 = third.GetEnumerator())
                    {
                        while (se1.MoveNext() && se2.MoveNext() && se3.MoveNext())
                        {
                            yield return resultSelector(se1.Current, se2.Current, se3.Current);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Trennt eine Sequenz von Paaren in zwei Sequenzen von Einzelelementen auf.
        /// </summary>
        /// <typeparam name="T1">Typ des ersten Elements der Paare der Eingabesequenz.</typeparam>
        /// <typeparam name="T2">Typ des zweiten Elements der Paare der Eingabesequenz.</typeparam>
        /// <param name="source">Die Eingabesequenz.</param>
        /// <returns>Ein Paar von zwei Einzelsequenzen, jeweils nach erstem und zweitem Element der
        /// Paare der Eingangssequenz aufgeteilt.</returns>
        public static (IEnumerable<T1>, IEnumerable<T2>) Unzip<T1, T2>(this IEnumerable<(T1, T2)> source)
        {
            Contract.Requires(source != null);

            return (source.Select(p => p.Item1), source.Select(p => p.Item2));
        }

        /// <summary>
        /// Trennt eine Sequenz von Tripeln in drei Sequenzen von Einzelelementen auf.
        /// </summary>
        /// <typeparam name="T1">Typ des ersten Elements der Tripel der Eingabesequenz.</typeparam>
        /// <typeparam name="T2">Typ des zweiten Elements der Tripel der Eingabesequenz.</typeparam>
        /// <typeparam name="T3">Typ des dritten Elements der Tripel der Eingabesequenz.</typeparam>
        /// <param name="source">Die Eingabesequenz.</param>
        /// <returns>Ein Tripel von drei Einzelsequenzen, jeweils nach erstem, zweitem und drittem Element der
        /// Tripel der Eingangssequenz aufgeteilt.</returns>
        public static (IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>) Unzip3<T1, T2, T3>(this IEnumerable<(T1, T2, T3)> source)
        {
            Contract.Requires(source != null);

            return (source.Select(p => p.Item1), source.Select(p => p.Item2), source.Select(p => p.Item3));
        }

        /// <summary>
        /// Gibt das linke Element eines <see cref="ValueTuple{T1, T2}"/> zurück.
        /// </summary>
        /// <typeparam name="T1">Datentyp des linken Elements.</typeparam>
        /// <typeparam name="T2">Datentyp des rechten Elements.</typeparam>
        /// <param name="tuple">Das Tuple, dessen linkes (erstes) Element zurückgegeben werden soll.</param>
        /// <returns>Das linke Element des Tupels</returns>
        public static T1 Fst<T1, T2>(this (T1, T2) tuple) => tuple.Item1;

        /// <summary>
        /// Gibt das rechte Element eines <see cref="ValueTuple{T1, T2}"/> zurück.
        /// </summary>
        /// <typeparam name="T1">Datentyp des linken Elements.</typeparam>
        /// <typeparam name="T2">Datentyp des rechten Elements.</typeparam>
        /// <param name="tuple">Das Tuple, dessen rechtes (zweites) Element zurückgegeben werden soll.</param>
        /// <returns>Das rechte Element des Tupels</returns>
        public static T2 Snd<T1, T2>(this (T1, T2) tuple) => tuple.Item2;

        /// <summary>
        /// Führt eine Aggregatfunktion über alle Element einer Sequenz von Anfang bis Ende aus (von links nach rechts).
        /// Dabei wird ein Startwert übergeben.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source enumerable.</param>
        /// <param name="func">The aggregate function.</param>
        /// <param name="seed">The starting argument for the function.</param>
        /// <returns>Der Ergebniswert der Operation</returns>
        /// <remarks>
        /// Die Aggregatfunktion <c>func</c> bekommt beim ersten Aufruf das erste Element der Sequenz
        /// sowie den übergebenen Startwert <c>seed</c>. Der zweite Aufruf erhält das zweite Element
        /// sowie das Ergebnis des ersten Aufrufs und so fort.
        /// </remarks>
        public static TResult Foldl<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult, TResult> func, TResult seed)
        {
            Contract.Requires(source != null);
            Contract.Requires(func != null);

            TResult acc = seed;
            foreach (var e in source)
            {
                acc = func(e, acc);
            }

            return acc;
        }

        /// <summary>
        /// Führt eine Aggregatfunktion über alle Element einer Sequenz von Anfang bis Ende aus (von links nach rechts).
        /// Dabei wird das erste Element der Sequenz als Startwert verwendet.
        /// </summary>
        /// <typeparam name="T">Der Typ der Sequenz.</typeparam>
        /// <param name="source">Die Eingabesequenz.</param>
        /// <param name="func">The aggregate function.</param>
        /// <returns>Der Ergebniswert der Operation</returns>
        /// <remarks>
        /// Die Aggregatfunktion <c>func</c> bekommt beim ersten Aufruf das erste Element
        /// und das zweite der Sequenz. Bei zweiten Aufruf das dritte Element sowie das Ergebnis
        /// aus dem vorherigen Aufruf und so fort.
        /// </remarks>
        public static T Foldl1<T>(this IEnumerable<T> source, Func<T, T, T> func)
        {
            Contract.Requires(source != null);
            Contract.Requires(func != null);

            T result;
            using (IEnumerator<T> enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    throw new InvalidOperationException("Source contains no elements");
                }
                T tSource = enumerator.Current;
                while (enumerator.MoveNext())
                {
                    tSource = func(enumerator.Current, tSource);
                }
                result = tSource;
            }
            return result;
        }

        /// <summary>
        /// Führt eine Aggregatfunktion über alle Element einer Sequenz vom Ende bis zum Anfang aus (von rechts nach links).
        /// Dabei wird ein Startwert übergeben.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source enumerable.</param>
        /// <param name="func">The aggregate function.</param>
        /// <param name="seed">The starting argument for the function.</param>
        /// <returns>Der Ergebniswert der Operation</returns>
        /// <remarks>
        /// Die Aggregatfunktion <c>func</c> bekommt beim ersten Aufruf das letzte Element der Sequenz
        /// sowie den übergebenen Startwert <c>seed</c>. Der zweite Aufruf erhält das vorletzte Element
        /// sowie das Ergebnis des ersten Aufrufs und so fort.
        /// </remarks>
        public static TResult Foldr<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult, TResult> func, TResult seed)
        {
            Contract.Requires(source != null);
            Contract.Requires(func != null);

            TResult acc = seed;
            var src = source.ToArray();
            for (int i = src.Length - 1; i >= 0; i--)
            {
                var e = src[i];
                acc = func(e, acc);
            }

            return acc;
        }

        /// <summary>
        /// Führt eine Aggregatfunktion über alle Element einer Sequenz vom Ende bis zum Anfang aus (von rechts nach links).
        /// Dabei wird das letzte Element der Sequenz als Startwert verwendet.
        /// </summary>
        /// <typeparam name="T">Der Typ der Sequenz.</typeparam>
        /// <param name="source">Die Eingabesequenz.</param>
        /// <param name="func">The aggregate function.</param>
        /// <returns>Der Ergebniswert der Operation</returns>
        /// <remarks>
        /// Die Aggregatfunktion <c>func</c> bekommt beim ersten Aufruf das letzte Element
        /// und das vorletzte der Sequenz. Bei zweiten Aufruf das drittletzte Element sowie das Ergebnis
        /// aus dem vorherigen Aufruf und so fort.
        /// </remarks>
        public static T Foldr1<T>(this IEnumerable<T> source, Func<T, T, T> func)
        {
            Contract.Requires(source != null);
            Contract.Requires(func != null);

            var src = source.ToArray();
            if (src.Length <= 0)
            {
                throw new InvalidOperationException("Source contains no elements.");
            }

            var acc = src[src.Length - 1];
            for (int i = src.Length - 2; i >= 0; i--)
            {
                var e = src[i];
                acc = func(e, acc);
            }

            return acc;
        }
    }
}