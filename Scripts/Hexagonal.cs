using Godot;
using System;
using System.Collections.Generic;

// See https://www.redblobgames.com/grids/hexagons/

namespace Hexagonal
{
    public struct Point
    {
        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
        public readonly double x;
        public readonly double y;
    }

    public struct Hex
    {
        public Hex(int q, int r)
        {
            this.q = q;
            this.r = r;
            this.s = -q - r;
        }
        public Hex(int q, int r, int s)
        {
            this.q = q;
            this.r = r;
            this.s = s;
            if (q + r + s != 0) throw new ArgumentException("q + r + s must be 0");
        }
        public readonly int q;
        public readonly int r;
        public readonly int s;

        public Hex Add(Hex b)
        {
            return new Hex(q + b.q, r + b.r, s + b.s);
        }
        
        public Hex Subtract(Hex b)
        {
            return new Hex(q - b.q, r - b.r, s - b.s);
        }


        public Hex Scale(int k)
        {
            return new Hex(q * k, r * k, s * k);
        }


        public Hex RotateLeft()
        {
            return new Hex(-s, -q, -r);
        }


        public Hex RotateRight()
        {
            return new Hex(-r, -s, -q);
        }

        static public List<Hex> directions = new List<Hex>{new Hex(1, 0, -1), new Hex(1, -1, 0), new Hex(0, -1, 1), new Hex(-1, 0, 1), new Hex(-1, 1, 0), new Hex(0, 1, -1)};

        static public Hex Direction(int direction)
        {
            return Hex.directions[direction];
        }


        public Hex Neighbor(int direction)
        {
            return Add(Hex.Direction(direction));
        }

        static public List<Hex> diagonals = new List<Hex>{new Hex(2, -1, -1), new Hex(1, -2, 1), new Hex(-1, -1, 2), new Hex(-2, 1, 1), new Hex(-1, 2, -1), new Hex(1, 1, -2)};

        public Hex DiagonalNeighbor(int direction)
        {
            return Add(Hex.diagonals[direction]);
        }


        public int Length()
        {
            return (int)((Math.Abs(q) + Math.Abs(r) + Math.Abs(s)) / 2);
        }


        public int Distance(Hex b)
        {
            return Subtract(b).Length();
        }

    }
    public struct Orientation
    {
        public Orientation(double f0, double f1, double f2, double f3, double b0, double b1, double b2, double b3, double start_angle)
        {
            this.f0 = f0;
            this.f1 = f1;
            this.f2 = f2;
            this.f3 = f3;
            this.b0 = b0;
            this.b1 = b1;
            this.b2 = b2;
            this.b3 = b3;
            this.start_angle = start_angle;
        }
        public readonly double f0;
        public readonly double f1;
        public readonly double f2;
        public readonly double f3;
        public readonly double b0;
        public readonly double b1;
        public readonly double b2;
        public readonly double b3;
        public readonly double start_angle;
    }
    
    public struct FractionalHex
    {
        public FractionalHex(double q, double r, double s)
        {
            this.q = q;
            this.r = r;
            this.s = s;
            if (Math.Round(q + r + s) != 0) throw new ArgumentException("q + r + s must be 0");
        }
        public readonly double q;
        public readonly double r;
        public readonly double s;

        public Hex HexRound()
        {
            int qi = (int)(Math.Round(q));
            int ri = (int)(Math.Round(r));
            int si = (int)(Math.Round(s));
            double q_diff = Math.Abs(qi - q);
            double r_diff = Math.Abs(ri - r);
            double s_diff = Math.Abs(si - s);
            if (q_diff > r_diff && q_diff > s_diff)
            {
                qi = -ri - si;
            }
            else
            if (r_diff > s_diff)
            {
                ri = -qi - si;
            }
            else
            {
                si = -qi - ri;
            }
            return new Hex(qi, ri, si);
        }


        public FractionalHex HexLerp(FractionalHex b, double t)
        {
            return new FractionalHex(q * (1.0 - t) + b.q * t, r * (1.0 - t) + b.r * t, s * (1.0 - t) + b.s * t);
        }


        static public List<Hex> HexLinedraw(Hex a, Hex b)
        {
            int N = a.Distance(b);
            FractionalHex a_nudge = new FractionalHex(a.q + 1e-06, a.r + 1e-06, a.s - 2e-06);
            FractionalHex b_nudge = new FractionalHex(b.q + 1e-06, b.r + 1e-06, b.s - 2e-06);
            List<Hex> results = new List<Hex>{};
            double step = 1.0 / Math.Max(N, 1);
            for (int i = 0; i <= N; i++)
            {
                results.Add(a_nudge.HexLerp(b_nudge, step * i).HexRound());
            }
            return results;
        }

    }
    
    public struct Layout
    {
        public Layout(Orientation orientation, Point size, Point origin)
        {
            this.orientation = orientation;
            this.size = size;
            this.origin = origin;
        }
        public readonly Orientation orientation;
        public readonly Point size;
        public readonly Point origin;
        static public Orientation pointy = new Orientation(Math.Sqrt(3.0), Math.Sqrt(3.0) / 2.0, 0.0, 3.0 / 2.0, Math.Sqrt(3.0) / 3.0, -1.0 / 3.0, 0.0, 2.0 / 3.0, 0.5);
        static public Orientation flat = new Orientation(3.0 / 2.0, 0.0, Math.Sqrt(3.0) / 2.0, Math.Sqrt(3.0), 2.0 / 3.0, 0.0, -1.0 / 3.0, Math.Sqrt(3.0) / 3.0, 0.0);

        public Point HexToPixel(Hex h)
        {
            Orientation M = orientation;
            double x = (M.f0 * h.q + M.f1 * h.r) * size.x;
            double y = (M.f2 * h.q + M.f3 * h.r) * size.y;
            return new Point(x + origin.x, y + origin.y);
        }


        public FractionalHex PixelToHex(Point p)
        {
            Orientation M = orientation;
            Point pt = new Point((p.x - origin.x) / size.x, (p.y - origin.y) / size.y);
            double q = M.b0 * pt.x + M.b1 * pt.y;
            double r = M.b2 * pt.x + M.b3 * pt.y;
            return new FractionalHex(q, r, -q - r);
        }


        public Point HexCornerOffset(int corner)
        {
            Orientation M = orientation;
            double angle = 2.0 * Math.PI * (M.start_angle - corner) / 6.0;
            return new Point(size.x * Math.Cos(angle), size.y * Math.Sin(angle));
        }


        public List<Point> PolygonCorners(Hex h)
        {
            List<Point> corners = new List<Point>{};
            Point center = HexToPixel(h);
            for (int i = 0; i < 6; i++)
            {
                Point offset = HexCornerOffset(i);
                corners.Add(new Point(center.x + offset.x, center.y + offset.y));
            }
            return corners;
        }

    }
}
