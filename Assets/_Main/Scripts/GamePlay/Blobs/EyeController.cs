using System.Collections;
using Fiber.Managers;
using GamePlay.Player;
using TriInspector;
using UnityEngine;
using UnityEngine.Animations;

namespace GamePlay.Blobs
{
	public class EyeController : MonoBehaviour
	{
		[SerializeField] private Animator eyesAnimator;
		[Space]
		[SerializeField] private LookAtConstraint[] lookAtConstraints;

		[Title("Options")]
		[SerializeField] private float minTransitionDuration = .5f;
		[SerializeField] private float maxTransitionDuration = 2f;

		[SerializeField] private float minWaitDuration = 2f;
		[SerializeField] private float maxWaitDuration = 5f;

		private static readonly int eyeX = Animator.StringToHash("EyeX");
		private static readonly int eyeY = Animator.StringToHash("EyeY");

		private void Awake()
		{
			foreach (var lookAtConstraint in lookAtConstraints)
			{
				var source = new ConstraintSource { sourceTransform = Player.Player.Instance.InputEyeTarget, weight = 0 };
				lookAtConstraint.AddSource(source);
			}
		}

		private void Start()
		{
			StartCoroutine(PlayRandomEyesAnimation());
		}

		private void OnEnable()
		{
			PlayerInputs.OnDown += OnFingerDown;
			PlayerInputs.OnUp += OnFingerUp;

			LevelManager.OnLevelWin += OnLevelWon;
			LevelManager.OnLevelLose += OnLevelLost;
		}

		private void OnDisable()
		{
			PlayerInputs.OnDown -= OnFingerDown;
			PlayerInputs.OnUp -= OnFingerUp;

			LevelManager.OnLevelWin -= OnLevelWon;
			LevelManager.OnLevelLose -= OnLevelLost;
		}

		private void OnFingerDown(Vector3 pos)
		{
			for (var i = 0; i < lookAtConstraints.Length; i++)
			{
				var eyeTarget_CS = lookAtConstraints[i].GetSource(0);
				eyeTarget_CS.weight = 0;
				lookAtConstraints[i].SetSource(0, eyeTarget_CS);

				var inputEyeTarget_CS = lookAtConstraints[i].GetSource(1);
				inputEyeTarget_CS.weight = 1;
				lookAtConstraints[i].SetSource(1, inputEyeTarget_CS);
			}
		}

		private void OnFingerUp(Vector3 pos)
		{
			for (var i = 0; i < lookAtConstraints.Length; i++)
			{
				var eyeTarget_CS = lookAtConstraints[i].GetSource(0);
				eyeTarget_CS.weight = 1;
				lookAtConstraints[i].SetSource(0, eyeTarget_CS);

				var inputEyeTarget_CS = lookAtConstraints[i].GetSource(1);
				inputEyeTarget_CS.weight = 0;
				lookAtConstraints[i].SetSource(1, inputEyeTarget_CS);
			}
		}

		private IEnumerator PlayRandomEyesAnimation()
		{
			while (isActiveAndEnabled)
			{
				var randomWaitDuration = Random.Range(minWaitDuration, maxWaitDuration);

				yield return StartCoroutine(MoveEyes());
				yield return new WaitForSeconds(randomWaitDuration);
			}
		}

		private IEnumerator MoveEyes()
		{
			var x = Random.Range(-1f, 1f);
			var y = Random.Range(-1f, 1f);

			var transitionDuration = Random.Range(minTransitionDuration, maxTransitionDuration);
			var timer = 0f;
			while (timer < transitionDuration)
			{
				var currentX = eyesAnimator.GetFloat(eyeX);
				var currentY = eyesAnimator.GetFloat(eyeY);
				eyesAnimator.SetFloat(eyeX, Mathf.Lerp(currentX, x, timer / transitionDuration));
				eyesAnimator.SetFloat(eyeY, Mathf.Lerp(currentY, y, timer / transitionDuration));

				timer += Time.deltaTime;
				yield return null;
			}

			eyesAnimator.SetFloat(eyeX, x);
			eyesAnimator.SetFloat(eyeY, y);
		}

		private void OnLevelWon()
		{
			OnFingerUp(default);
		}

		private void OnLevelLost()
		{
			OnFingerUp(default);
		}
	}
}