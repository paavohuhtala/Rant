﻿using System;
using System.Collections.Generic;
using System.Text;
using Rant.Engine;
using Rant.Engine.Formatters;
using Rant.Formats;

namespace Rant
{
    /// <summary>
    /// Stores output from a pattern channel.
    /// </summary>
    public sealed class RantChannel
    {
        internal const int InitialBufferSize = 512;

        private RantFormat _format;
        private StringBuilder _currentBuffer;
        private readonly List<StringBuilder> _buffers;
        private readonly Dictionary<string, StringBuilder> _backPrintPoints = new Dictionary<string, StringBuilder>();
        private readonly Dictionary<string, StringBuilder> _forePrintPoints = new Dictionary<string, StringBuilder>();
        private readonly Dictionary<StringBuilder, _<StringBuilder, OutputFormatter>> _articleConverters = new Dictionary<StringBuilder, _<StringBuilder, OutputFormatter>>();

        private readonly OutputFormatter _formatter;

        private int _bufferCount;
        private int _length;

        /// <summary>
        /// The name of the channel.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The visibility of the channel.
        /// </summary>
        public RantChannelVisibility Visiblity { get; internal set; }

        internal OutputFormatter Formatter => _formatter;

        internal RantFormat Format
        {
            get { return _format; }
            set { _format = value; }
        }

        internal RantChannel(string name, RantChannelVisibility visibility, RantFormat format)
        {
            Name = name;
            Visiblity = visibility;
            _currentBuffer = new StringBuilder(InitialBufferSize);
            _buffers = new List<StringBuilder>{_currentBuffer};
            _format = format;
            _formatter = new OutputFormatter();
        }

        internal void Write(string value)
        {
            if (value == null) return;
            _length += value.Length;
            _currentBuffer.Append(_formatter.Format(value, _format));
            UpdateArticle(_currentBuffer);
        }

        internal void WriteArticle()
        {
            char lc = _formatter.LastChar;
            
            var anBuilder = _.Create(new StringBuilder(_formatter.Format(_format.IndefiniteArticles.ConsonantForm, _format, OutputFormatterOptions.NoUpdate | OutputFormatterOptions.IsArticle)), _formatter.Clone());
            var afterBuilder = _currentBuffer = new StringBuilder();
            _articleConverters[afterBuilder] = anBuilder;
            _buffers.Add(anBuilder.Item1);
            _buffers.Add(afterBuilder);
            _bufferCount += 2;
            _length += anBuilder.Item1.Length;
        }

        private void UpdateArticle(StringBuilder target)
        {
            _<StringBuilder, OutputFormatter> aBuilder;
            if (!_articleConverters.TryGetValue(target, out aBuilder)) return;
            int l1 = aBuilder.Item1.Length;
            if (target.Length == 0) // Clear to "a" if the after-buffer is empty
            {
                aBuilder.Item1.Length = 0;
                aBuilder.Item1.Append(aBuilder.Item2.Format(_format.IndefiniteArticles.ConsonantForm, _format, OutputFormatterOptions.NoUpdate | OutputFormatterOptions.IsArticle));
                _length += -l1 + aBuilder.Item1.Length;
                return;
            }

            // Check for vowel
            if (!_format.IndefiniteArticles.PrecedesVowel(target)) return;
            aBuilder.Item1.Length = 0;
            aBuilder.Item1.Append(aBuilder.Item2.Format(_format.IndefiniteArticles.VowelForm, _format, OutputFormatterOptions.NoUpdate | OutputFormatterOptions.IsArticle));
            _length += -l1 + aBuilder.Item1.Length;
        }

        internal void ClearTarget(string name)
        {
            StringBuilder sb;
            if (_backPrintPoints.TryGetValue(name, out sb) || _forePrintPoints.TryGetValue(name, out sb))
            {
                sb.Length = 0;
            }
        }

        internal void WriteToTarget(string name, string value, bool overwrite = false)
        {
            StringBuilder sb;
            if (!_backPrintPoints.TryGetValue(name, out sb))
            {
                sb = _forePrintPoints[name] = new StringBuilder(InitialBufferSize);
                if (overwrite) sb.Length = 0;
                sb.Append(_formatter.Format(value, _format));
            }
            else
            {
                if (overwrite) sb.Length = 0;
                sb.Append(_formatter.Format(value, _format));
                UpdateArticle(sb);
            }
        }

        internal void CreateTarget(string name)
        {
            StringBuilder sb;
            if (_forePrintPoints.TryGetValue(name, out sb))
            {
                _buffers.Add(sb);
                _bufferCount++;
            }
            else
            {
                if (!_backPrintPoints.TryGetValue(name, out sb))
                {
                    sb = _backPrintPoints[name] = new StringBuilder(InitialBufferSize);
                }
                _buffers.Add(sb);
                _bufferCount++;
            }

            _buffers.Add(_currentBuffer = new StringBuilder(InitialBufferSize));
            _bufferCount++;
        }

        internal int MeasureDistance(int bufIndexA, int bufIndexB, int bufCharA, int bufCharB)
        {
            int ia = Math.Min(bufIndexA, bufIndexB);
            int ib = Math.Max(bufIndexA, bufIndexB);
            int len = bufCharB;
            for (int i = ia; i < ib; i++)
            {
                len += _buffers[i].Length;
            }
            return len - bufCharA;
        }

        internal string CopyRegion(int bufIndexA, int bufIndexB, int bufCharA, int bufCharB)
        {
            int ia = Math.Min(bufIndexA, bufIndexB);
            int ib = Math.Max(bufIndexA, bufIndexB);
            if (ia == ib) return _buffers[ia].ToString().Substring(bufCharA, bufCharB - bufCharA);
            var sb = new StringBuilder();
            for (int i = ia; i <= ib; i++)
            {
                if (i == ia)
                {
                    sb.Append(_buffers[i].ToString().Substring(bufCharA));
                }
                else if (i == ib)
                {
                    sb.Append(_buffers[i].ToString().Substring(0, bufCharB));
                }
                else
                {
                    sb.Append(_buffers[i]);
                }
            }
            return sb.ToString();
        }

        internal int CurrentBufferIndex => _bufferCount;

        internal int CurrentBufferLength => _currentBuffer.Length;

        /// <summary>
        /// The number of characters in the output.
        /// </summary>
        public int Length => _length;

        /// <summary>
        /// The output string stored in the channel.
        /// </summary>
        public string Value
        {
            get
            {
                var sb = new StringBuilder(InitialBufferSize);
                foreach (var b in _buffers)
                {
                    sb.Append(b);
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// Returns a string that identifies the channel by name and visibility.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{Name} ({Visiblity})";
    }
}