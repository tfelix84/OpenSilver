using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace OpenSilver.Compiler
{
    internal class CustomAssemblyLoadContext : IDisposable
    {
        private AssemblyLoadContext _context;
        private MetadataLoadContext _reflectionContext;

        private bool isDisposed = false;

        public CustomAssemblyLoadContext()
        {
            var path = Path.GetDirectoryName(typeof(CustomAssemblyLoadContext).Assembly.Location);
            var corelibPath = Path.GetDirectoryName(typeof(object).Assembly.Location);

            var paths = Directory.GetFiles(path, "*.dll");
            var paths2 = Directory.GetFiles(corelibPath, "*.dll");
            var resolver = new PathAssemblyResolver(paths.Union(paths2));

            _context = new AssemblyLoadContext("myContext", true);
            _reflectionContext = new MetadataLoadContext(resolver);
        }

        private void ThrowIfDisposed()
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException("Already disposed");
            }
        }

        public void Dispose()
        {
            ThrowIfDisposed();

            isDisposed = true;

            var contextReference = new WeakReference(this._context, trackResurrection: true);
            this._context.Unload();

            _reflectionContext.Dispose();

            _context = null;
            _reflectionContext = null;

            // AssemblyLoadContext unloading is an asynchronous operation
            for (int i = 0; contextReference.IsAlive && i < 10; i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        public Assembly LoadAssembly(string assemblyString)
        {
            ThrowIfDisposed();

            var assemblyName = new AssemblyName(assemblyString)
            {
                CultureInfo = System.Globalization.CultureInfo.InvariantCulture
            };

            return _context.LoadFromAssemblyName(assemblyName);
        }

        public Assembly LoadFromAssemblyPath(string assemblyPath)
        {
            ThrowIfDisposed();

            return _context.LoadFromAssemblyPath(assemblyPath);
        }

        public Assembly LoadFromAssemblyName(AssemblyName assemblyName)
        {
            ThrowIfDisposed();

            return _context.LoadFromAssemblyName(assemblyName);
        }

        public Assembly ReflectionOnlyLoadFromAssemblyName(AssemblyName assemblyName)
        {
            ThrowIfDisposed();

            return _reflectionContext.LoadFromAssemblyName(assemblyName);
        }

        public Assembly ReflectionOnlyLoad(string assemblyString)
        {
            ThrowIfDisposed();

            var assemblyName = new AssemblyName(assemblyString)
            {
                CultureInfo = System.Globalization.CultureInfo.InvariantCulture
            };

            return _reflectionContext.LoadFromAssemblyName(assemblyName);
        }

        public Assembly ReflectionOnlyLoadFromPath(string assemblyPath)
        {
            ThrowIfDisposed();

            return _reflectionContext.LoadFromAssemblyPath(assemblyPath);
        }
    }
}
