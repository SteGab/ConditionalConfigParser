using myIT_Workplace_ConfigParser.Datamodel;
using System;
using System.Collections.Generic;
using System.Text;

namespace myIT_Workplace_ConfigParser
{
    interface IRuleEngine
    {

        public Boolean evaluateRule(String ruleId);
        public void setRules(List<Rule> listOfRules);

        public List<Rule> getRules();

    }
}
