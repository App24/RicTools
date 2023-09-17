using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RicUtils.Utilities
{
    public sealed class GameTimer
    {
        public bool Repeat { get; set; }

        public float Timer { get; set; }

        internal bool ended;

        private bool started;

        private float time;

        public bool Paused => !started;

        public event System.Action onTick;

        internal GameTimer(float timer, bool repeat)
        {
            Timer = timer;
            Repeat = repeat;
        }

        internal void Update()
        {
            if (ended || !started) return;

            time += Time.deltaTime;
            if (time >= Timer)
            {
                onTick?.Invoke();
                if (!Repeat) ended = true;
                time -= Timer;
            }
        }

        public void Start()
        {
            started = true;
            time = 0;
        }

        public void Pause()
        {
            started = false;
        }

        public void Unpause()
        {
            started = true;
        }

        public void Remove()
        {
            ended = true;
        }
    }
}
