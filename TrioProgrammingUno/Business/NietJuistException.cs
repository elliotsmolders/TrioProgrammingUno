using System.Runtime.Serialization;

namespace TrioProgrammingUno.Business
{
    [Serializable]
    internal class NietJuistException : Exception
    {
        public NietJuistException()
        {
        }

        public NietJuistException(string? message) : base(message)
        {
        }

        public NietJuistException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected NietJuistException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}