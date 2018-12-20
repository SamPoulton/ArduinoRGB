using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArduinoRGBGui
{
    public class ColumnGroupItem
    {
        public List<string> Data;

        public ColumnGroupItem(params string[] data)
        {
            foreach (string item in data) 
            Data.Add(item);
        }
    }
}
