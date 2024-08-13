using Fiber.Utilities;
using GamePlay.Blobs;

namespace GamePlay.Obstacles
{
	public class BreakableObstacle : BaseObstacle
	{
		public override void OnBlastNear(Blob blob)
		{
			base.OnBlastNear(blob);

			CurrentGridCell.CurrentNode = null;

			Destroy(gameObject);
			ParticlePooler.Instance.Spawn("BreakableObstacle", transform.position);
		}
	}
}