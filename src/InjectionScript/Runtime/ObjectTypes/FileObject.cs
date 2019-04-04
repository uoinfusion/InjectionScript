using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Runtime.ObjectTypes
{
    internal class FileObject : InjectionObject
    {
        private readonly string fileName;

        private FileStream stream;
        private TextReader reader;
        private TextWriter writer;

        public FileObject(string fileName) : base("File")
        {
            this.fileName = fileName;

            Register(new NativeSubrutineDefinition("Open", (Action)Open));
            Register(new NativeSubrutineDefinition("Create", (Action)Create));
            Register(new NativeSubrutineDefinition("Close", (Action)Close));
            Register(new NativeSubrutineDefinition("WriteLn", (Action<InjectionValue>)WriteLn));
            Register(new NativeSubrutineDefinition("ReadNumber", (Func<InjectionValue>)ReadNumber));
        }

        internal static FileObject Create(string fileName)
            => new FileObject(fileName);

        public void Open()
        {
            if (stream != null)
                stream.Dispose();

            stream = File.OpenRead(fileName);
            if (writer != null)
            {
                writer.Dispose();
                writer = null;
            }
            reader = new StreamReader(stream);
        }

        public void Create()
        {
            if (stream != null)
                stream.Dispose();

            stream = File.OpenWrite(fileName);
            if (reader != null)
            {
                reader.Dispose();
                reader = null;
            }
            writer = new StreamWriter(stream);
        }

        public void Close()
        {
            if (stream != null)
            {
                stream.Flush();
                stream.Dispose();
                stream = null;
            }

            reader = null;
            writer = null;
        }

        public void WriteLn(InjectionValue text)
        {
            if (writer != null)
            {
                writer.WriteLine((string)text);
                writer.Flush();
            }
        }

        public InjectionValue ReadNumber()
        {
            if (reader == null)
                throw new InvalidOperationException($"File {fileName} is closed. You have open it first.");

            var line = reader.ReadLine();
            if (int.TryParse(line, out int num))
                return new InjectionValue(num);

            return InjectionValue.Zero;
        }
    }
}
