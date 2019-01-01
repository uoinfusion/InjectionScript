using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Runtime
{
    public class InjectionApi
    {
        private readonly DateTime timerStart;
        private readonly IApiBridge bridge;
        private readonly Objects objects = new Objects();
        public InjectionApiUO UO { get; }

        public InjectionApi(IApiBridge bridge, Metadata metadata, Globals globals)
        {
            this.bridge = bridge;
            UO = new InjectionApiUO(bridge, this, metadata, globals);
            Register(metadata);
            timerStart = DateTime.UtcNow;
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
            metadata.Add(new NativeSubrutineDefinition("GetArrayLength", InternalSubrutines.GetArrayLength));
            metadata.Add(new NativeSubrutineDefinition("Now", (Func<int>)Now));

            metadata.AddIntrinsicVariable(new NativeSubrutineDefinition("true", (Func<int>)(() => 1)));
            metadata.AddIntrinsicVariable(new NativeSubrutineDefinition("false", (Func<int>)(() => 0)));
        }

        public int GetObject(string id)
        {
            if (id.Equals("finditem", StringComparison.OrdinalIgnoreCase))
                return bridge.FindItem;
            if (id.Equals("self", StringComparison.OrdinalIgnoreCase))
                return bridge.Self;
            if (id.Equals("lastcorpse", StringComparison.OrdinalIgnoreCase))
                return bridge.LastCorpse;
            if (id.Equals("lasttarget", StringComparison.OrdinalIgnoreCase))
                return bridge.LastTarget;
            if (id.Equals("laststatus", StringComparison.OrdinalIgnoreCase))
                return bridge.LastStatus;

            if (objects.TryGet(id, out var value))
                return value;

            if (NumberConversions.TryStr2Int(id, out value))
                return value;

            return 0;
        }

        public void SetObject(string name, int value)
        {
            objects.Set(name, value);
        }

        public void Wait(int ms) => bridge.Wait(ms);

        public int Now()
        {
            var duration = DateTime.UtcNow - timerStart;

            return (int)duration.TotalMilliseconds;
        }
    }
}
