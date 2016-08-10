using System;
using System.Collections.Generic;
using System.Text;

namespace QuartzBeginnerExample
{
    /// <summary>
    /// Interface for examples.
    /// </summary>
    public interface IExample
    {
        string Name
        {
            get;
        }

        void Run();
    }
}
