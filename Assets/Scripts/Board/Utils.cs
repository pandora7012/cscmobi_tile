using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    private static List<Tile.EType> _types = new List<Tile.EType>();
    private static System.Random random;


    public static void Setup(int size)
    {
        foreach (Tile.EType type in Enum.GetValues(typeof(Tile.EType)))
        {
            for (int i = 0; i < 3; i++)
            {
                _types.Add(type);
            }

            size -= 3;
        }

        random = new System.Random();

        while (size > 0)
        {
            Tile.EType type = (Tile.EType)random.Next(Enum.GetValues(typeof(Tile.EType)).Length);
            _types.Add(type);
            _types.Add(type);
            _types.Add(type);
            size -= 3;
        }
    }

    public static Tile.EType GetRandomType()
    {
        if (_types.Count == 0)
        {
            throw new Exception("No more types available");
        }

        int index = random.Next(_types.Count);
        Tile.EType type = _types[index];
        _types.RemoveAt(index);
        return type;
    }

    public static bool CanGetRandomType => _types.Count > 0;
}