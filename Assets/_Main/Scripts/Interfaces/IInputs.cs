using UnityEngine;
using UnityEngine.Events;

namespace Interfaces
{
	public interface IInputs
	{
		public bool CanInput { get; set; }
		public static event UnityAction<Vector3> OnDown;
		public static event UnityAction<Vector3> OnMove;
		public static event UnityAction<Vector3> OnUp;
	}
}