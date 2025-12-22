using System.Collections;
using Newtonsoft.Json;
using Sources.Code.Core.Singletones;
using UnityEngine;
using UnityEngine.Serialization;

namespace Sources.Code.Gameplay.GameSaves
{
    public class GameSaverLoader : SingletonBehaviour<GameSaverLoader>
    {
        private const string SETTINGS_PROGRESS_KEY = "SettingsProgress";

        [FormerlySerializedAs("_defaultSettingsProgress")] [SerializeField] private PlayerProgress defaultPlayerProgress;

        private Coroutine _autoSaveCoroutine;

        public PlayerProgress PlayerProgress { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            var defaultSettingsProgressInJson = JsonConvert.SerializeObject(defaultPlayerProgress);
            string settingsProgressInJson = PlayerPrefs.GetString(SETTINGS_PROGRESS_KEY, defaultSettingsProgressInJson);
            PlayerProgress = JsonConvert.DeserializeObject<PlayerProgress>(settingsProgressInJson);

            _autoSaveCoroutine = StartCoroutine(AutoSaveCoroutine());
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                MadeAutoSave();
            }
        }

        private void OnApplicationQuit()
        {
            MadeAutoSave();
        }

        private IEnumerator AutoSaveCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(1);
                MadeAutoSave();
            }
        }

        private void MadeAutoSave()
        {
            var settingsProgressInJson = JsonConvert.SerializeObject(PlayerProgress);
            PlayerPrefs.SetString(SETTINGS_PROGRESS_KEY, settingsProgressInJson);
        }
    }
}