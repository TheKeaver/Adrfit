﻿using System;
using System.Collections.Generic;

namespace GameJam
{
    /// <summary>
    /// A manager for controlling the lifecycle and sequence of processes.
    /// </summary>
    public class ProcessManager
    {
        List<FirePorjectileProcess> _processList = new List<FirePorjectileProcess>();

        public FirePorjectileProcess[] Processes
        {
            get
            {
                return _processList.ToArray();
            }
        }

        public void Attach(FirePorjectileProcess process)
        {
            // Instant processes are special; they can be ran in 0 ticks
            if (process is InstantProcess)
            {
                // Doesn't use time, doesn't matter what we enter
                process.Update(0);

                // Attach the next process, if there is one
                if (process.Next != null)
                {
                    Attach(process.Next);
                    process.SetNext(null);
                }

                return;
            }

            _processList.Add(process);
            process.IsActive = true;
        }

        void Detatch(FirePorjectileProcess process)
        {
            _processList.Remove(process);
        }

        internal void Attach()
        {
            throw new NotImplementedException();
        }

        public bool HasProcesses()
        {
            return _processList.Count > 0;
        }

        public void Update(float dt)
        {
            for (int i = 0; i < _processList.Count; i++)
            {
                FirePorjectileProcess curr = _processList[i];

                if (!curr.IsAlive)
                {
                    if (curr.Next != null)
                    {
                        Attach(curr.Next);
                        curr.SetNext(null);
                    }
                    Detatch(curr);
                    i--;
                    continue;
                }
                if (curr.IsActive && !curr.IsPaused)
                {
                    curr.Update(dt);
                }
            }
        }
    }
}
