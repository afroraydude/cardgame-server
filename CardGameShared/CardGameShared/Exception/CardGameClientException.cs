namespace CardGameShared.Exception
{
    public class CardGameClientException : CardGameException
    {
        public CardGameClientException()
        {
        }

        internal CardGameClientException(string message) : base(message)
        {
        }

        internal CardGameClientException(string message, System.Exception inner) : base(message, inner)
        {
        }

        public CardGameClientException(ErrorCode errorCode) : base(errorCode)
        {
        }

        public CardGameClientException(string message, ErrorCode errorCode) : base(message, errorCode)
        {
        }
    }
}