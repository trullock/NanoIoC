# NanoIoc

A tiny IoC container, does exactly what you want, and only that.

## Getting Started

`Container.Current` is a static instance of `IContainer`. You can use this as your entry point.

### Registering Dependencies

Use either of these methods:

<pre>
void Container.Current.Register<TAbstract, TConcrete>(Lifecycle lifecycle);
void Container.Current.Register(Type abstract, Type concrete, Lifecycle lifecycle);
</pre>

### Resolving Dependencies:

Use either of these methods:

<pre>
T Container.Current.Resolve<T>();
object Container.Current.Resolve(Type type);
</pre>

You can resolve concrete types that aren't registered, as long as all their dependencies are registered.