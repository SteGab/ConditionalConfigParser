using System;
using System.Collections.Generic;
using System.Text;

namespace myIT_Workplace_ConfigParser.Datamodel
{
    class Rule
    {
        public String id;
        public String name;
        public String term;

        public Rule(string id, string name, string term)
        {
            this.id = id ?? throw new ArgumentNullException(nameof(id));
            this.name = name ?? throw new ArgumentNullException(nameof(name));
            this.term = term ?? throw new ArgumentNullException(nameof(term));
        }
    }
}
