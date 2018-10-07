using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Ultz.Jfp.IO;
using Ultz.SimpleServer.Handlers;
using Ultz.SimpleServer.Internals;

namespace Ultz.Jfp.SimpleServer
{
    public class JfpAttributeHandlerResolver : IAttributeHandlerResolver
    {
        private List<Type> _types;

        public void Register<T>() where T : JfpAttribute
        {
            _types.Add(typeof(T));
        }

        public void Deregister<T>() where T : JfpAttribute
        {
            _types.Remove(typeof(T));
        }

        public IEnumerable<IHandler> GetHandlers(object obj)
        {
            return from method in obj.GetType().GetMethods()
                from attr in _types
                where method.GetCustomAttributes(attr, true).Length != 0
                select new Handler(((JfpAttribute) Activator.CreateInstance(attr)).MessageType, method, obj);
        }

        private class Handler : IHandler
        {
            private string _type;
            private MethodInfo _methodInfo;
            private object _instance;

            public Handler(string type, MethodInfo info, object instance)
            {
                _instance = instance;
            }

            public bool CanHandle(IRequest request)
            {
                return ((JfpContext) request).MessageType == _type;
            }

            public void Handle(IContext context)
            {
                var parameters = new List<object>();
                foreach (var param in _methodInfo.GetParameters())
                {
                    if (param.ParameterType == typeof(JfpContext))
                        parameters.Add((JfpContext) context);
                    else
                        parameters.Add(null);
                }

                _methodInfo.Invoke(_instance, parameters.ToArray());
            }
        }
    }
}