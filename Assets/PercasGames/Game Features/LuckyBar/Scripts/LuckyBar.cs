using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Percas;

public class LuckyBar : MonoBehaviour, IActivatable
{
    [SerializeField] bool isPopupWin = false;
    [SerializeField] List<RectTransform> segments;
    [SerializeField] List<float> winRates;
    [SerializeField] RectTransform cursor; // Reference to the cursor UI element
    [SerializeField] float moveSpeed = 300f; // Speed of cursor movement
    [SerializeField] TMP_Text _textResult;

    // [!] Make sure segments & cursor have pivot x = 0

    private int _currentIndex = 0; // Index of the current value

    private bool _isMoving = true; // Control automatic movement

    public void Activate()
    {
        // Initialize cursor position to center
        cursor.anchoredPosition = new Vector2(0, cursor.anchoredPosition.y);
    }

    public void Deactivate() { }

    private void UpdateResult()
    {
        if (_textResult != null && !isPopupWin) _textResult.text = $"<sprite=0> {winRates[_currentIndex] * GameLogic.FreePictureLevelCoin}";
        if (_textResult != null && isPopupWin) _textResult.text = $"<sprite=0> {winRates[_currentIndex] * GameLogic.CoinEarnWinLevel}";
    }

    private void Update()
    {
        if (_isMoving) // Infinite loop for continuous movement
        {
            Vector2 newPosition = cursor.anchoredPosition;
            float leftEnd = segments.First().anchoredPosition.x;
            float rightEnd = Mathf.Abs(leftEnd);

            // Determine speed based on distance from center and bounds
            float distanceFromCenter = Mathf.Abs(newPosition.x);
            float maxDistance = Mathf.Abs(leftEnd);

            // Speed increases as it approaches center and decreases as it approaches bounds
            float speedMultiplier = Mathf.Lerp(0.5f, 3f, 1 - (distanceFromCenter / maxDistance));

            newPosition.x += moveSpeed * speedMultiplier * Time.deltaTime;

            if (newPosition.x >= rightEnd) // Change direction when reaching right end
            {
                newPosition.x = rightEnd;
                moveSpeed = -moveSpeed; // Reverse direction
            }
            else if (newPosition.x <= leftEnd) // Change direction when reaching left end
            {
                newPosition.x = leftEnd;
                moveSpeed = -moveSpeed; // Reverse direction
            }

            cursor.anchoredPosition = newPosition;

            // Update current index based on cursor position relative to segments
            _currentIndex = UpdateCurrentIndex(newPosition.x);

            UpdateResult();
        }
    }

    private float GetSegmentLeftEnd(int index)
    {
        RectTransform segment = segments[index];
        //return segment.anchoredPosition.x - _cursorWidth / 2 + 1;
        return segment.anchoredPosition.x + 1;
    }

    private float GetSegmentRightEnd(int index)
    {
        RectTransform segment = segments[index];
        //return segment.anchoredPosition.x + segment.rect.width - _cursorWidth / 2 + 1;
        return segment.anchoredPosition.x + segment.rect.width + 1;
    }

    private int UpdateCurrentIndex(float cursorX)
    {
        if (cursorX < GetSegmentLeftEnd(0)) return 0;
        else if (cursorX >= GetSegmentLeftEnd(0) && cursorX < GetSegmentRightEnd(0)) return 0;
        else if (cursorX >= GetSegmentLeftEnd(1) && cursorX < GetSegmentRightEnd(1)) return 1;
        else if (cursorX >= GetSegmentLeftEnd(2) && cursorX < GetSegmentRightEnd(2)) return 2;
        else if (cursorX >= GetSegmentLeftEnd(3) && cursorX < GetSegmentRightEnd(3)) return 3;
        else if (cursorX >= GetSegmentLeftEnd(4) && cursorX < GetSegmentRightEnd(4)) return 4;
        else return 4;
    }

    public float StopCursor()
    {
        _isMoving = false; // Stop automatic movement
        return winRates[_currentIndex];
    }

    public void RunCursor()
    {
        if (_isMoving) return;
        _isMoving = true; // Start or resume automatic movement
    }
}
