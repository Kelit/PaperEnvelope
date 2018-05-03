using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvelopePaper.Class
{
    public interface IFIleManager
    {
        int GetSymbolCount(string content);
    }
    class FileManager
    {
        public int GetSymbolCount(string content)
        {
            int count = content.Length;
            return count;
        }
    }
}
