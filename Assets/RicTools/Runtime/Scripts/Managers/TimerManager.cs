using RicTools.Managers;
using RicTools.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RicTools
{
    public class TimerManager : SingletonGenericManager<TimerManager>
    {
        private List<GameTimer> timers = new List<GameTimer>();

        public GameTimer CreateTimer(float timer, bool repeat = true)
        {
            GameTimer gameTimer = new GameTimer(timer, repeat);

            timers.Add(gameTimer);

            return gameTimer;
        }

        private void Update()
        {
            timers.RemoveAll(g => g == null);

            foreach (var timer in timers)
            {
                timer.Update();
            }

            timers.RemoveAll(g => g.ended);
        }
    }
}
