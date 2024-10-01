using System;
using UnityEngine;

namespace r_core.coresystems.time
{
    [System.Serializable]
    public class R_Timer
    {
        private float waitingTime = float.MaxValue; //Tiempo total que tiene que completar el timer
        private double timer = 0.0; //Contador de tiempo del timer
        private uint idTimer = 0; //Identificador de este timer, util si hay que pararlo.
        private bool timerInUse = false;
        private Action<R_Timer> cachedDelegateAction = null;
        private Action<R_Timer> onUpdateFramecachedDelegateAction = null;
        private double timerWhenEnterPause = 0.0;

        public bool CannotBePaused { private set; get; }
        public bool IsActive { get; private set; }
        public object Context { get; private set; }

        public R_Timer()
        {
            timerWhenEnterPause = 0.0;
            CannotBePaused = false;
            waitingTime = float.MaxValue;
            timer = 0.0;
            idTimer = 0;
            IsActive = false;
            timerInUse = false;
        }
        
        public void SetContext(object aContext)
        {
            Context = aContext;
        }

        public void SetTimerActionCompletedWithSelfCallback(Action<R_Timer> aNewcallback)
        {
            cachedDelegateAction = aNewcallback;
        }

        public void SetTimerOnUpdateWithSelfCallback(Action<R_Timer> aNewcallback)
        {
            onUpdateFramecachedDelegateAction = aNewcallback;
        }

        public bool GetIsActive()
        {
            return IsActive && timerInUse;
        }

        public double GetTimerValue()
        {
            return timer;
        }

        public uint GetTimerID()
        {
            return idTimer;
        }
        public float GetWaitingTimer()
        {
            return waitingTime;
        }

        public void Destroy()
        {
            timerWhenEnterPause = 0.0;
            CannotBePaused = false;
            waitingTime = float.MaxValue;
            timer = 0.0;
            idTimer = 0;
            IsActive = false;
            timerInUse = false;
            SetContext(null);
            SetTimerActionCompletedWithSelfCallback(null);
            SetTimerOnUpdateWithSelfCallback(null);
        }

        #region timer functionality
        public void PauseTimer(bool aGameIsPaused)
        {
            if (CannotBePaused) return;

            if(IsActive && aGameIsPaused)
            {
                timerWhenEnterPause = Time.time;
                timerInUse = true;
                IsActive = false;
            }
            else if(!aGameIsPaused && timerInUse)
            {
                var totalTimeInPause = timerWhenEnterPause != 0.0 ? timerWhenEnterPause : timer;
                var elapsed = Time.time - totalTimeInPause;

                //Incrementamos la cantidad de tiempo del timer que ha sido pausado
                waitingTime += (float)elapsed;
                timerWhenEnterPause = 0.0;
                IsActive = true;
            }
        }

        public void StartTimer(float aWaitValue, bool aCannotbePaused, uint aTimerid)
        {
            timerWhenEnterPause = 0.0;
            waitingTime = aWaitValue;
            timer = Time.time;
            CannotBePaused = aCannotbePaused;
            IsActive = true;
            idTimer = aTimerid;
            timerInUse = true;
        }

        public void StopTimer()
        {
            Context = null;
            cachedDelegateAction = null;
            timerWhenEnterPause = 0.0;
            CannotBePaused = false;
            waitingTime = float.MaxValue;
            timer = 0.0;
            idTimer = 0;
            IsActive = false;
            timerInUse = false;
        }

        public void UpdateTimer()
        {
            if (!IsActive) return;

            //calculamos la diferencie desde el inicio hasta el tiempo actual
            var elapsed = Time.time - timer;

            onUpdateFramecachedDelegateAction?.Invoke(this);

            //condition met, time to fire up the delegate on completion
            if (!(elapsed >= waitingTime)) return;

            cachedDelegateAction?.Invoke(this);

            StopTimer();
        }
        #endregion
    }
}
