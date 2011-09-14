# NanoIoc

A tiny IoC container, does exactly what you want, and only that.

## Getting Started

`Container.Current` is a static instance of `IContainer`. You can use this as your entry point.

### Registering Dependencies

Use either of these methods:

<pre>
Container.Current.Register<TAbstract, TConcrete>(Lifecycle lifecycle);
Container.Current.Register(Type abstract, Type concrete, Lifecycle lifecycle);
</pre>