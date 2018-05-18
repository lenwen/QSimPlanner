﻿using System;
using QSP.RouteFinding.Data.Interfaces;
using QSP.RouteFinding.Routes;

namespace QSP.RouteFinding.FileExport.Providers
{
    // TODO: How do other providers handle lat/lon waypoint?

    /// <summary>
    /// Implements the "3 version" format. Supports x-plane 8 to 11.
    /// Specs: https://flightplandatabase.com/dev/specification
    /// </summary>
    /// 
    /// For newer format, see https://developer.x-plane.com/?article=flightplan-files-v11-fms-file-format
    public static class XplaneProvider
    {
        /// <summary>
        /// Get string of the flight plan to export.
        /// </summary>
        /// <exception cref="Exception"></exception>
        public static string GetExportText(Route route)
        {
            throw new NotImplementedException();

            if (route.Count < 2) throw new ArgumentException();
            var from = route.FirstWaypoint;
            var to = route.LastWaypoint;
            var s = @"I
3 version
1
" + (route.Count - 1);


            var firstLine = GetLine(from.ID, from, 1);
            var lastLine = GetLine(to.ID, to, 1);

        }

        // Types:
        // 1 - Airport ICAO
        // 2 - NDB
        // 3 - VOR
        // 11 - Fix
        // 28 - Lat/Lon Position
        private static string GetLine(string id, ICoordinate c, int type)
        {
            var lat = c.Lat.ToString("0.000000");
            var lon = c.Lon.ToString("0.000000");
            return $"{type} {id} 0.000000 {lat} {lon}";
        }
    }
}