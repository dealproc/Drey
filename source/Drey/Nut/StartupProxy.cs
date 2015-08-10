using System;
using System.Reflection;

namespace Drey.Nut
{
    public class StartupProxy : MarshalByRefObject, IDisposable
    {
        Type _type;
        object _object;

        public void Instantiate(string pathAndFile, string typeName, params object[] args)
        {
            var assembly = Assembly.LoadFrom(pathAndFile);
            _type = assembly.GetType(typeName);
            _object = Activator.CreateInstance(_type, args);
        }

        public void Invoke(string methodName, params object[] args)
        {
            var mInfo = _type.GetMethod(methodName);
            mInfo.Invoke(_object, args);
        }

        public void Dispose()
        {
            if (_object is IDisposable)
            {
                (_object as IDisposable).Dispose();
            }
        }
    }
}