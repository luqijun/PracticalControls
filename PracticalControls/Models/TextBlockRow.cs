using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticalControls.Models
{
    public class TextBlockRow
    {
        public string Header { get; set; }

        public string Content { get; set; }

        public TextBlockRow()
        {

        }

        public TextBlockRow(string header, string content)
        {
            Header = header;
            Content = content;
        }
    }
}
