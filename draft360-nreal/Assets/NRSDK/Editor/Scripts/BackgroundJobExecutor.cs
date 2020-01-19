/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;

    /// @cond EXCLUDE_FROM_DOXYGEN
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
     Justification = "Internal")]
    public class BackgroundJobExecutor
    {
        private AutoResetEvent m_Event = new AutoResetEvent(false);
        private Queue<Action> m_JobsQueue = new Queue<Action>();
        private Thread m_Thread;
        private bool m_Running = false;

        public BackgroundJobExecutor()
        {
            m_Thread = new Thread(Run);
            m_Thread.Start();
        }

        public int PendingJobsCount
        {
            get
            {
                lock (m_JobsQueue)
                {
                    return m_JobsQueue.Count + (m_Running ? 1 : 0);
                }
            }
        }

        public void PushJob(Action job)
        {
            lock (m_JobsQueue)
            {
                m_JobsQueue.Enqueue(job);
            }

            m_Event.Set();
        }

        public void RemoveAllPendingJobs()
        {
            lock (m_JobsQueue)
            {
                m_JobsQueue.Clear();
            }
        }

        private void Run()
        {
            while (true)
            {
                if (PendingJobsCount == 0)
                {
                    m_Event.WaitOne();
                }

                Action job = null;
                lock (m_JobsQueue)
                {
                    if (m_JobsQueue.Count == 0)
                    {
                        continue;
                    }

                    job = m_JobsQueue.Dequeue();
                    m_Running = true;
                }

                job();
                lock (m_JobsQueue)
                {
                    m_Running = false;
                }
            }
        }
    }
    /// @endcond
}
