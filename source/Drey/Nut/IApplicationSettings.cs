﻿namespace Drey.Nut
{
    public interface IApplicationSettings
    {
        string this[string key] { get; }
    }
}