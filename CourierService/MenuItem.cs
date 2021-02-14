using System;
using System.Collections.Generic;
using System.Text;

namespace Courier
{
    public interface IMenuItem
    {
       
    }
    public class MenuItem : IMenuItem
    {
        public int Key;
        public Action Action;
        public string Description;
    }
}
