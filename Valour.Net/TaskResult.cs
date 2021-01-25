using System;

namespace Valour.Net
{
    public class TaskResult<T>
    { 
        public string Info { get; set; }
        public bool Success { get; set; }
        public T Data { get; set; }

        public TaskResult(bool success, string info, T data)
        {
            Success = success;
            Info = info;
            Data = data;
        }
        
        public TaskResult(string info)
        {
            Success = false;
            Info = info;
        }

        public override string ToString()
        {
            return Success ? $"[SUCCESS] {Info}" : $"[FATAL] {Info}";
        }
    }
}
