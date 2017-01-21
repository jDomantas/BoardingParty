using System.Diagnostics;
using static System.Math;

namespace BoardingParty
{
    [DebuggerDisplay("({X}, {Y})")]
    struct Vector
    {
        public static Vector Zero => new Vector(0, 0);
        public static Vector UnitX => new Vector(1, 0);
        public static Vector UnitY => new Vector(0, 1);


        public double X { get; set; }
        public double Y { get; set; }

        public Vector(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double Length => Sqrt(LengthSquared);
        public double LengthSquared => X * X + Y * Y;
        public Vector Normalized => this / Length;
        public Vector Left => new Vector(-Y, X);
        public Vector Right => new Vector(Y, -X);

        public Vector Rotated(double angle)
        {
            Vector newX = new Vector(Cos(angle), Sin(angle));
            Vector newY = newX.Left;
            return X * newX + Y * newY;
        }

        public double Dot(Vector other)
        {
            return X * other.X + Y * other.Y;
        }

        public double Cross(Vector other)
        {
            return X * other.Y - other.X * Y;
        }

        public static Vector operator +(Vector a, Vector b)
        {
            return new Vector(a.X + b.X, a.Y + b.Y);
        }

        public static Vector operator -(Vector a, Vector b)
        {
            return new Vector(a.X - b.X, a.Y - b.Y);
        }

        public static Vector operator -(Vector a)
        {
            return new Vector(-a.X, -a.Y);
        }

        public static Vector operator *(Vector a, double b)
        {
            return new Vector(a.X * b, a.Y * b);
        }

        public static Vector operator *(double a, Vector b)
        {
            return b * a;
        }

        public static Vector operator /(Vector a, double b)
        {
            return new Vector(a.X / b, a.Y / b);
        }
    }
}
