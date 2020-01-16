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

namespace Interlook.Monads
{
    /// <summary>
    /// An implementation of the state monad.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public delegate StateEntry<TState, TValue> State<TState, TValue>(TState state);

    /// <summary>
    /// The result value of the state monad.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public class StateEntry<TState, TValue>
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        public TValue Value { get; }

        /// <summary>
        /// Gets the state.
        /// </summary>
        public TState State { get; }

        internal StateEntry(TState state, TValue value)
        {
            Value = value;
            State = state;
        }
    }

    /// <summary>
    /// Extension and factory methods for <see cref="StateEntry{TState, TValue}"/>
    /// </summary>
    public static class State
    {
        /// <summary>
        /// Returns a new monadic state object with a certain or default value.
        /// </summary>
        /// <typeparam name="TState">The type of the state.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The value for the object. Defaults to <c>default(TValue)</c> if not specified.</param>
        /// <returns>A new <see cref="State{TState, TValue}"/> instance with the specified or the default value.</returns>
        public static State<TState, TValue> ReturnState<TState, TValue>(TValue value = default(TValue))
            => state => new StateEntry<TState, TValue>(state, value);

        /// <summary>
        /// Returns a new monadic state object, for which
        /// the value and state have the same data type and which
        /// applies a function to the state, to derive the value.
        /// </summary>
        /// <typeparam name="TState">The type of the state AND the value.</typeparam>
        /// <param name="func">The function to derive the value from the state.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="func"/> was <c>null</c>.</exception>
        public static State<TState, TState> Get<TState>(Func<TState, TState> func)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));

            return (TState state) => new StateEntry<TState, TState>(state, func(state));
        }

        /// <summary>
        /// Returns a new monadic state object, for which
        /// the value and state have the same data type and content.
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <returns></returns>
        public static State<TState, TState> Get<TState>() => (TState state) => new StateEntry<TState, TState>(state, state);

        /// <summary>
        /// Returns a new monadic state object with a specified state
        /// but without any actual value.
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public static State<TState, Unit> Put<TState>(TState state)
        {
            return _ => new StateEntry<TState, Unit>(state, Unit.Default);
        }

        /// <summary>
        /// Binds a function to the state of the monadic state object.
        /// </summary>
        /// <typeparam name="TState">The type of the state.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="obj">The monadic state object.</param>
        /// <param name="stateMapper">The state mapper.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="obj"/> or <paramref name="stateMapper"/> was <c>null</c>.
        /// </exception>
        public static State<TState, TValue> With<TState, TValue>(this State<TState, TValue> obj, Func<TState, TState> stateMapper)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (stateMapper == null) throw new ArgumentNullException(nameof(stateMapper));

            return state =>
            {
                var previous = obj(state);
                return new StateEntry<TState, TValue>(stateMapper(previous.State), previous.Value);
            };
        }

        /// <summary>
        /// Binds a function to the state of the monadic state object.
        /// </summary>
        /// <typeparam name="TState">The type of the state.</typeparam>
        /// <typeparam name="TValue">The type of the value of the original state object.</typeparam>
        /// <typeparam name="TValueNew">The type of the value of the resulting state object.</typeparam>
        /// <param name="obj">The monadic state object.</param>
        /// <param name="valueMapper">The value mapping function.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="obj"/> or <paramref name="valueMapper"/> was <c>null</c>.
        /// </exception>
        public static State<TState, TValueNew> Map<TState, TValue, TValueNew>(this State<TState, TValue> obj, Func<TValue, TValueNew> valueMapper)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (valueMapper == null) throw new ArgumentNullException(nameof(valueMapper));

            return (TState state) =>
            {
                var previous = obj(state);
                return new StateEntry<TState, TValueNew>(previous.State, valueMapper(previous.Value));
            };
        }

        /// <summary>
        /// Binds a function to the value of the monadic state object.
        /// </summary>
        /// <typeparam name="TState">The type of the state.</typeparam>
        /// <typeparam name="TValue">The type of the value of the original state object.</typeparam>
        /// <typeparam name="TValueNew">The type of the value of the resulting state object.</typeparam>
        /// <param name="obj">The monadic state object.</param>
        /// <param name="func">The value mapping function.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="obj"/> or <paramref name="func"/> was <c>null</c>.
        /// </exception>
        public static State<TState, TValueNew> Bind<TState, TValue, TValueNew>(this State<TState, TValue> obj, Func<TValue, State<TState, TValueNew>> func)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (func == null) throw new ArgumentNullException(nameof(func));

            return (TState state) =>
            {
                var previous = obj(state);
                var secondary = func(previous.Value)(previous.State);

                return new StateEntry<TState, TValueNew>(secondary.State, secondary.Value);
            };
        }

        /// <summary>
        /// For LINQ support. Just calls <see cref="Map{TState, TValue, TValueNew}(State{TState, TValue}, Func{TValue, TValueNew})"/>
        /// </summary>
        /// <typeparam name="TState">The type of the state.</typeparam>
        /// <typeparam name="TValue">The type of the value of the original state object.</typeparam>
        /// <typeparam name="TValueNew">The type of the value of the resulting state object.</typeparam>
        /// <param name="obj">The monadic state object.</param>
        /// <param name="valueMapper">The value mapping function.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="obj"/> or <paramref name="valueMapper"/> was <c>null</c>.
        /// </exception>
        public static State<TState, TValueNew> Select<TState, TValue, TValueNew>(this State<TState, TValue> obj, Func<TValue, TValueNew> valueMapper)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (valueMapper == null) throw new ArgumentNullException(nameof(valueMapper));

            return (TState state) =>
            {
                var previous = obj(state);
                return new StateEntry<TState, TValueNew>(previous.State, valueMapper(previous.Value));
            };
        }

        /// <summary>
        /// Selects the many.
        /// </summary>
        /// <typeparam name="TState">The type of the state.</typeparam>
        /// <typeparam name="TValue">The type of the value of the original state object.</typeparam>
        /// <typeparam name="TSelect">The type of the value of the second state object.</typeparam>
        /// <typeparam name="TValueNew">The type of the value of the resulting state object.</typeparam>
        /// <param name="obj">The self.</param>
        /// <param name="collector">A function selecting a second state object from the value of the first one.</param>
        /// <param name="resultSelector">A function to select the result of the above state objects.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="obj"/>, <paramref name="collector"/> or <paramref name="resultSelector"/> is <c>null</c>.
        /// </exception>
        public static State<TState, TValueNew> SelectMany<TState, TValue, TSelect, TValueNew>(
            this State<TState, TValue> obj,
            Func<TValue, State<TState, TSelect>> collector,
            Func<TValue, TSelect, TValueNew> resultSelector
            )
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (collector == null) throw new ArgumentNullException(nameof(collector));
            if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));

            return state =>
            {
                var previous = obj(state);
                var secondary = collector(previous.Value)(previous.State);
                var result = resultSelector(previous.Value, secondary.Value);
                return new StateEntry<TState, TValueNew>(secondary.State, result);
            };
        }

        /// <summary>
        /// Evaluates the state object and returns the result object.
        /// </summary>
        /// <typeparam name="TState">The type of the state.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="obj">The monadic state object.</param>
        /// <param name="state">The initial state to use.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="obj"/> was <c>null</c>.</exception>
        public static StateEntry<TState, TValue> GetResult<TState, TValue>(this State<TState, TValue> obj, TState state)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            return obj(state);
        }

        /// <summary>
        /// Lazily evaluates the state object and returns the result object.
        /// </summary>
        /// <typeparam name="TState">The type of the state.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="obj">The monadic state object.</param>
        /// <param name="state">The initial state to use.</param>
        /// <returns>A function that actually evaluates the state object.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="obj" /> was <c>null</c>.</exception>
        public static Func<StateEntry<TState, TValue>> GetResultLazy<TState, TValue>(this State<TState, TValue> obj, TState state)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            return () => new Lazy<StateEntry<TState, TValue>>(() => obj(state)).Value;
        }
    }
}