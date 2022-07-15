using System;

namespace WeekendBot.Utils
{
    /// <summary>
    /// Class containing extension methods as guards.
    /// </summary>
    internal static class GuardExtensions
    {
        /// <summary>
        /// Guards that <param name="argument"> is not <c>null</c>.</param>
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="argument"/></typeparam>
        /// <param name="argument">The argument to guard.</param>
        /// <param name="argumentName">The name of the argument.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="argument"/> is <c>null</c>.</exception>
        public static void IsNotNull<T>(this T argument, string argumentName) where T : class
        {
            if (argument == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }
    }
}