using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Debugging
{
    internal class RingStringAppender
    {
        private readonly int bufferCapacity;
        private readonly int lineCapacity;
        private Queue<StringBuilder> buffer;
        private StringBuilder currentLine;

        public RingStringAppender(int bufferCapacity, int lineCapacity)
        {
            buffer = new Queue<StringBuilder>(this.bufferCapacity);
            this.bufferCapacity = bufferCapacity;
            this.lineCapacity = lineCapacity;
        }

        public void AppendLine(string str)
        {
            Append(str);
            AppendLine();
        }

        public void Append(string str)
        {
            if (currentLine == null)
                currentLine = GetNewLineBuilder();

            currentLine.Append(str);
        }

        private StringBuilder GetNewLineBuilder()
        {
            if (buffer.Count < bufferCapacity)
            {
                var newBuilder = new StringBuilder(lineCapacity);
                buffer.Enqueue(newBuilder);
                return newBuilder;
            }

            var recycledBuilder = buffer.Dequeue();
            recycledBuilder.Clear();
            buffer.Enqueue(recycledBuilder);
            return recycledBuilder;
        }

        public void AppendLine()
        {
            if (currentLine != null)
                currentLine.AppendLine();
            currentLine = GetNewLineBuilder();
        }

        public void Dump(StringBuilder output)
        {
            var newBuffer = new Queue<StringBuilder>(bufferCapacity);

            while (buffer.Count > 0)
            {
                var line = buffer.Dequeue();
                output.Append(line);
                line.Clear();

                newBuffer.Enqueue(line);
            }

            buffer = newBuffer;
        }
    }
}
