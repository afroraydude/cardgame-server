namespace CardGameShared.Exception
{
    public class CardGameException : System.Exception
    {
        private ErrorCode _code { get; }

        public CardGameException() : this(ErrorCode.Unknown)
        {
        }

        internal CardGameException(string message)
            : base(message)
        {
        }

        internal CardGameException(string message, System.Exception inner)
            : base(message, inner)
        {
        }

        public CardGameException(ErrorCode errorCode) : this(errorCode.ToString())
        {
            _code = errorCode;
        }

        public CardGameException(string message, ErrorCode errorCode) : this($"{errorCode}: \"{message}\"")
        {
            _code = errorCode;
        }
    }
}