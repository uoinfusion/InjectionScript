using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace InjectionScript
{
    public class MessageCollection : IEnumerable<Message>
    {
        private readonly Message[] messages;

        public static MessageCollection Empty { get; } = new MessageCollection(Array.Empty<Message>());

        public MessageCollection(IEnumerable<Message> messages)
        {
            this.messages = messages.ToArray();
        }

        public MessageCollection(Message message)
        {
            this.messages = new Message[] { message };
        }

        public IEnumerator<Message> GetEnumerator()
        {
            return ((IEnumerable<Message>)messages).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return messages.GetEnumerator();
        }

        public override string ToString()
        {
            return messages.Select(m => m.ToString()).Aggregate((l, r) => l + "\n" + r);
        }
    }
}
