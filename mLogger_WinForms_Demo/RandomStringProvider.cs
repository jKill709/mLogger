using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mLogger_WinForms_Demo
{
    internal class RandomStringProvider
    {
        private readonly List<string> _strings = new();

        public void AddString(string str)
        {
            _strings.Add(str);
        }

        public void AddStrings(IEnumerable<string> strings)
        {
            _strings.AddRange(strings);
        }

        public string GetString()
        {
            if (_strings.Count == 0)
                throw new InvalidOperationException("No strings have been added.");

            return _strings[Random.Shared.Next(_strings.Count)];
        }
    }
}
