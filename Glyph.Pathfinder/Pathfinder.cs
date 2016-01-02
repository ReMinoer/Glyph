using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Glyph.Pathfinder
{
    public abstract class Pathfinder<TCase, TMove, TAction>
        where TMove : Move<TAction>
    {
        protected readonly TCase[][] Space;
        protected bool[][] ClosedGrid;
        protected PathfinderList Closedlist;
        protected bool[][] DroppedGrid;
        protected PathfinderList Openlist;
        public bool Success { get; protected set; }
        public bool IsEnd { get; protected set; }
        public bool IsReady { get; protected set; }
        public Point Start { get; protected set; }
        public Point Finish { get; protected set; }
        public Point Current { get; protected set; }
        public Stopwatch TimerTimeout { get; protected set; }
        public Stopwatch TimerProcess { get; protected set; }
        protected abstract int Timeout { get; }

        protected Pathfinder(TCase[][] space)
        {
            Space = space;

            Success = false;
            IsEnd = false;
            IsReady = true;

            Openlist = new PathfinderList();
            Closedlist = new PathfinderList();

            ClosedGrid = new bool[space.Length][];
            for (int i = 0; i < ClosedGrid.Length; i++)
                ClosedGrid[i] = new bool[space[0].Length];

            DroppedGrid = new bool[space.Length][];
            for (int i = 0; i < DroppedGrid.Length; i++)
                DroppedGrid[i] = new bool[space[0].Length];

            TimerTimeout = new Stopwatch();
            TimerProcess = new Stopwatch();
        }

        public void Initialize(Point d, Point f)
        {
            Success = false;
            IsReady = true;
            if (!(IsCaseExist(d) && IsCaseExist(f) && IsCaseVide(d)))
            {
                IsReady = false;
                return;
            }
            IsEnd = false;

            Openlist = new PathfinderList();
            Closedlist = new PathfinderList();

            ClosedGrid = new bool[Space.Length][];
            for (int i = 0; i < ClosedGrid.Length; i++)
                ClosedGrid[i] = new bool[Space[0].Length];

            DroppedGrid = new bool[Space.Length][];
            for (int i = 0; i < DroppedGrid.Length; i++)
                DroppedGrid[i] = new bool[Space[0].Length];

            Start = d;
            Finish = f;
            Current = Start;

            Closedlist[Current] = new Node(Current);
            ClosedGrid[Current.Y][Current.X] = true;

            CalculationProcess();

            TimerTimeout.Reset();
            TimerProcess.Reset();
        }

        public bool CalculateRoute()
        {
            if (!IsReady)
                return false;

            TimerTimeout.Start();
            TimerProcess.Start();

            while (Openlist.Any() && TimerTimeout.ElapsedMilliseconds < Timeout)
            {
                Current = BestNodeOpenlist();
                AddClosedList(Current);

                if (Current == Finish)
                    break;

                CalculationProcess();
            }

            TimerProcess.Stop();
            TimerTimeout.Stop();
            TimerTimeout.Reset();

            Success = Current == Finish;
            IsEnd = Success || !Openlist.Any();

            return IsEnd;
        }

        public List<TMove> GetRoute()
        {
            var route = new List<TMove>();

            if (!Closedlist.Any())
                return route;

            Point actual = Closedlist.ContainsKey(Finish) ? Finish : BestNodeClosedlist();

            if (actual == new Point(-1, -1))
                return route;

            Node node = Closedlist[actual];

            route.Add(NewPathfinderMove(node.Parent, new Point(actual.X - node.Parent.X, actual.Y - node.Parent.Y)));

            while (actual != Start)
            {
                actual = node.Parent;
                node = Closedlist[actual];

                route.Add(NewPathfinderMove(node.Parent, new Point(actual.X - node.Parent.X, actual.Y - node.Parent.Y)));
            }

            route.Reverse();
            return route;
        }

        protected abstract void CalculationProcess();
        protected abstract bool IsMoveValid(Point newPoint, Point parent, Point move, int cond = 0);
        protected abstract bool IsCaseVide(Point p);
        protected abstract TMove NewPathfinderMove(Point p, Point m);

        protected void AddSurroundingCases(Point parent, int limitMax, int limitMin = 1, int cond = 0)
        {
            IEnumerable<Point> list = ListPotentialMovesbyInterval(limitMax, limitMin);

            // Regarder une par une toute les possibilités de d'action
            foreach (Point move in list)
            {
                // Définir point de destination
                var newPoint = new Point(parent.X + move.X, parent.Y + move.Y);

                if (!IsMoveValid(newPoint, parent, move, cond))
                    continue;

                // Le déplacement est possible. Calcul du noeud et ajout à l'openlist.
                var newNoeud = new Node(parent);
                float actionCost = (Math.Abs(move.X) + Math.Abs(move.Y) - 1);

                newNoeud.ParentCost = Closedlist[parent].ParentCost + DistanceTo(parent, newPoint);
                newNoeud.PersonalCost = DistanceTo(newPoint, Finish) + actionCost;
                AddOpenList(newPoint, newNoeud);
            }
        }

        protected IEnumerable<Point> ListPotentialMovesbyInterval(int max, int min = 1)
        {
            var list = new List<Point>();

            for (int i = -max; i <= max; i++)
                for (int j = Math.Abs(i) - max; j <= -Math.Abs(i) + max; j++)
                    if (Math.Abs(i) + Math.Abs(j) >= min)
                        list.Add(new Point(i, j));

            return list;
        }

        protected void AddOpenList(Point p, Node n)
        {
            if (Openlist.ContainsKey(p))
            {
                if (n.Cost < Openlist[p].Cost)
                    Openlist[p] = n;
            }
            else
                Openlist[p] = n;
        }

        protected void AddClosedList(Point p)
        {
            Closedlist[p] = Openlist[p];
            ClosedGrid[p.Y][p.X] = true;
            Openlist.Remove(p);
        }

        protected virtual Point BestNodeOpenlist()
        {
            var tmp = new Point(-1, -1);
            float p = float.MaxValue;

            foreach (KeyValuePair<Point, Node> n in Openlist)
            {
                float np = n.Value.Cost;
                if (np < p)
                {
                    tmp = n.Key;
                    p = np;
                }
            }

            return tmp;
        }

        protected virtual Point BestNodeClosedlist()
        {
            var tmp = new Point(-1, -1);
            float d = float.MaxValue;

            foreach (KeyValuePair<Point, Node> n in Closedlist)
            {
                float nd = DistanceTo(n.Key, Finish);
                if (nd < d)
                {
                    d = nd;
                    tmp = n.Key;
                }
            }

            return tmp;
        }

        protected bool IsCaseExist(Point p)
        {
            return (p.Y >= 0) && (p.Y < Space.Length) && (p.X >= 0) && (p.X < Space[0].Length);
        }

        static private float DistanceTo(Point value, Point other)
        {
            return (float)Math.Sqrt(Math.Pow(value.X - other.X, 2) + Math.Pow(value.Y - other.Y, 2));
        }
    }
}