using System;

namespace Apogee
{
    class ApogeeFlusher : IApogeeFlusher
    {
        Action _fn = () => { };

        public void Flush() =>
            _fn();

        public void Register(Action fn) =>
            _fn = (Action)Delegate.Combine(_fn, fn);
    }
}
