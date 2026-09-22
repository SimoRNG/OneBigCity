using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace OneBigCity_Model.Classi.Geografia;

public readonly struct Punto3D : IEquatable<Punto3D>
{
    public int X { get; }
    public int Y { get; }
    public int Z { get; }

    [JsonConstructor]
    public Punto3D(int x = 0, int y = 0, int z = 0)
    {
        X = x;
        Y = y;
        Z = z;
    }

    // Confronto di uguaglianza per verificare rapidamente se due entità sono nella stessa posizione
    public bool Equals(Punto3D other) => X == other.X && Y == other.Y && Z == other.Z;

    public override bool Equals(object? obj) => obj is Punto3D other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(X, Y, Z);

    public static bool operator ==(Punto3D left, Punto3D right) => left.Equals(right);
    public static bool operator !=(Punto3D left, Punto3D right) => !left.Equals(right);

    public override string ToString() => $"[X:{X}, Y:{Y}, Z:{Z}]";
}
