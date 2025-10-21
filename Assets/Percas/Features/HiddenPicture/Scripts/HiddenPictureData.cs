using System;
using System.Collections.Generic;

namespace Percas
{
    public enum HiddenPictureStatus
    {
        None,
        InProcess,
        Completed,
    }

    public class HiddenPictureData
    {
        // Global
        public List<int> CollectedEvents = new();
        public List<int> CollectedEventQuotes = new();
        public List<string> CollectedPicturePieces = new();

        // Event
        public HiddenPictureStatus Status;
        public int EventID = -1;
        public int EventQuoteID = -1;
        public int Keys = 0;
        public int FreeKeys = 1;
        public int FreeAdKeys = 1;
        public string EndTime;
        public List<int> UnlockedPieces = new();
        public List<int> ReceivedRewards = new();
        public bool PictureCollected = false;

        public void Reset()
        {
            Status = HiddenPictureStatus.None;
            EventID = -1;
            EventQuoteID = -1;
            Keys = 0;
            FreeKeys = 1;
            FreeAdKeys = 1;
            EndTime = null;
            UnlockedPieces = new();
            ReceivedRewards = new();
            PictureCollected = false;
        }

        public void ResetFreeKeys()
        {
            FreeKeys = 1;
            FreeAdKeys = 1;
            HiddenPictureManager.OnSave?.Invoke();
        }

        public void UpdateKeys(int value)
        {
            Keys += value;
            HiddenPictureManager.OnSave?.Invoke();
        }

        public void UpdateFreeKeys(int value)
        {
            FreeKeys += value;
            HiddenPictureManager.OnSave?.Invoke();
        }

        public void UpdateFreeAdKeys(int value)
        {
            FreeAdKeys += value;
            HiddenPictureManager.OnSave?.Invoke();
        }

        public void Start()
        {
            Status = HiddenPictureStatus.InProcess;
            EventID = DataManager.Instance.GetRandomHiddenPictureID();
            EventQuoteID = DataManager.Instance.GetRandomQuoteIndex();
            Keys = 2;
            FreeKeys = 1;
            FreeAdKeys = 1;
            EndTime = TimeHelper.ToIsoString(DateTime.UtcNow.AddDays(6).EndOfDay());
            UnlockedPieces = new();
            ReceivedRewards = new();
            PictureCollected = false;
        }

        public bool HasAvailableEvent()
        {
            return DataManager.Instance.GetRandomHiddenPictureID() >= 0;
        }

        public bool HasActiveEvent()
        {
            return Status == HiddenPictureStatus.InProcess && !IsCompleted();
        }

        public bool IsCompleted()
        {
            try
            {
                TimeSpan remainTime = DateTime.UtcNow - TimeHelper.ParseIsoString(EndTime);
                return remainTime.TotalSeconds > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool IsUnlocked(int index)
        {
            return UnlockedPieces.Contains(index);
        }

        public void AddUnlockedPieces(int index)
        {
            if (IsUnlocked(index)) return;
            UnlockedPieces.Add(index);
            HiddenPictureManager.OnSave?.Invoke();
        }

        public bool IsReceived(int reward)
        {
            return ReceivedRewards.Contains(reward);
        }

        public void AddReceivedRewards(int reward)
        {
            if (IsReceived(reward)) return;
            ReceivedRewards.Add(reward);
            HiddenPictureManager.OnSave?.Invoke();
        }

        public bool IsCollectedEvent(int eventID)
        {
            return CollectedEvents.Contains(eventID);
        }

        public void AddCollectedEvents(int eventID)
        {
            PictureCollected = true;
            if (IsCollectedEvent(eventID)) return;
            CollectedEvents.Add(eventID);
            CollectedEventQuotes.Add(EventQuoteID);
            PictureCollected = true;
            HiddenPictureManager.OnSave?.Invoke();
        }

        public bool IsCollectedPiece(string piece)
        {
            return CollectedPicturePieces.Contains(piece);
        }

        public void AddCollectedPiece(string piece)
        {
            if (IsCollectedPiece(piece)) return;
            CollectedPicturePieces.Add(piece);
            HiddenPictureManager.OnSave?.Invoke();
        }
    }
}
