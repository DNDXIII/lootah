using System;

namespace Shared.Utils
{
    public static class Preconditions
    {
        public static T CheckNotNull<T>(T reference, string message = null)
        {
            if (reference is UnityEngine.Object obj && obj == null || reference is null)
            {
                throw new ArgumentNullException(message);
            }

            return reference;
        }

        public static void CheckState(bool expression, string messageTemplate, params object[] messageArgs)
        {
            CheckState(expression, string.Format(messageTemplate, messageArgs));
        }

        public static void CheckState(bool expression, string message = null)
        {
            if (expression)
            {
                return;
            }

            throw message == null ? new InvalidOperationException() : new InvalidOperationException(message);
        }
    }
}