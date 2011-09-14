# NanoIoC

A tiny IoC container, does exactly what you want, and only that.

## Getting Started

`Container.Global` is a static instance of `IContainer`. You can use this as your entry point.

### Registering Dependencies

Use either of these methods:

<pre>
void IContainer.Register&lt;TAbstract, TConcrete&gt;(Lifecycle lifecycle);
void IContainer.Register(Type abstract, Type concrete, Lifecycle lifecycle);
</pre>

### Resolving Dependencies:

Use either of these methods:

<pre>
T IContainer.Resolve&lt;T&gt;();
object IContainer.Resolve(Type type);
</pre>

You can resolve concrete types that aren't registered, as long as all their dependencies are registered.