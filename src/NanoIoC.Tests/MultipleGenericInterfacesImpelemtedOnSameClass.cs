using System;
using NUnit.Framework;

namespace NanoIoC.Tests
{
	[TestFixture]
	public class MultipleGenericInterfacesImpelemtedOnSameClass
	{
		public interface IMsg { }

		public interface IMsgHandler<T>
			where T : IMsg
		{
		}

		public class FooMsg : IMsg { }
		public class BahMsg : IMsg  { }

		public class MsgHandler :
			IMsgHandler<FooMsg>,
			IMsgHandler<BahMsg>
		{
			public MsgHandler()
			{
			}
		}

		public class MsgHandlerTypeProcessor : OpenGenericTypeProcessor
		{
			protected override Type OpenGenericTypeToClose
			{
				get { return typeof(IMsgHandler<>); }
			}

			public override Lifecycle Lifecycle
			{
				get { return Lifecycle.Singleton; }
			}
		}

		[Test]
		public void Can_locate_mutltiple_generic_interfaces_on_same_class()
		{
			var container = new Container();
			container.RunAllTypeProcessors();

			var fooHandler = container.Resolve<IMsgHandler<FooMsg>>();
			Assert.That(fooHandler, Is.Not.Null, "foo handler");

			var bahHandler = container.Resolve<IMsgHandler<BahMsg>>();
			Assert.That(bahHandler, Is.Not.Null, "bah handler");
		}

		[Test]
		[Ignore("Deliberately added a test I know will fail, to resolve at some time in the future as it would be a more significant restructure.")]
		public void Singleton_instance_is_created_per_concrete_class_not_per_interface_implementation()
		{
			var container = new Container();
			container.RunAllTypeProcessors();

			// Not a test, but an assertion of state we rely on.
			Assert.That(new MsgHandlerTypeProcessor().Lifecycle, Is.EqualTo(Lifecycle.Singleton));

			var fooHandler = container.Resolve<IMsgHandler<FooMsg>>();
			var bahHandler = container.Resolve<IMsgHandler<BahMsg>>();

			Assert.That(fooHandler, Is.SameAs(bahHandler));
		}
	}
}