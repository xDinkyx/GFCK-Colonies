using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Jobs
{
    public class Job
    {
        public Building building { get; private set; }
        public UnitComponentJob UnitJobComponent = null;

        public JobType type = JobType.Unemployed;
        public List<Task> tasks = new();
        public Task currentTask { get; private set; } = null;

        private bool _forceStop = false;
        public bool repeat = true;

        public Job(Building building, JobType type)
        {
            this.building = building; // All jobs are created by buildings.
            this.type = type;
        }

        public virtual void Start()
        {
            foreach (var task in tasks)
            {
                task.Finished += StartNextTask;
            }

            currentTask = tasks[0];
            currentTask.Start();
        }

        public void AddTask(Task newTask, int index = -1)
        {
            newTask.Finished += StartNextTask;

            if (index == -1) // -1 to add at end.
                tasks.Add(newTask);
            else
                tasks.Insert(index, newTask);
        }

        public void RemoveTask(Task task)
        {
            if(currentTask == task)
                StartNextTask();

            tasks.Remove(task);

            task.Finished -= StartNextTask;
        }

        public virtual void Tick()
        {
            if (_forceStop)
            {
                Finish();
                return;
            }

            currentTask.Tick();
        }

        public virtual void Finish()
        {
            currentTask = null;

            JobManager.Instance.UnregisterJob(this); // Also removes job from unit.
        }

        public virtual void ForceStop()
        {
            _forceStop = true;
        }

        public void StartNextTask()
        {
            int i = tasks.IndexOf(currentTask);
            if (i == tasks.Count - 1)
            {
                if (repeat)
                    currentTask = tasks[0];
                else
                    Finish();
            }
            else
            {
                currentTask = tasks[i + 1];
            }

            currentTask.Start();
        }

        public Unit GetAssignedUnit()
        {
            if (UnitJobComponent == null)
                return null;

            return UnitJobComponent.Owner;
        }

        public string GetCurrentTaskDescription()
        {
            return currentTask.GetTaskDescription();
        }
    }
}
