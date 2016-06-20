﻿using QSP.LibraryExtension;
using QSP.RouteFinding.Containers;
using QSP.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QSP.RouteFinding.Routes
{
    public class Route : IEnumerable<RouteNode>, IEnumerable
    {
        public LinkedList<RouteNode> Nodes { get; private set; }

        /// <exception cref="NullReferenceException"></exception>
        public Waypoint FirstWaypoint
        {
            get
            {
                return First.Value.Waypoint;
            }
        }

        /// <exception cref="NullReferenceException"></exception>
        public Waypoint LastWaypoint
        {
            get
            {
                return Last.Value.Waypoint;
            }
        }

        public LinkedListNode<RouteNode> First
        {
            get
            {
                return Nodes.First;
            }
        }

        public LinkedListNode<RouteNode> Last
        {
            get
            {
                return Nodes.Last;
            }
        }

        public int Count
        {
            get
            {
                return Nodes.Count;
            }
        }

        public Route()
        {
            Nodes = new LinkedList<RouteNode>();
        }

        public Route(Route item)
        {
            Nodes = new LinkedList<RouteNode>(item);
        }

        public double GetTotalDistance()
        {
            if (Nodes.Count == 0)
            {
                throw new InvalidOperationException("Route is empty.");
            }

            double dis = 0.0;
            var node = First;

            while (node != Last)
            {
                dis += node.Value.DistanceToNext;
                node = node.Next;
            }

            return dis;
        }

        /// <summary>
        /// Add the specified waypoint as the first in the route. 
        /// This waypoint is connected to the next one by the 
        /// airway specified, with the given distance.       
        /// </summary>
        public void AddFirstWaypoint(
            Waypoint item, string viaAirway, double distanceToNext)
        {
            var node = new RouteNode(item, viaAirway, distanceToNext);
            Nodes.AddFirst(node);
        }

        public void AddFirstWaypoint(Waypoint item, string viaAirway)
        {
            var first = Nodes.First;

            double distance =
                first == null ?
                0.0 :
                item.DistanceFrom(first.Value.Waypoint);

            AddFirstWaypoint(item, viaAirway, distance);
        }

        /// <summary>
        /// Append the specified waypoint to the end of the route. 
        /// This waypoint is connected from the previous one by the 
        /// airway specified, with the given distance.       
        /// </summary>
        /// <param name="viaAirway">Airway or SID/STAR name.</param>
        public void AddLastWaypoint(
            Waypoint item, string viaAirway, double distanceFromPrev)
        {
            var last = Nodes.Last;

            if (last != null)  // Route is non-empty.
            {
                last.Value.DistanceToNext = distanceFromPrev;
                last.Value.AirwayToNext = viaAirway;
            }

            Nodes.AddLast(new RouteNode(item, viaAirway, distanceFromPrev));
        }

        public void AddLastWaypoint(Waypoint item, string viaAirway)
        {
            var last = Nodes.Last;

            double distance =
               Last == null ?
               0.0 :
               item.DistanceFrom(last.Value.Waypoint);

            AddLastWaypoint(item, viaAirway, distance);
        }

        /// <summary>
        /// Add the waypoint to the end of the route.
        /// </summary>
        public void AddLastWaypoint(Waypoint item)
        {
            Nodes.AddLast(new RouteNode(item, "", 0.0));
        }

        /// <summary>
        /// Append the given route at the end.
        /// </summary>
        public void AddLast(Route item, string airway)
        {
            double distance =
                Last == null ?
                0.0 :
                LastWaypoint.DistanceFrom(item.FirstWaypoint);

            AddLast(item, airway, distance);
        }

        /// <summary>
        /// Append the given route at the end.
        /// </summary>
        public void AddLast(Route item, string airway, double distance)
        {
            if (Last != null)
            {
                Nodes.Last.Value.AirwayToNext = airway;
                Nodes.Last.Value.DistanceToNext = distance;
            }

            Nodes.AddLast(item.Nodes);
        }

        /// <summary>
        /// Connect the given route at the end of the current one.
        /// The last waypoint of current route must be the same 
        /// as the first one in item.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public void ConnectRoute(Route item)
        {
            if (item.Count == 0)
            {
                return;
            }

            if (Last != null)
            {
                // This route is non-empty.
                ConditionChecker.Ensure<ArgumentException>(
                    LastWaypoint.Equals(item.FirstWaypoint));
                Nodes.RemoveLast();
            }

            Nodes.AddLast(item.Nodes);
        }

        /// <summary>
        /// A string represents the usual route text.
        /// </summary>
        public override string ToString()
        {
            return ToString(false, false);
        }

        /// <summary>
        /// A string represents the usual route text with options.
        /// </summary>
        public string ToString(bool ShowFirstWaypoint, bool ShowLastWaypoint)
        {
            if (Nodes.Count < 2)
            {
                throw new InvalidOperationException(
                    "Number of waypoints in the route is less than 2.");
            }

            var result = new StringBuilder();
            var node = Nodes.First;
            var last = Nodes.Last;

            if (ShowFirstWaypoint)
            {
                result.Append(node.Value.Waypoint.ID + ' ');
            }

            while (node.Next != last)
            {
                if (node.Value.AirwayToNext != node.Next.Value.AirwayToNext ||
                    node.Value.AirwayToNext == "DCT")
                {
                    result.Append(node.Value.AirwayToNext + ' ');
                    node = node.Next;
                    result.Append(node.Value.Waypoint.ID + ' ');
                }
                else
                {
                    node = node.Next;
                }
            }

            result.Append(node.Value.AirwayToNext + ' ');

            if (ShowLastWaypoint)
            {
                result.Append(last.Value.Waypoint.ID);
            }

            return result.ToString();
        }

        public IEnumerator<RouteNode> GetEnumerator()
        {
            return Nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Equals(Route other)
        {
            return Enumerable.SequenceEqual(
                this.Reverse().Skip(1),
                other.Reverse().Skip(1)) &&
                LastWaypoint.Equals(other.LastWaypoint);
        }
    }
}
