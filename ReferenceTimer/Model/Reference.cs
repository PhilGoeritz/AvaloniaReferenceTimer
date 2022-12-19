using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceTimer.Model
{
    internal interface IReference
    {
        string Path { get; }
        string Title { get; }
    }

    internal class Reference : IReference
    {
        public string Title { get; }
        public string Path { get; }

        public Reference(string title, string path)
        {
            Title = title;
            Path = path;
        }
    }
}
