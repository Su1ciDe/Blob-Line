using Fiber.Utilities;
using Managers;
using UnityEngine;

namespace Fiber.Managers
{
	[DefaultExecutionOrder(-1)]
	public class GameManager : SingletonInit<GameManager>
	{
		[SerializeField] private BlobMaterialsSO blobMaterialsSO;
		public BlobMaterialsSO BlobMaterialsSO => blobMaterialsSO;

		protected override void Awake()
		{
			base.Awake();
			Application.targetFrameRate = 60;
			Debug.unityLogger.logEnabled = Debug.isDebugBuild;
		}
	}
}