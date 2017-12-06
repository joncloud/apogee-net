using System;

namespace Apogee
{
    interface IApogeeFlusher
    {
        void Flush();
        void Register(Action fn);
    }
}
