﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Rant.Engine
{
    internal partial class VM
    {
        // Item1: Channel buffer index
        // Item2: Channel buffer character index
        // Item3: Channel
        private readonly Dictionary<string, _<int, int, RantChannel>> _markers = new Dictionary<string, _<int, int, RantChannel>>();

        public void SetMarker(string name)
        {
            var ch = CurrentState.Output.GetActive().First();
            _markers[name] = _.Create(ch.CurrentBufferIndex, ch.CurrentBufferLength, ch);
        }

        public int GetMarkerDistance(string a, string b)
        {
            _<int, int, RantChannel> ma, mb;
            if (!_markers.TryGetValue(a, out ma) || !_markers.TryGetValue(b, out mb) || ma.Item3 != mb.Item3) return 0;
            return ma.Item3.MeasureDistance(ma.Item1, mb.Item1, ma.Item2, mb.Item2);
        }

        public string CopyMarkerRegion(string a, string b)
        {
            _<int, int, RantChannel> ma, mb;
            if (!_markers.TryGetValue(a, out ma) || !_markers.TryGetValue(b, out mb) || ma.Item3 != mb.Item3) return String.Empty;
            return ma.Item3.CopyRegion(ma.Item1, mb.Item1, ma.Item2, mb.Item2);
        }
    }
}