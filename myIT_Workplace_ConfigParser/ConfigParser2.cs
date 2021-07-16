using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace myIT_Workplace_ConfigParser
{
    class ConfigParser2
    {
        private string xmlFileName = @"D:\01_Development\VisualStudio\myIT_Workplace_ConfigParser\myIT_Workplace_ConfigParser\myITWP_Config.xml";
        private XmlTextReader reader = null;
        private DataModelFactory datamodelFactory = null;
        private IRuleEngine ruleEngine;
        private XPathNavigator navigator;
        private XPathDocument document;

        public void setConfigFileName(String filename)
        {
            this.xmlFileName = filename;
        }

        public ConfigModel parseConfig()
        {
            datamodelFactory = new DataModelFactory();
            ruleEngine = new RuleEngine();

            document = new XPathDocument(xmlFileName);
            navigator = document.CreateNavigator();

            parseRules();
            ruleEngine.setRules(datamodelFactory.rules);

            parseGroups();

            //parsePages();

            Console.WriteLine("Parsing done");

            return datamodelFactory.getDatamodel();
        }


        private void parseRules()
        {
            XPathNodeIterator nodes = navigator.Select("Configuration/Rules/*");
            while (nodes.MoveNext())
            {
                String ruleId = nodes.Current.GetAttribute("id", "");
                String name = nodes.Current.GetAttribute("name", "");
                String term = nodes.Current.Value.Trim();
                Console.WriteLine("id={0}", ruleId);
                Console.WriteLine("name={0}", name);
                Console.WriteLine("term={0}", term);

                datamodelFactory.addRule(ruleId, name, term);
            }
        }

        private Boolean evaluateRule(XPathNodeIterator nodes, Boolean invertRule)
        {
            Boolean ruleResult = false;
            if (nodes.MoveNext() && (nodes.Current.Name == "RuleOK" || nodes.Current.Name == "RuleNOK"))
            {
                String ruleId = nodes.Current.GetAttribute("id", "");
                ruleResult = ruleEngine.evaluateRule(ruleId);   
            }
            if (invertRule) ruleResult = !ruleResult;
            return ruleResult;
        }


        private void parseConditionalGroupTags(XPathNodeIterator nodes, Group group, Boolean invertRule) // invertRule: Negate RuleResult. Use it for RuleNOK is true when rule failed.
        {
            /*if (nodes.Count > 0)
            {
                nodes.MoveNext();
                String ruleId = nodes.Current.GetAttribute("id", "");
                Boolean ruleResult = ruleEngine.evaluateRule(ruleId);
                if (invertRule) ruleResult = !ruleResult;

                if (ruleResult)
                {
                    parseGroupTags(nodes, group);
                }
            }*/
            if (evaluateRule(nodes, invertRule))
            {
                parseGroupTags(nodes, group);
            }
        }

        private void parseGroups()
        {
            XPathNodeIterator nodes = navigator.Select("Configuration/Groups/*[self::Group]");
            while (nodes.MoveNext())
            {
                Group actGroup = datamodelFactory.addGroup();
                actGroup.id = nodes.Current.GetAttribute("id", "");
                actGroup.translationKey = nodes.Current.GetAttribute("key", "");

                // Parse all Group configuration related Tags
                XPathNavigator groupNavigator = nodes.Current.CreateNavigator();
                XPathNodeIterator groupNodes = groupNavigator.Select("./*[not(self::Entity) and not(self::RuleOK) and not(self::RuleNOK)]");
                parseGroupTags(groupNodes, actGroup);

                // Parse Conditional Group configuration for RuleOK
                groupNodes = groupNavigator.Select("./*[(self::RuleOK)]");
                if (evaluateRule(groupNodes, false))
                {
                    parseGroupTags(nodes, actGroup);
                }

                // Parse Conditional Group configuration for RuleNOK
                groupNodes = groupNavigator.Select("./*[(self::RuleNOK)]");
                if (evaluateRule(groupNodes, true))
                {
                    parseGroupTags(nodes, actGroup);
                }

                // Parse all Entities (unconditional)
                groupNodes = groupNavigator.Select("./*[self::Entity]");
                parseEntities(groupNodes, actGroup);

                // Parse all Conditional Entities for RuleOK
                groupNodes = groupNavigator.Select("./RuleOK/*[self::Entity]");
                if (evaluateRule(groupNodes, false))
                {
                    parseEntities(groupNodes, actGroup);
                }

                // Parse all Conditional Entities for RuleOK
                groupNodes = groupNavigator.Select("./RuleNOK/*[self::Entity]");
                if (evaluateRule(groupNodes, true))
                {
                    parseEntities(groupNodes, actGroup);
                }


            }
        }

        private void parseGroupTags(XPathNodeIterator nodes, Group group)
        {
            XPathNavigator groupNavigator = nodes.Current.CreateNavigator();
            XPathNodeIterator groupNodes = groupNavigator.Select("./*[not(self::Entity) and not(self::RuleOK) and not(self::RuleNOK)]");

            while (groupNodes.MoveNext())
            {
                switch (groupNodes.Current.Name)
                {
                    case "displayType":
                        group.displayType = groupNodes.Current.GetAttribute("type", "");
                        break;
                    case "isHidden":
                        group.isHidden = true;
                        break;
                    case "tooltip":
                        group.tooltip = groupNodes.Current.Value;
                        break;

                }
            }
        }

        private void parseEntities(XPathNodeIterator nodes, Group group)
        {

            while (nodes.MoveNext())
            {
                Entity actEntity = datamodelFactory.addEntityToGroup(group);
                actEntity.id = nodes.Current.GetAttribute("id", "");
                actEntity.translationKey = nodes.Current.GetAttribute("name", "");

                // Parse all Entity configuration related Tags
                XPathNavigator entityNavigator = nodes.Current.CreateNavigator();
                XPathNodeIterator subNodes = entityNavigator.Select("./*[self::Entity]");
                parseGroupTags(subNodes, group);

                // Parse Conditional Entity configuration for RuleOK


                // Parse Conditional Entity configuration for RuleNOK

            }
        }

        private void parsePageTags(XPathNodeIterator nodes, Entity entity)
        {

        }
    }
}
