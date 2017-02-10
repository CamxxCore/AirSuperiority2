using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirSuperiority.Core
{
    public abstract class ScriptExtension : ScriptComponent, IScriptExtension, IDisposable
    {
        public virtual void Dispose()
        { }
    }
}
