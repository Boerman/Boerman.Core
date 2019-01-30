using System;
using System.Runtime.Serialization;

namespace Boerman.Core
{
	public class PoemException : Exception
    {
        public PoemException() {
            //base.Message = _poems[new Random().Next(_poems.Length)];
        }

        public PoemException(string message) : base(message)
        {
        }

        public PoemException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PoemException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        private readonly string[] _poems = new[] {
@"Roses are red,
Violets are blue.

Unexpected '{'
on line 32.",
@"If you want to declare you are right, 
When wrong stares me in the face.
And you want me to believe, 
What you perceive.
Do it! 
There is no argument.",
            @"01001000 01100101 01101100 01101100 01101111 00100000 01110111 01101111 01110010 01101100 01100100 00100001"
        };
    }
}
