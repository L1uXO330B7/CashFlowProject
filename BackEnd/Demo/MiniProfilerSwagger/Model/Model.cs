﻿namespace MiniProfilerSwagger.Model
{
    public class Result<T>
    {
        public T Data { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; } = true;
    }
}
