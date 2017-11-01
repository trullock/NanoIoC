using System;
using Xunit;

namespace NanoIoC.Tests
{
	public class MultipleGenericInterfacesImpelemtedOnSameClass
	{
		public interface IMsg
		{
		}

		public interface IMsgHandler<T> where T : IMsg
		{
		}

		public class FooMsg : IMsg
		{
		}

		public class BahMsg : IMsg
		{
		}

		public class MsgHandler : IMsgHandler<FooMsg>, IMsgHandler<BahMsg>
		{
		}

		public class MsgHandlerTypeProcessor : OpenGenericTypeProcessor
		{
			protected override Type OpenGenericTypeToClose => typeof (IMsgHandler<>);

			public override Lifecycle Lifecycle => Lifecycle.Singleton;
		}

		[Fact]
		public void Can_locate_mutltiple_generic_interfaces_on_same_class()
		{
			var container = new Container();
			container.RunAllTypeProcessors();

			var fooHandler = container.Resolve<IMsgHandler<FooMsg>>();
			Assert.NotNull(fooHandler);

			var bahHandler = container.Resolve<IMsgHandler<BahMsg>>();
			Assert.NotNull(bahHandler);
		}

		[Fact(Skip = "Deliberately added a test I know will fail, to resolve at some time in the future as it would be a more significant restructure.")]
		public void Singleton_instance_is_created_per_concrete_class_not_per_interface_implementation()
		{
			var container = new Container();
			container.RunAllTypeProcessors();

			// Not a test, but an assertion of state we rely on.
			Assert.Equal(new MsgHandlerTypeProcessor().Lifecycle, Lifecycle.Singleton);

			var fooHandler = container.Resolve<IMsgHandler<FooMsg>>();
			var bahHandler = container.Resolve<IMsgHandler<BahMsg>>();

			Assert.Same(fooHandler, bahHandler);
		}
	}
}