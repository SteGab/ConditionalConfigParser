using System;
using myIT_Workplace_ConfigParser;

namespace myIT_Workplace_ConfigParser
{
    class Program
    {
        static void Main(string[] args)
        {
            ConfigParser parser = new ConfigParser();

            ConfigModel cm = parser.parseConfig();

            Console.WriteLine("");
            Console.WriteLine("Rules in Configuration: {0}", cm.rules.Count);
            foreach (var rule in cm.rules)
            {
                Console.WriteLine("Rule {0}", rule.id);
                Console.WriteLine("    Name: {0}", rule.name);
                Console.WriteLine("    Term: {0}", rule.term);
                Console.WriteLine("");
            }

            Console.WriteLine("");
            Console.WriteLine("Groups in Configuration: {0}", cm.groups.Count);
            foreach (var group in cm.groups)
            {
                Console.WriteLine("Group {0}", group.id);
                Console.WriteLine("    Key: {0}", group.translationKey);
                Console.WriteLine("    displayType: {0}", group.displayType);
                Console.WriteLine("    isHidden: {0}", group.isHidden);
                Console.WriteLine("    tooltip: {0}", group.tooltip);
                Console.WriteLine("    Entities: {0}", group.Entities.Count);
                Console.WriteLine("");

                foreach (var entity in group.Entities)
                {
                    Console.WriteLine("    Entity {0}", entity.id);
                    Console.WriteLine("    Key: {0}", entity.translationKey);
                    Console.WriteLine("    displayType: {0}", entity.displayType);
                    Console.WriteLine("    isHidden: {0}", entity.isHidden);
                    Console.WriteLine("    tooltip: {0}", entity.tooltip);
                    Console.WriteLine("    Iconstring: {0}", entity.iconString);
                    Console.WriteLine("    type: {0}", entity.type);
                    Console.WriteLine("    target: {0}", entity.target);
                    Console.WriteLine("    hintIcon: {0}", entity.hintIcon);
                    Console.WriteLine("    hintTooltip: {0}", entity.hintTooltip);
                    Console.WriteLine("    groupId: {0}", entity.groupId);
                    Console.WriteLine("");
                }
                
            }
        }
    }
}
