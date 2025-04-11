using System;

namespace Shared
{
    [Serializable]
    public class DifficultySettings<T>
    {
        public T easy;
        public T normal;
        public T hard;
        public T nightmare;


        public DifficultySettings(T easy, T normal, T hard, T nightmare)
        {
            this.easy = easy;
            this.normal = normal;
            this.hard = hard;
            this.nightmare = nightmare;
        }

        public T GetValue(GameDifficulty difficulty)
        {
            return difficulty switch
            {
                GameDifficulty.Easy => easy,
                GameDifficulty.Normal => normal,
                GameDifficulty.Hard => hard,
                GameDifficulty.Nightmare => nightmare,
                _ => throw new ArgumentOutOfRangeException(nameof(difficulty), difficulty, null)
            };
        }
    }
}