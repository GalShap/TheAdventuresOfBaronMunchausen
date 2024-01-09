using Core;
using UnityEngine;

namespace BitStrap.Examples
{
	public sealed class DummySingleton : SingletonPersistent<DummySingleton>
	{
		public int dummyIntField = 8;
	}
}
