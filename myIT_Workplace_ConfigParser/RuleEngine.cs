using myIT_Workplace_ConfigParser.Datamodel;
using System;
using System.Collections.Generic;
using System.Text;

namespace myIT_Workplace_ConfigParser
{
    class RuleEngine : IRuleEngine
    {
        private List<Rule> rules;

        public bool evaluateRule(string ruleId)
        {
            Boolean result = false;

            if (rules == null)
            {
                rules = new List<Rule>();
            }

            var rule = rules.Find(r => r.id == ruleId);

            if (rule != null)
            {
                Console.WriteLine("Evaluate Rule {0}; {1}", ruleId, rule.name);
                result = parseTerm(rule.term);
            }else
            {
                Console.WriteLine("Rule Not Found {0}", ruleId);
            }

           

            return result;
        }

        public List<Rule> getRules()
        {
            return rules;
        }

        public void setRules(List<Rule> listOfRules)
        {
            rules = listOfRules;
        }

        private Boolean parseTerm(String ruleTerm)
        {
            Boolean termResult = true;
            //A miracle occurs!!!
            return termResult;
        }
    }
}
