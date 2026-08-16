using System;

namespace FarmTycoon.Workers
{
    public class WorkTask
    {
        public string Id { get; private set; }
        public string Description { get; private set; }
        public TaskPriority Priority { get; set; } = TaskPriority.Normal;
        public WorkerType RequiredWorkerType { get; private set; }
        public string TargetLandId { get; private set; }
        public string TargetMachineId { get; private set; }
        public float EstimatedHours { get; private set; }
        public float Progress { get; private set; } = 0f;
        public bool IsCompleted { get; private set; } = false;
        public bool IsAssigned { get; set; } = false;
        public string AssignedWorkerId { get; set; }
        public DateTime CreatedAt { get; private set; }

        public WorkTask(string id, string description, WorkerType requiredType,
            string landId = null, string machineId = null, float estimatedHours = 2f)
        {
            Id = id;
            Description = description;
            RequiredWorkerType = requiredType;
            TargetLandId = landId;
            TargetMachineId = machineId;
            EstimatedHours = estimatedHours;
            CreatedAt = DateTime.Now;
        }

        public void UpdateProgress(float hoursWorked, float workerPerformance)
        {
            Progress += hoursWorked * workerPerformance / EstimatedHours;
            if (Progress >= 1f)
            {
                Progress = 1f;
                IsCompleted = true;
            }
        }
    }
}
