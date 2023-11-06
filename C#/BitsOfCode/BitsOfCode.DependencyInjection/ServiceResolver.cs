using System.Reflection;

namespace BitsOfCode.DependencyInjection
{
    public class ServiceResolver
    {
        private Dictionary<Type, object?> _Services = new Dictionary<Type, object?>();

        public object? ResolveService(Type type)
        {
            if (!_Services.ContainsKey(type))
                return null;
            return _Services[type];
        }

        public void AddSingleton<TService>() where TService : class, new()
        {
            _Services.Add(typeof(TService), new TService());
        }

        // TODO : AddTransient, will have to use a factory (might as well create a wrapper for singleton)

        public TType InjectDependencies<TType>()
        {
            Type myClassType = typeof(TType);
            // Maybe use an attribut to specify the constructor to use
            ConstructorInfo constructor = myClassType.GetConstructors().First();

            ParameterInfo[] parameters = constructor.GetParameters();
            List<object> parametersInstances = new List<object>();

            foreach (ParameterInfo parameter in parameters)
            {
                var instance = ResolveService(parameter.ParameterType);
                if (instance == null) throw new ArgumentNullException($"Can't resolve service '{parameter.ParameterType}'.");

                parametersInstances.Add(instance);
            }

            return (TType)Activator.CreateInstance(typeof(TType), args: parametersInstances.ToArray());
        }
    }
}