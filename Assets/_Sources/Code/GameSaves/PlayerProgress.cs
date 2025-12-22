using System;
using Newtonsoft.Json;

namespace Sources.Code.Gameplay.GameSaves
{
    [Serializable]
    public class PlayerProgress
    {
        [JsonProperty] public int LevelNumber = 1;
    }
}