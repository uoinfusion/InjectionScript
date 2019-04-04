using InjectionScript.Runtime.ObjectTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Runtime
{
    public class InjectionApi
    {
        private readonly IApiBridge bridge;
        private readonly ITimeSource timeSource;
        public InjectionApiUO UO { get; }

        public InjectionApi(IApiBridge bridge, Metadata metadata, Globals globals, ITimeSource timeSource, Paths paths, Objects objects)
        {
            this.bridge = bridge;
            UO = new InjectionApiUO(bridge, this, metadata, globals, timeSource, paths, objects);
            Register(metadata);
            this.timeSource = timeSource;
        }

        private void Register(Metadata metadata)
        {
            metadata.Add(new NativeSubrutineDefinition("wait", (Action<int>)Wait));
            metadata.Add(new NativeSubrutineDefinition("str", (Func<int, string>)InternalSubrutines.Str));
            metadata.Add(new NativeSubrutineDefinition("str", (Func<double, string>)InternalSubrutines.Str));
            metadata.Add(new NativeSubrutineDefinition("str", (Func<string, string>)InternalSubrutines.Str));
            metadata.Add(new NativeSubrutineDefinition("val", (Func<string, InjectionValue>)InternalSubrutines.Val));
            metadata.Add(new NativeSubrutineDefinition("len", (Func<string, int>)InternalSubrutines.Len));
            metadata.Add(new NativeSubrutineDefinition("len", (Func<int, int>)InternalSubrutines.Len));
            metadata.Add(new NativeSubrutineDefinition("len", (Func<double, int>)InternalSubrutines.Len));
            metadata.Add(new NativeSubrutineDefinition("left", (Func<string, int, string>)InternalSubrutines.Left));
            metadata.Add(new NativeSubrutineDefinition("right", (Func<string, int, string>)InternalSubrutines.Right));
            metadata.Add(new NativeSubrutineDefinition("mid", (Func<string, int, int, string>)InternalSubrutines.Mid));
            metadata.Add(new NativeSubrutineDefinition("GetArrayLength", (Func<InjectionValue, InjectionValue>)InternalSubrutines.GetArrayLength));
            metadata.Add(new NativeSubrutineDefinition("Now", (Func<int>)Now));
            metadata.Add(new NativeSubrutineDefinition("File", (Func<InjectionValue, InjectionValue>)File));

            metadata.AddIntrinsicVariable(new NativeSubrutineDefinition("true", (Func<int>)(() => 1)));
            metadata.AddIntrinsicVariable(new NativeSubrutineDefinition("false", (Func<int>)(() => 0)));
        }

        public InjectionValue File(InjectionValue fileName) => new InjectionValue(FileObject.Create((string)fileName));

        public int Now() => (int)timeSource.SinceStart.TotalMilliseconds;

        public void Wait(int ms) => bridge.Wait(ms);
    }
}
