namespace CardGameShared.Exception
{
    public class CardGameServerException : CardGameException
    {

        public CardGameServerException()
        {
        }

        internal CardGameServerException(string message) : base(message)
        {
        }

        internal CardGameServerException(string message, System.Exception inner) : base(message, inner)
        {
        }

        public CardGameServerException(ErrorCode errorCode) : base(errorCode)
        {
        }

        public CardGameServerException(string message, ErrorCode errorCode) : base(message, errorCode)
        {
        }
    }
}