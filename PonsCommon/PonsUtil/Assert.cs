using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace PonsUtil
{
    public static class DbAssert
    {
        /// <summary>
        /// Checks the value of the supplied <paramref name="argument"/> and throws an
        /// <see cref="System.ArgumentNullException"/> if it is <see langword="null"/>.
        /// </summary>
        /// <param name="argument">The object to check.</param>
        /// <param name="name">The argument name.</param>
        /// <exception cref="System.ArgumentNullException">
        /// If the supplied <paramref name="argument"/> is <see langword="null"/>.
        /// </exception>
        public static void ArgumentNotNull(object argument, string name)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(
                    name,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Argument '{0}' cannot be null.", name));
            }
        }

        /// <summary>
        /// Checks the value of the supplied <paramref name="argument"/> and throws an
        /// <see cref="System.ArgumentNullException"/> if it is <see langword="null"/>.
        /// </summary>
        /// <param name="argument">The object to check.</param>
        /// <param name="name">The argument name.</param>
        /// <param name="message">
        /// An arbitrary message that will be passed to any thrown
        /// <see cref="System.ArgumentNullException"/>.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// If the supplied <paramref name="argument"/> is <see langword="null"/>.
        /// </exception>
        public static void ArgumentNotNull(object argument, string name, string message)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(name, message);
            }
        }


        /// <summary>
        /// Checks the value of the supplied <see cref="ICollection"/> <paramref name="argument"/> and throws
        /// an <see cref="ArgumentNullException"/> if it is <see langword="null"/> or contains no elements.
        /// </summary>
        /// <param name="argument">The array or collection to check.</param>
        /// <param name="name">The argument name.</param>
        /// <exception cref="System.ArgumentNullException">
        /// If the supplied <paramref name="argument"/> is <see langword="null"/> or
        /// contains no elements.
        /// </exception>
        public static void ArgumentHasLength(ICollection argument, string name)
        {
            if (!DbArrayUtils.HasLength(argument))
            {
                throw new ArgumentNullException(
                    name,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Argument '{0}' cannot be null or resolve to an empty array", name));
            }
        }

        /// <summary>
        /// Checks the value of the supplied <see cref="ICollection"/> <paramref name="argument"/> and throws
        /// an <see cref="ArgumentNullException"/> if it is <see langword="null"/> or contains no elements.
        /// </summary>
        /// <param name="argument">The array or collection to check.</param>
        /// <param name="name">The argument name.</param>
        /// <param name="message">An arbitrary message that will be passed to any thrown <see cref="System.ArgumentNullException"/>.</param>
        /// <exception cref="System.ArgumentNullException">
        /// If the supplied <paramref name="argument"/> is <see langword="null"/> or
        /// contains no elements.
        /// </exception>
        public static void ArgumentHasLength(ICollection argument, string name, string message)
        {
            if (!DbArrayUtils.HasLength(argument))
            {
                throw new ArgumentNullException(name, message);
            }
        }

        /// <summary>
        /// Checks the value of the supplied <see cref="ICollection"/> <paramref name="argument"/> and throws
        /// an <see cref="ArgumentException"/> if it is <see langword="null"/>, contains no elements or only null elements.
        /// </summary>
        /// <param name="argument">The array or collection to check.</param>
        /// <param name="name">The argument name.</param>
        /// <exception cref="System.ArgumentNullException">
        /// If the supplied <paramref name="argument"/> is <see langword="null"/>, 
        /// contains no elements or only null elements.
        /// </exception>
        public static void ArgumentHasElements(ICollection argument, string name)
        {
            if (!DbArrayUtils.HasElements(argument))
            {
                throw new ArgumentException(
                    name,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Argument '{0}' must not be null or resolve to an empty collection and must contain non-null elements", name));
            }
        }


        /// <summary>
        /// Checks whether the specified <paramref name="argument"/> can be cast 
        /// into the <paramref name="requiredType"/>.
        /// </summary>
        /// <param name="argument">
        /// The argument to check.
        /// </param>
        /// <param name="argumentName">
        /// The name of the argument to check.
        /// </param>
        /// <param name="requiredType">
        /// The required type for the argument.
        /// </param>
        /// <param name="message">
        /// An arbitrary message that will be passed to any thrown
        /// <see cref="System.ArgumentException"/>.
        /// </param>
        public static void AssertArgumentType(object argument, string argumentName, Type requiredType, string message)
        {
            if (argument != null && requiredType != null && !requiredType.IsAssignableFrom(argument.GetType()))
            {
                throw new ArgumentException(message, argumentName);
            }
        }


        /// <summary>
        ///  Assert a boolean expression, throwing <code>ArgumentException</code>
        ///  if the test result is <code>false</code>.
        /// </summary>
        /// <param name="expression">a boolean expression.</param>
        /// <param name="message">The exception message to use if the assertion fails.</param>
        /// <exception cref="ArgumentException">
        /// if expression is <code>false</code>
        /// </exception>
        public static void IsTrue(bool expression, string message)
        {
            if (!expression)
            {
                throw new ArgumentException(message);
            }
        }

        /// <summary>
        ///  Assert a boolean expression, throwing <code>ArgumentException</code>
        ///  if the test result is <code>false</code>.
        /// </summary>
        /// <param name="expression">a boolean expression.</param>
        /// <exception cref="ArgumentException">
        /// if expression is <code>false</code>
        /// </exception>
        public static void IsTrue(bool expression)
        {
            IsTrue(expression, "[Assertion failed] - this expression must be true");
        }

        /// <summary>
        /// Assert a bool expression, throwing <code>InvalidOperationException</code>
        /// if the expression is <code>false</code>.
        /// </summary>
        /// <param name="expression">a boolean expression.</param>
        /// <param name="message">The exception message to use if the assertion fails</param>
        /// <exception cref="InvalidOperationException">if expression is <code>false</code></exception>
        public static void State(bool expression, string message)
        {
            if (!expression)
            {
                throw new InvalidOperationException(message);
            }
        }

        #region Constructor (s) / Destructor

        // CLOVER:OFF

        // CLOVER:ON

        #endregion
    }
}
