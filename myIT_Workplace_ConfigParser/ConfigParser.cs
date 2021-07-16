using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace myIT_Workplace_ConfigParser
{
    class ConfigParser
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
            Console.WriteLine("***** Parsing start");
            datamodelFactory = new DataModelFactory();
            ruleEngine = new RuleEngine();

            document = new XPathDocument(xmlFileName);
            navigator = document.CreateNavigator();

            parseRules();
            ruleEngine.setRules(datamodelFactory.rules);

            parseGroups();

            parsePages();

            Console.WriteLine("***** Parsing done");

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
                datamodelFactory.addRule(ruleId, name, term);
            }
        }

        private Boolean evaluateRule(XPathNodeIterator nodes, Boolean invertRule)
        {
            Boolean ruleResult = false;
            if ((nodes.Current.Name == "RuleOK" || nodes.Current.Name == "RuleNOK"))
            {
                String ruleId = nodes.Current.GetAttribute("id", "");
                ruleResult = ruleEngine.evaluateRule(ruleId);   
            }
            if (invertRule) ruleResult = !ruleResult;
            return ruleResult;
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
                parseConfigTags(groupNodes, actGroup);

                // Parse Conditional Group configuration for RuleOK
                groupNodes = groupNavigator.Select("./*[(self::RuleOK)]");
                while (groupNodes.MoveNext())
                {
                    if (evaluateRule(groupNodes, false))
                    {
                        parseConfigTags(nodes, actGroup);
                    }
                }

                // Parse Conditional Group configuration for RuleNOK
                groupNodes = groupNavigator.Select("./*[(self::RuleNOK)]");
                while (groupNodes.MoveNext())
                {
                    if (evaluateRule(groupNodes, true))
                    {
                        parseConfigTags(nodes, actGroup);
                    }
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

        private void parseConfigTags(XPathNodeIterator nodes, UIElement uiElement)
        {
            XPathNavigator groupNavigator = nodes.Current.CreateNavigator();
            XPathNodeIterator groupNodes = null;
            groupNodes = groupNavigator.Select("./*[not(self::Entity) and not(self::RuleOK) and not(self::RuleNOK)]");

            while (groupNodes.MoveNext())
            {
                switch (groupNodes.Current.Name)
                {
                    case "displayType":
                        uiElement.displayType = groupNodes.Current.GetAttribute("type", "");
                        break;
                    case "isHidden":
                        uiElement.isHidden = true;
                        break;
                    case "tooltip":
                        uiElement.tooltip = groupNodes.Current.Value;
                        break;
                    case "IconString":
                        if (uiElement is Entity)
                        {
                            ((Entity)uiElement).iconString = groupNodes.Current.Value;
                        }
                        break;
                    case "Type":
                        if (uiElement is Entity)
                        {
                            ((Entity)uiElement).type = groupNodes.Current.Value;
                        }
                        break;
                    case "Target":
                        if (uiElement is Entity)
                        {
                            ((Entity)uiElement).target = groupNodes.Current.Value;
                        }
                        break;
                    case "Hint":
                        if (uiElement is Entity)
                        {
                            ((Entity)uiElement).hintIcon = groupNodes.Current.GetAttribute("icon","");
                            ((Entity)uiElement).hintTooltip = groupNodes.Current.GetAttribute("tooltip", "");
                        }
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
                actEntity.groupId = group.id;

                // Parse all Entity configuration related Tags
                XPathNavigator entityNavigator = nodes.Current.CreateNavigator();
                XPathNodeIterator subNodes = entityNavigator.Select("./*[self::Entity]");
                parseConfigTags(subNodes, actEntity);

                // Parse Conditional Entity configuration for RuleOK
                subNodes = entityNavigator.Select("./*[self::RuleOK]");
                while (subNodes.MoveNext())
                {
                    if (evaluateRule(subNodes, false))
                    {
                        parseConfigTags(subNodes, actEntity);
                    }
                }

                // Parse Conditional Entity configuration for RuleNOK
                subNodes = entityNavigator.Select("./*[self::RuleNOK]");
                while (subNodes.MoveNext())
                {
                    if (evaluateRule(subNodes, true))
                    {
                        parseConfigTags(subNodes, actEntity);
                    }
                }
            }
        }

        private void parsePages()
        {
            
            

            //Parse Page configuration
            XPathNodeIterator nodes = navigator.Select("/Configuration/Pages/*[self::Page]");
            while (nodes.MoveNext())
            {
                Page actPage = datamodelFactory.addPage();
                actPage.id = nodes.Current.GetAttribute("id", "");
                actPage.name = nodes.Current.GetAttribute("name", "");
                actPage.template = nodes.Current.GetAttribute("template", "");

                XPathNavigator pageNavigator = nodes.Current.CreateNavigator();
                XPathNodeIterator groupNodes = pageNavigator.Select("./*[self::Group]");

                while (groupNodes.MoveNext())
                {
                    actPage.groups.Add(groupNodes.Current.GetAttribute("id", ""));
                }

                XPathNodeIterator ruleNodes = pageNavigator.Select("./*[self::RuleOK]");
                while (ruleNodes.MoveNext())
                {
                    if (evaluateRule(ruleNodes, false))
                    {
                        XPathNodeIterator groupOKNodes = ruleNodes.Current.Select("./*[self::Group]");
                        while (groupOKNodes.MoveNext())
                        {
                            actPage.groups.Add(groupOKNodes.Current.GetAttribute("id", ""));
                        }
                    }
                }

                ruleNodes = pageNavigator.Select("./*[self::RuleNOK]");
                while (ruleNodes.MoveNext())
                {
                    if (evaluateRule(ruleNodes, true))
                    {
                        XPathNodeIterator groupOKNodes = ruleNodes.Current.Select("./*[self::Group]");
                        while (groupOKNodes.MoveNext())
                        {
                            actPage.groups.Add(groupOKNodes.Current.GetAttribute("id", ""));
                        }
                    }
                }

            }

            
            


        }

        
    }
}
