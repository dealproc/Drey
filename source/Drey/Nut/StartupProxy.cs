using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Drey.Nut
{
    public class StartupProxy : MarshalByRefObject, IDisposable
    {
        ShellStartOptions _options;
        Type _type;
        object _object;

        public void SetStartOptions(ShellStartOptions options)
        {
            _options = options;
        }

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

        public bool Shutdown()
        {
            var shutdownMethod = _type.GetMethod("Shutdown");

            if (shutdownMethod != null)
            {
                return (bool)shutdownMethod.Invoke(_object, null);
            }

            return true;
        }

        public Assembly ResolveAssemblyInDomain(object sender, ResolveEventArgs args)
        {
            var asmName = args.Name + ".dll";
            Console.WriteLine(asmName);


            var foundDll = (new[] { Path.GetDirectoryName(_options.DllPath), Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) })
                    .Select(fullPath => Path.Combine(fullPath, asmName))
                    .Select(fullPath => File.Exists(fullPath) ? Assembly.LoadFrom(fullPath) : null)
                    .Where(asm => asm != null)
                    .FirstOrDefault();

            Console.WriteLine("DLL Found? {0}", foundDll != null);

            return foundDll;

        }
    }
}