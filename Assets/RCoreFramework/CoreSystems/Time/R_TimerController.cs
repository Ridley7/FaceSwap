//usage examples
//
//            //0ks gc alloc
//            TimerManager.StartTimer(2f, this, anAction =>
//            {
//                (anAction.context as YourClass).YourMethodToCallName();
//            });
//
//
// stop timer
//TimerManager.StopTimer (cacheAction =>
//	{
//		(cacheAction.context as YourClass).StopDoingStuffFunction();
//	});

using System;
using System.Diagnostics;

namespace r_core.coresystems.time
{
	public class R_TimerController: IDestroyable, IUpdateTimedSystems
	{
		//Lista de timers disponibles
		private R_Timer[] timerList;

		//Esto sera usado como contador para dar un nuevo id a cada timer
		private uint timerID = 0;
        private int totalList = 0;
        int currentTimerPositionId = 0;

        #region Init and destroy timers
        public void InitTimerManager(int amountOfTimers)
		{
			timerList = new R_Timer[amountOfTimers];

			for(int cnt = 0; cnt < amountOfTimers; cnt++)
			{
				R_Timer timer = new R_Timer();
				timerList[cnt] = timer;
			}

			totalList = amountOfTimers;
		}

		public void DestroyAndCleanUp()
		{
			for (int cnt = 0; cnt < totalList; ++cnt)
			{
				if (timerList[cnt] != null)
				{
					timerList[cnt].Destroy();
				}
			}

			System.Array.Clear(timerList, 0, timerList.Length);

			timerList = null;
		}

        void IDestroyable.IDestroy()
        {
			DestroyAndCleanUp();
        }

        void IDestroyable.IDestroyUnity()
        {
			DestroyAndCleanUp();
        }
        #endregion

        #region public functions
		public bool CheckIfTimerManagerWasInitiated()
		{
			return timerList != null;
		}

		public void PauseAllTimers(bool aGameIsPaused)
		{
			for(var cnt = 0; cnt < totalList; ++cnt)
			{
				var timer = timerList[cnt];

				timer.PauseTimer(aGameIsPaused);
			}
		}

		public void StopAllTimers()
		{
			for(var cnt = 0; cnt < totalList; ++cnt)
			{
				var timer = timerList[cnt];

				timer.StopTimer();
			}
		}
        #endregion

        #region control para timers individuales
        /// <summary>
        /// Pause or Unpause a timer with a given id
        /// </summary>
        /// <param name="aTimerid"></param>
        /// <param name="aIspaused"></param>
        public void PauseTimerWithID(uint aTimerid, bool aIspaused)
        {
            if (aTimerid == 0) return;

            for (var cnt = 0; cnt < totalList; ++cnt)
            {
                var timer = timerList[cnt];

                //timer match?
                //we don't care if it's active, just care about an Id match
                if (timer.GetTimerID() == aTimerid)
                {
                    timer.PauseTimer(aIspaused);
                    break;
                }
            }
        }

        /// <summary>
        /// stop the timer with the given id if it exists
        /// and the timer id is not 0, which means no timer at all
        /// </summary>
        /// <param name="aTimerid"></param>
        public void StopTimerWithID(uint aTimerid)
        {
            if (aTimerid == 0) return;

            for (var cnt = 0; cnt < totalList; ++cnt)
            {
                var timer = timerList[cnt];

                //timer match?
                //we don't care if it's active, just care about an Id match
                if (timer.GetTimerID() != aTimerid) continue;

                timer.StopTimer();
                break;
            }
        }

        /// <summary>
        /// return the first unused timer
        /// </summary>
        /// <returns></returns>
        public R_Timer GetUnusedTimer()
        {
            for (var cnt = 0; cnt < totalList; ++cnt)
            {
                var timer = timerList[cnt];

                if (!timer.GetIsActive())
                    return timer;
            }

            return null;
        }

        #endregion

        #region start timers
        /// <summary>
        /// start timer with a given time, the context and the callback
        /// </summary>
        /// <param name="aTimerValue"></param>
        /// <param name="aContext"></param>
        /// <param name="aNewcallback"></param>
        public void StartTimer(float aTimerValue, object aContext, Action<R_Timer> aNewcallback)
        {
            StartTimer(aTimerValue, false, aContext, aNewcallback);
        }

        /// <summary>
        /// start timer with a given time, the context, a callback to get an update every tick, and the callback
        /// </summary>
        /// <param name="aTimerValue"></param>
        /// <param name="aContext"></param>
        /// <param name="aOnUpdateFrameTimer"></param>
        /// <param name="aNewcallback"></param>
        public void StartTimer(float aTimerValue, object aContext, Action<R_Timer> aOnUpdateFrameTimer, Action<R_Timer> aNewcallback)
        {
            StartTimer(aTimerValue, false, aContext, aOnUpdateFrameTimer, aNewcallback);
        }

        /// <summary>
        /// start timer with a given time, a flag to decide if this timer can be stopped or cannot, the context and the callback
        /// </summary>
        /// <param name="aTimerValue"></param>
        /// <param name="aCannotbePaused"></param>
        /// <param name="aContext"></param>
        /// <param name="aNewcallback"></param>
        public void StartTimer(float aTimerValue, bool aCannotbePaused, object aContext, Action<R_Timer> aNewcallback)
        {
			var foundTimer = SetTimer(aTimerValue, aCannotbePaused, aContext, aNewcallback);

			if (foundTimer) return;

			//Si no encontramos ningun timer, reiniciamos el loop y obtenemos uno nuevo
			ResetTimerCounterPosition();
            SetTimer(aTimerValue, aCannotbePaused, aContext, aNewcallback);

        }

        /// <summary>
        /// start timer with a given time, a flag to decide if this timer can be stopped or cannot, the context, a callback to get an update every tick, and the callback
        /// </summary>
        /// <param name="aTimerValue"></param>
        /// <param name="aCannotbePaused"></param>
        /// <param name="aContext"></param>
        /// <param name="aOnUpdateFrameTimer"></param>
        /// <param name="aNewcallback"></param>
        public void StartTimer(float aTimerValue, bool aCannotbePaused, object aContext, Action<R_Timer> aOnUpdateFrameTimer, Action<R_Timer> aNewcallback)
        {
            var foundTimer = SetTimer(aTimerValue, aCannotbePaused, aContext, aOnUpdateFrameTimer, aNewcallback);

            if (foundTimer) return;

            //no timer found, restart loop and get a new one
            ResetTimerCounterPosition();
            SetTimer(aTimerValue, aCannotbePaused, aContext, aOnUpdateFrameTimer, aNewcallback);
        }        

        private bool SetTimer(float aTimerValue, bool aCannotBePaused, object aContext, Action<R_Timer> aNewcallback)
		{
			var foundTimer = false;

			for(var cnt = currentTimerPositionId; cnt < totalList; ++cnt)
			{
				var timer = timerList[cnt];

				if (timer.GetIsActive()) continue;

				//set the new callback and start the timer
				timer.SetContext(aContext);
				timer.SetTimerActionCompletedWithSelfCallback(aNewcallback);
				timer.SetTimerOnUpdateWithSelfCallback(null);
				timer.StartTimer(aTimerValue, aCannotBePaused, 0);

				foundTimer = true;

				SetTimerCounterPosition(cnt);
				IncreaseTimerCounterPosition();

				break;
			}

			return foundTimer;
		}

        private bool SetTimer(float aTimerValue, bool aCannotBePaused, object aContext, Action<R_Timer> aOnUpdateFrameTimer, Action<R_Timer> aNewcallback)
        {
            var foundTimer = false;

            for (var cnt = currentTimerPositionId; cnt < totalList; ++cnt)
            {
                var timer = timerList[cnt];

                if (timer.GetIsActive()) continue;

                //set the new callback and start the timer
                timer.SetContext(aContext);
                timer.SetTimerActionCompletedWithSelfCallback(aNewcallback);
                timer.SetTimerOnUpdateWithSelfCallback(aOnUpdateFrameTimer);
                timer.StartTimer(aTimerValue, aCannotBePaused, 0);

                foundTimer = true;

                SetTimerCounterPosition(cnt);
                IncreaseTimerCounterPosition();

                break;
            }

            return foundTimer;
        }

        #endregion

        #region start timer with id

        /// <summary>
        /// start timer with a given time, the context and the callback, RETURNS and id with the current timer in order to stop it later if needed
        /// </summary>
        /// <param name="aTimerValue"></param>
        /// <param name="aContext"></param>
        /// <param name="aNewcallback"></param>
        /// <returns></returns>
        public uint StartTimerId(float aTimerValue, object aContext, Action<R_Timer> aNewcallback)
        {
            return StartTimerId(aTimerValue, false, aContext, aNewcallback);
        }

        /// <summary>
        /// start timer with a given time, the context and the callback, RETURNS and id with the current timer in order to stop it later if needed
        /// </summary>
        /// <param name="aTimerValue"></param>
        /// <param name="aContext"></param>
        /// <param name="aOnUpdateFrameTimer"></param>
        /// <param name="aNewcallback"></param>
        /// <returns></returns>
        public uint StartTimerId(float aTimerValue, object aContext, Action<R_Timer> aOnUpdateFrameTimer, Action<R_Timer> aNewcallback)
        {
            return StartTimerId(aTimerValue, false, aContext, aOnUpdateFrameTimer, aNewcallback);
        }

        /// <summary>
        /// start timer with a given time, the context and the callback, RETURNS and id with the current timer in order to stop it later if needed
        /// RETURNS an id with the timer
        /// </summary>
        /// <param name="aTimerValue"></param>
        /// <param name="aCannotbePaused"></param>
        /// <param name="aContext"></param>
        /// <param name="aNewcallback"></param>
        /// <returns></returns>
        public uint StartTimerId(float aTimerValue, bool aCannotbePaused, object aContext, Action<R_Timer> aNewcallback)
        {
            var foundId = SetTimerWithId(aTimerValue, aCannotbePaused, aContext, aNewcallback);

            if (foundId != 0) return timerID;

            //no timer found, restart loop and get a new one
            ResetTimerCounterPosition();
            return SetTimerWithId(aTimerValue, aCannotbePaused, aContext, aNewcallback);
        }

        /// <summary>
        /// start timer with a given time, a flag to decide if this timer can be stopped or cannot, the context, a callback to get an update every tick, and the callback
        /// RETURNS an id with the timer
        /// </summary>
        /// <param name="aTimerValue"></param>
        /// <param name="aCannotbePaused"></param>
        /// <param name="aContext"></param>
        /// <param name="aOnUpdateFrameTimer"></param>
        /// <param name="aNewcallback"></param>
        /// <returns></returns>
        public uint StartTimerId(float aTimerValue, bool aCannotbePaused, object aContext, Action<R_Timer> aOnUpdateFrameTimer, Action<R_Timer> aNewcallback)
        {
            var foundId = SetTimerWithId(aTimerValue, aCannotbePaused, aContext, aOnUpdateFrameTimer, aNewcallback);

            if (foundId != 0) return timerID;

            //no timer found, restart loop and get a new one
            ResetTimerCounterPosition();
            return SetTimerWithId(aTimerValue, aCannotbePaused, aContext, aOnUpdateFrameTimer, aNewcallback);
        }


        private uint SetTimerWithId(float aTimerValue, bool aCannotbePaused, object aContext, Action<R_Timer> aNewcallback)
        {
            IncreaseTimerId();

            for (var cnt = currentTimerPositionId; cnt < totalList; ++cnt)
            {
                var timer = timerList[cnt];

                if (timer.GetIsActive()) continue;

                //set the new callback and start the timer
                timer.SetContext(aContext);
                timer.SetTimerActionCompletedWithSelfCallback(aNewcallback);
                timer.SetTimerOnUpdateWithSelfCallback(null);
                timer.StartTimer(aTimerValue, aCannotbePaused, timerID);

                SetTimerCounterPosition(cnt);
                IncreaseTimerCounterPosition();

                return timerID;
            }

            return 0;
        }

        private uint SetTimerWithId(float aTimerValue, bool aCannotbePaused, object aContext, Action<R_Timer> aOnUpdateFrameTimer, Action<R_Timer> aNewcallback)
        {
            IncreaseTimerId();

            for (var cnt = currentTimerPositionId; cnt < totalList; ++cnt)
            {
                var timer = timerList[cnt];

                if (timer.GetIsActive()) continue;

                //set the new callback and start the timer
                timer.SetContext(aContext);
                timer.SetTimerActionCompletedWithSelfCallback(aNewcallback);
                timer.SetTimerOnUpdateWithSelfCallback(aOnUpdateFrameTimer);
                timer.StartTimer(aTimerValue, aCannotbePaused, timerID);

                SetTimerCounterPosition(cnt);
                IncreaseTimerCounterPosition();

                return timerID;
            }

            return 0;
        }

        #endregion


        //Aqui es donde se hace la magia
        #region loop timer internal functions
        private void IncreaseTimerId()
        {
            timerID++;

            //if overflow start again in 1, 0 is the reserved value for a "not used" timer
            if (timerID >= uint.MaxValue)
                timerID = 1;
        }

        private void IncreaseTimerCounterPosition()
        {
            //kind of a way to loop the whole list.
            //each time we found a valid timer we increase this position
            //that way instead of starting the loop from 0 each frame
            //we start from the last position we used as valid
            currentTimerPositionId = (currentTimerPositionId + 1) % totalList;
        }

        private void SetTimerCounterPosition(int aPosition)
		{
			currentTimerPositionId = aPosition;
		}

        private void ResetTimerCounterPosition()
        {
            //after looping and not finding any free timer, we start from 0
            currentTimerPositionId = 0;
        }
        #endregion

        public void IOnUpdate(float aDeltaTime = 1)
        {
            //process 2 timer at once to reduce the N(0) complexity of the list
            for(var cnt = 0; cnt < totalList; cnt += 2)
            {
                var timer = timerList[cnt];
                timer.UpdateTimer();

                var timer2 = timerList[cnt + 1];
                timer2.UpdateTimer();
            }
        }

        //Estos metodos no hacen falta, esta por que los trae la interfaz
        public void IOnFixedUpdate(float aDeltaTime = 1)
        {
            throw new NotImplementedException();
        }

        public void IOnLateUpdate(float aDeltaTime = 1)
        {
            throw new NotImplementedException();
        }
    }
}
