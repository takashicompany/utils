namespace Kayac
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using DG.Tweening;
	using MoreMountains.NiceVibrations;

	/// <summary>
	/// バイブレーションのラッパークラス
	/// </summary>
	public static class Vibration
	{
		public enum Type
		{
			Selection,
			Success,
			Warning,
			Failure,
			LightImpact,
			MediumImpact,
			HeavyImpact
		}

		public static void Selection()
		{
			MMVibrationManager.Haptic(HapticTypes.Selection);
		}

		public static void Success()
		{
			MMVibrationManager.Haptic(HapticTypes.Success);
		}

		public static void Warning()
		{
			MMVibrationManager.Haptic(HapticTypes.Warning);
		}

		public static void Failure()
		{
			MMVibrationManager.Haptic(HapticTypes.Failure);
		}

		public static void LightImpact()
		{
			MMVibrationManager.Haptic(HapticTypes.LightImpact);
		}

		public static void MediumImpact()
		{
			MMVibrationManager.Haptic(HapticTypes.MediumImpact);
		}

		public static void HeavyImpact()
		{
			MMVibrationManager.Haptic(HapticTypes.HeavyImpact);
		}

		public static void Vibrate(this Type vibType)
		{
			switch (vibType)
			{
				case Type.Selection:
					Selection();
					break;

				case Type.Success:
					Success();
					break;

				case Type.Warning:
					Warning();
					break;

				case Type.Failure:
					Failure();
					break;
					
				case Type.LightImpact:
					LightImpact();
					break;

				case Type.MediumImpact:
					MediumImpact();
					break;

				case Type.HeavyImpact:
					HeavyImpact();
					break;
			}
		}

		public static void Vibrate(this Type vib, int count, float interval)
		{
			var vibs = new Type[count];
			Vibrate(vibs, interval);
		}

		public static void Vibrate(this Type[] vibs, float interval)
		{
			var seq = DOTween.Sequence();

			foreach (var v in vibs)
			{
				seq.AppendCallback(() =>
				{
					v.Vibrate();
				});

				seq.AppendInterval(interval);
			}
		}
	}
}
