namespace Pustokk.CustomExceptions.BookExceptions
{
    public class InvalidNotFoundExceptionException:Exception
    {
        public string PropertyName { get; set; }

        public InvalidNotFoundExceptionException()
        {

        }
        public InvalidNotFoundExceptionException(string propertyName, string message) : base(message)
        {
            PropertyName = propertyName;
        }

    }
}
