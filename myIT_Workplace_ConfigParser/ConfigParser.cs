using System;
using System.Collections.Generic;
using System.Text;
using System;
using System.IO;
using System.Xml;

namespace myIT_Workplace_ConfigParser
{
    class ConfigParser
    {
        private string xmlFileName = @"D:\01_Development\VisualStudio\myIT_Workplace_ConfigParser\myIT_Workplace_ConfigParser\myITWP_Config.xml";
        private XmlTextReader reader = null;
        private Boolean inSectionRules = false;
        private Boolean inSectionNavigation = false;
        private Boolean inSectionGroups = false;
        private DataModelFactory df = null;
        private IRuleEngine ruleEng;

        public void setConfigFileName(String filename)
        {
            this.xmlFileName = filename;
        }

        public void parseConfig()
        {
            df = new DataModelFactory();
            ruleEng = new RuleEngine();
            try
            {
                reader = new XmlTextReader(this.xmlFileName);
                reader.WhitespaceHandling = WhitespaceHandling.None;

                while (reader.Read())
                {
                    if ( isStartTag("rules") )
                    {
                        // Rule Section
                        inSectionRules = true;
                        parseRules();
                    }

                    if (isStartTag("navigation"))
                    {
                        // Rule Section
                    }

                    if (isStartTag("groups"))
                    {
                        // Rule Section
                        parseGroups();
                    }

                    if (isStartTag("Pages"))
                    {
                        // Page Section
                    }
                }
            }

            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        private void parseRules()
        {
            Console.WriteLine("Parsing Rules Section...");
            List<String> attributes = new List<String>();

            while (reader.Read() && !isEndTag("rules") )
            {
                
                if (isStartTag("rule"))
                {
                    attributes.Add(reader.GetAttribute("id"));
                    attributes.Add(reader.GetAttribute("name"));
                    Console.WriteLine("id={0}", attributes[0]);
                    Console.WriteLine("name={0}", attributes[1]);
                }

                if (reader.NodeType == XmlNodeType.Text)
                {
                    attributes.Add(reader.Value.Trim());
                }

                if (isEndTag("rule"))
                {
                    df.addRule(attributes[0], attributes[1], attributes[2]);
                    attributes.Clear();
                }
                
            }

            
            Console.WriteLine("Parsing Rules Section DONE!");
        }

        private void parseNavigation()
        {

        }

        private void parseEntity()
        {

        }

        private void parseGroups()
        {
            Console.WriteLine("Parsing Groups Section...");
            List<String> attributes = new List<String>();

            ruleEng.setRules(df.rules);

            while (reader.Read() && !isEndTag("groups"))
            {

                if (isStartTag("group"))
                {
                    attributes.Add(reader.GetAttribute("id"));
                    attributes.Add(reader.GetAttribute("key"));
                    Console.WriteLine("id={0}", attributes[0]);
                    Console.WriteLine("key={0}", attributes[1]);
                }

                if (isStartTag("RuleOK"))
                {
                    String ruleToEvaluate = reader.GetAttribute("id");
                    if (ruleEng.evaluateRule(ruleToEvaluate))
                    {
                        // RuleOK: Handle Tags within Rule
                        parseConditioanlGroupTags();
                        Console.WriteLine("RuleOK id={0}: Evaluate Tags", ruleToEvaluate);

                    } else
                    {
                        Console.WriteLine("RuleOK id={0}: Rule evaluates to NOK", ruleToEvaluate);
                    }
                    
                }

                if (isStartTag("RuleNOK"))
                {
                    String ruleToEvaluate = reader.GetAttribute("id");
                    if (!ruleEng.evaluateRule(ruleToEvaluate))
                    {
                        // RuleNOK: Handle Tags within Rule
                        parseConditioanlGroupTags();
                        Console.WriteLine("RuleNOK id={0}: Evaluate Tags", ruleToEvaluate);
                    }else
                    {
                        Console.WriteLine("RuleNOK id={0}: Rule evaluates to OK", ruleToEvaluate);
                    }
                }

                if (isStartTag("entity"))
                {
                    parseEntity();
                }


                if (isEndTag("group"))
                {
                    df.addGroup();
                    attributes.Clear();
                }

            }


            Console.WriteLine("Parsing Rules Section DONE!");
        }

        private void parseConditioanlGroupTags()
        {
            while (reader.Read() && !(isEndTag("ruleOK") || isEndTag("ruleNOK")) )
            {
                
            }
        }

        private void parseEntityTags()
        {

        }

        private Boolean isStartTag(String tagName)
        {
            Boolean res = false;
            res = reader.NodeType == XmlNodeType.Element && reader.Name.Equals(tagName, StringComparison.OrdinalIgnoreCase);
            return res;
        }

        private Boolean isEndTag(String tagName)
        {
            Boolean res = false;
            res = reader.NodeType == XmlNodeType.EndElement && reader.Name.Equals(tagName, StringComparison.OrdinalIgnoreCase);
            return res;
        }

    }
}
