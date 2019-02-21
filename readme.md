# NanoIoC

A tiny IoC container, does exactly what you want, and only that.

## Getting Started

`Container.Global` is a static instance of `IContainer`. You can use this as your entry point.

### Manually Registering Dependencies

Use either of these methods to register a concrete type as an abstract type:

```
void IContainer.Register<TAbstract, TConcrete>(ServiceLifetime serviceLifetime = ServiceLifetime.Singleton);
void IContainer.Register(Type abstract, Type concrete, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton);
```

You can tell NanoIoC to construct a concrete type for an abstract type in a custom way, using either of these methods:

```
void Register(Type abstractType, Func<IResolverContainer, object> ctor, ServiceLifetime serviceLifetime);
void Register<TAbstract>(this IContainer container, Func<IResolverContainer, TAbstract> ctor, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton);
```

You will typically want to put your registrations inside an `IContainerRegistry`.

NanoIoC will find all `IContainerRegistrys` in all assemblies in the application's base directory (excluding those that start with the `System` namespace)

To run all the registries, use:

```
void IContainer.RunAllRegistries();
```

### Auto Registering Dependencies

You can create `TypeProcessors` that scan all types allowing you to auto-wire them up. 

NanoIoC will find all `TypeProcesors` in all assemblies in the application's base directory (excluding those that start with the `System` namespace)

For example:

```
void IContainer.RunAllTypeProcessors();
```

Where one of your `TypeProcessor`s might look like:
```
public class ExampleTypeProcessor : ITypeProcessor
{
	public void Process(Type type, IContainer container)
	{
		if(typeof(MyInterface).IsAssignableFrom(type) && type != typeof(MyInterface))
			container.Register(typeof(MyInterface), type, ServiceLifetime.Singleton);
	}
}
```


### Resolving Dependencies:

Use either of these methods:

```
T IContainer.Resolve&lt;T&gt;();
object IContainer.Resolve(Type type);
```

You can resolve concrete types that aren't registered, as long as all their dependencies are registered or directly constructable.

You can get all registered types:
```
IEnumerable<T> IContainer.ResolveAll<T>()
IEnumerable IContainer.ResolveAll(Type type);
```

### Self-Resolution

You can resolve the container itself as either `IContainer` or its read-only method subset; `IResolverContainer`, for example:

```
sealed class MyClass
{
	// take a dependency on a read-only version of the container
	public MyClass(IResolverContainer container)
	{
	}
}
```

### Injecting instances:

You can inject existing instances:

```
void IContainer.Inject<T>(T instance, ServiceLifetime lifetime = ServiceLifetime.Singleton);
void IContainer.Inject(object instance, Type type, ServiceLifetime lifetime);
```

### Analysis and Debug

You can ask NanoIoC for the dependency graph for a given type. This will return a graph of all the types dependencies, and their dependencies and so on.
Note that this cannot traverse custom constructor registrations, and so the graph will have a leaf node at that point.

```
GraphNode DependencyGraph(Type type);
```