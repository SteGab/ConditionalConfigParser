using System;
using System.Collections.Generic;
using System.Text;

namespace myIT_Workplace_ConfigParser
{
    abstract class UIElement
    {
        public String id;
        public String translationKey = "";
        public String displayType = "";
        public Boolean isHidden = false;
        public String tooltip = "";
    }
}
