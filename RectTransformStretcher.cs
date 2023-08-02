namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class RectTransformStretcher : MonoBehaviour
	{
		private enum UpdateMode
		{
			Update = 0b1,
			LateUpdate = 0b10,
			FixedUpdate = 0b100
		}
		
		[SerializeField]
		private RectTransform _rectTransform;  // Set this in the inspector
		
		[SerializeField]
		private RectTransform _targetRectTransform; // Set this in the inspector for the new target position
		
		[SerializeField]
		private bool _stretchHorizontal = true;  // Enable or disable horizontal stretching
		
		[SerializeField]
		private bool _stretchVertical = true;  // Enable or disable vertical stretching

		[SerializeField, EnumFlag]
		private UpdateMode _updateMode = UpdateMode.Update;

		private void Update()
		{
			if (_updateMode.HasFlag(UpdateMode.Update)) StretchToTarget(_rectTransform, _targetRectTransform, _stretchHorizontal, _stretchVertical);
		}

		private void LateUpdate()
		{
			if (_updateMode.HasFlag(UpdateMode.LateUpdate)) StretchToTarget(_rectTransform, _targetRectTransform, _stretchHorizontal, _stretchVertical);
		}

		private void FixedUpdate()
		{
			if (_updateMode.HasFlag(UpdateMode.FixedUpdate)) StretchToTarget(_rectTransform, _targetRectTransform, _stretchHorizontal, _stretchVertical);
		}

		public static void StretchToTarget(RectTransform rectTransform, RectTransform targetRectTransform, bool stretchHorizontal, bool stretchVertical)
		{
			// Calculate the world position of the target's pivot
			Vector3 pivotInWorld = targetRectTransform.TransformPoint(targetRectTransform.pivot);

			// Convert the target pivot position to local position
			Vector2 localPoint = rectTransform.InverseTransformPoint(pivotInWorld);

			// Handling Horizontal Stretching
			if (stretchHorizontal)
			{
				if (localPoint.x < 0) // Target is to the left
				{
					// Set the pivot to the right edge
					rectTransform.pivot = new Vector2(1, rectTransform.pivot.y);

					// The current position of the right edge
					float rightPositionX = rectTransform.anchoredPosition.x + rectTransform.rect.width * (1 - rectTransform.pivot.x);

					// Compute the new width, based on the desired position of the left edge
					float newWidth = rightPositionX - localPoint.x;

					// Apply the new width to the RectTransform
					rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
				}
				else // Target is to the right
				{
					// Set the pivot to the left edge
					rectTransform.pivot = new Vector2(0, rectTransform.pivot.y);

					// The current position of the left edge
					float leftPositionX = rectTransform.anchoredPosition.x - rectTransform.rect.width * rectTransform.pivot.x;

					// Compute the new width, based on the desired position of the right edge
					float newWidth = localPoint.x - leftPositionX;

					// Apply the new width to the RectTransform
					rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
				}
			}

			// Handling Vertical Stretching
			if (stretchVertical)
			{
				if (localPoint.y < 0) // Target is below
				{
					// Set the pivot to the upper edge
					rectTransform.pivot = new Vector2(rectTransform.pivot.x, 1);

					// The current position of the upper edge
					float upperPositionY = rectTransform.anchoredPosition.y + rectTransform.rect.height * (1 - rectTransform.pivot.y);

					// Compute the new height, based on the desired position of the lower edge
					float newHeight = upperPositionY - localPoint.y;

					// Apply the new height to the RectTransform
					rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
				}
				else // Target is above
				{
					// Set the pivot to the lower edge
					rectTransform.pivot = new Vector2(rectTransform.pivot.x, 0);

					// The current position of the lower edge
					float lowerPositionY = rectTransform.anchoredPosition.y - rectTransform.rect.height * rectTransform.pivot.y;

					// Compute the new height, based on the desired position of the upper edge
					float newHeight = localPoint.y - lowerPositionY;

					// Apply the new height to the RectTransform
					rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
				}
			}
		}
	}
}