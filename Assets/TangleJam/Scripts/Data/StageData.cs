using System;

namespace Tuon
{
    [Serializable]
    public sealed class StageData
    {
        [Serializable]
        public sealed class CellData
        {
            public CellType Type;

            public int Value;

            public Direction Direction;

            public string String;

            public int MoveIndex;

            public bool isCoil
            {
                get
                {
                    return Type == CellType.Coil || Type == CellType.CoilLocked || Type == CellType.CoilPair;
                }
            }
        }

        public enum CellType
        {
            Empty = 0,
            Coil = 1,
            Wall = 2,
            Stack = 3,
            CoilLocked = 4,
            PinWall = 5,
            PinControl = 6,
            Key = 7,
            Lock = 8,
            CoilPair = 9,
            ButtonStack = 10,
        }

        public enum Direction
        {
            Up = 0,
            Right = 1,
            Down = 2,
            Left = 3
        }

        public int Width;

        public int Height;

        public string RandomCoils;

        public string LockedCoils;

        public string Tutorial;

        public CellData[] Data;

        public LevelAsset.TutorialType tutorialType;

        public LevelAsset.Difficulty difficulty;

        public CellData GetData(int x, int y)
        {
            return null;
        }
    }
}