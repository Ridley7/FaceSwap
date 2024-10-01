//Core reusable para cualquier proyecto hecho con Unity
//Estado del proyecto:
/*
 * - Timers controllers : hecho
 * - Messages controllers : hecho
 * - Language controller: hecho
 * - Audio controller: play sounds, play music : en pruebas
 */

//Cosas que faltan, y sobretodo entenderlas:
/*
 - Actions controller: Load audio, load translations, Load level. -> I
 - Scene controller: Load unity scene
 - Screen controller: UGUI y NGUI -> I
 - Gamepad Controller: PS4, PC, Xbox One, Swithc, WiiU
 - Save/Load:
 */

using UnityEngine;
using r_core.util;
using r_core.coresystems.time;
using System;
using r_core.language;
using r_core.save_load;
using System.Collections.Generic;

namespace r_core.core
{
    public class R_Core : R_Singleton<R_Core>
    {
        //Editor links
        // -------------------------------------------------------
        [SerializeField] private int totalTimersPooled = 100;
        [SerializeField] private bool setGame30FPS = false;
        [SerializeField] private bool slowPhysicsFor60FPS = false;
        [SerializeField] private float delayTimeToStart = 1f; //NEW --> Para nosotros de momento va a ser 0

        //Servicios del core
        //---------------------------------------------------------
        private R_TimerController timer = null;

        //Servicio de traduccion
        private LanguageController language;

        //Servicio de audio
        //serialized fields are initiated by unity (called new instance)
        [SerializeField] private R_SoundsGamePool soundsGamePool = null;

        //Servicio de guardar y cargar datos
        private SaveLoadController saveLoad;

        private List<IDestroyable> destroyableSystems = new List<IDestroyable>(10);
        private List<IUpdateTimedSystems> updateTimedSystems = new List<IUpdateTimedSystems>(3);

        private bool gamePaused = false;
        private bool unityPlayerInPause = false;
        private int totalUpdateSystems = 0;

        #region pause game

        private System.Action<bool> onGamePaused = null; //Cualquier pueda suscribirse aqui y ser notificado cuando el juego es pausado

        public void SubscribeToPause(System.Action<bool> onFunction)
        {
            onGamePaused += onFunction;
        }

        public void UnSubscribeToPause(System.Action<bool> onFunction)
        {
            onGamePaused -= onFunction;
        }

        public void SetCorePaused()
        {
            gamePaused = true;
            timer.PauseAllTimers(gamePaused);
            onGamePaused?.Invoke(gamePaused);
        }

        public void SetCoreUnPaused()
        {
            gamePaused = false;
            timer.PauseAllTimers(gamePaused);
            onGamePaused?.Invoke(gamePaused);
        }
        #endregion

        #region monobehaviours and init
        protected override void Initialize(bool dontdestroy = false)
        {
            //Indicamos que no se destruya el core
            base.Initialize(true);

            Debug.Log("HOla desde el core");

            gamePaused = false;

            //Indicamos el frame rate al que se va a ejecutar nuestro juego
            Application.targetFrameRate = R_CoreConstants.SIXTY_FRAME_RATE;

            //Cargamos los demos elementos del core
            LoadCoreSystem();
        }

        private void Start()
        {
            if (setGame30FPS)
            {
                Application.targetFrameRate = R_CoreConstants.SIXTY_FRAME_RATE;
                Time.fixedDeltaTime = 0.033333f; //physics at 30 fps
            }
            else
            {
                Application.targetFrameRate = R_CoreConstants.THIRTY_FRAME_RATE;
                Time.fixedDeltaTime = slowPhysicsFor60FPS ? 0.02f : 0.0167f;
            }

            // Load libraries
            InitExternalPlugins();

            //NEW to load the gameflow
            if(delayTimeToStart > 0f)
            {
                StartTimer(delayTimeToStart, true, this, cacheAction =>
                {
                    (cacheAction.Context as R_Core).InitialLoadOnStart();
                });
            }
            else
            {
                InitialLoadOnStart();
            }
        }

        public override void OnDestroy()
        {
            updateTimedSystems.Clear();

            for(var i = destroyableSystems.Count - 1; i > -1; --i)
            {
                destroyableSystems[i]?.IDestroyUnity();
            }

            destroyableSystems.Clear();

            base.OnDestroy();

            //Limpiamos la memoria de todas las referencias a traducciones
            language.ClearAllStrings();
            language = null;

            //Limpiamos el servicio de sonido
            soundsGamePool = null;

            //Limpiamos el servicio de guardado y carga de datos
            saveLoad = null;
        }

        private void Update()
        {
            //if we go outside the app focus (minimize of somenthing like that)
            if (unityPlayerInPause) return;

            //Actualizamos servicios
            for (var i = 0; i < totalUpdateSystems; ++i)
            {
                updateTimedSystems[i].IOnUpdate(1f);
            }
        }

        //unity went to background or came from it... (not a game pause)
        private void OnApplicationPause(bool pause)
        {
            unityPlayerInPause = pause;
        }

        #endregion


        #region load internals
        private void InitialLoadOnStart()
        {
            //Aqui se inicia el flow de toda la aplicacion en caso de tenerlo
            //De momento no lo vamos a usar
        }

        private void LoadCoreSystem()
        {
            //Creamos el controlador de timers
            timer = new R_TimerController();
            System.GC.KeepAlive(timer);
            timer.InitTimerManager(totalTimersPooled);

            //Creamos el sistema de idiomas
            language = new LanguageController();

            //Cargamos los ficheros de idiomas de la carpeta resources
            //ATENCION POR QUE ESTO LO TIENE QUE HACER OTRO ELEMENO QUIZAS EL SISTEMA ACCIONES
            //ESTA BASURILLA TIENE QUE ESTAR EN OTRO LADO, QUE HA DE SER OTRA PARTE DEL CORE
            for(int i = 0; i < Resources.LoadAll<TextAsset>("Translations/TranslationFilesJson/").Length; i++)
            {

                TextAsset translationIndexFile = Resources.LoadAll<TextAsset>("Translations/TranslationFilesJson/")[i];

                language.LoadLanguageFileJson(translationIndexFile, R_CoreConstants.GLOBAL_LANGUAGE_SELECTED, (result) =>
                {
                    if (!result)
                    {
                        Debug.LogError("error loading translations file");
                    }
                });
            }

            //Cargamos el audio controller
            soundsGamePool = FindObjectOfType<R_SoundsGamePool>();
            soundsGamePool.InitLoadAllSounds();

            //Cargamos el servicio de guardado y carga de datos
            saveLoad = new SaveLoadController();

            destroyableSystems.Add(timer);

            AddUpdatableSystem(timer);
        }

        private void AddUpdatableSystem(IUpdateTimedSystems aSystem)
        {
            updateTimedSystems.Add(aSystem);
            totalUpdateSystems++;
        }

        private void InitExternalPlugins()
        {

        }

        #endregion

        #region API Timers
        public bool CheckIfTimerManagerWasInitiated()
        {
            return timer.CheckIfTimerManagerWasInitiated();
        }

        public void PauseAllTimers(bool aGameIsPaused)
        {
            timer.PauseAllTimers(aGameIsPaused);
        }

        public void StopAllTimers()
        {
            timer.StopAllTimers();
        }

        public void StopTimerWithID(uint aTimerid)
        {
            timer.StopTimerWithID(aTimerid);
        }

        public R_Timer GetUnusedTimer()
        {
            return timer.GetUnusedTimer();
        }

        public void StartTimer(float aTimerValue, object aContext, Action<R_Timer> aNewcallback)
        {
            timer.StartTimer(aTimerValue, aContext, aNewcallback);
        }
        public void StartTimer(float aTimerValue, object aContext, Action<R_Timer> aOnUpdateFrame, Action<R_Timer> aNewcallback)
        {
            timer.StartTimer(aTimerValue, aContext, aOnUpdateFrame, aNewcallback);
        }

        public void StartTimer(float aTimerValue, bool aCannotbePaused, object aContext, Action<R_Timer> aNewcallback)
        {
            timer.StartTimer(aTimerValue, aCannotbePaused, aContext, aNewcallback);
        }

        public void StartTimer(float aTimerValue, bool aCannotbePaused, object aContext, Action<R_Timer> aOnUpdateFrame, Action<R_Timer> aNewcallback)
        {
            timer.StartTimer(aTimerValue, aCannotbePaused, aContext, aOnUpdateFrame, aNewcallback);
        }

        public uint StartTimerId(float aTimerValue, object aContext, Action<R_Timer> aNewcallback)
        {
            return timer.StartTimerId(aTimerValue, aContext, aNewcallback);
        }

        public uint StartTimerId(float aTimerValue, object aContext, Action<R_Timer> aOnUpdateFrame, Action<R_Timer> aNewcallback)
        {
            return timer.StartTimerId(aTimerValue, aContext, aOnUpdateFrame, aNewcallback);
        }

        public uint StartTimerId(float aTimerValue, bool aCannotbePaused, object aContext, Action<R_Timer> aNewcallback)
        {
            return timer.StartTimerId(aTimerValue, aCannotbePaused, aContext, aNewcallback);
        }

        public uint StartTimerId(float aTimerValue, bool aCannotbePaused, object aContext, Action<R_Timer> aOnUpdateFrame, Action<R_Timer> aNewcallback)
        {
            return timer.StartTimerId(aTimerValue, aCannotbePaused, aContext, aOnUpdateFrame, aNewcallback);
        }

        #endregion

        #region API Languages
        public string GetString(String key_json)
        {
            return language.GetString(key_json);
        }

        public void SetNewLanguage(LanguageType languageType)
        {
            language.SetNewLanguage(languageType);
        }

        public LanguageType GetLanguage()
        {
            return language.GetLanguage();
        }
        #endregion

        #region API audio
        public void PlaySound(string name, float volume)
        {
            soundsGamePool.PlaySound(name, volume);
        }

        #endregion

        #region API SaveLoad

        public void Save(string information, string namefile)
        {
            saveLoad.Save(information, namefile);
        }

        public R_DummyData LoadDummyData(string filename)
        {
            return saveLoad.Load<R_DummyData>(filename);
        }

        public R_DummyInfo LoadDummyInfo(string filename)
        {
            return saveLoad.Load<R_DummyInfo>(filename);
        }

        #endregion
    }

}
