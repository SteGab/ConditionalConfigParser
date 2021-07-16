using System;
using System.Collections.Generic;
using System.Text;

using myIT_Workplace_ConfigParser.Datamodel;

namespace myIT_Workplace_ConfigParser
{
    class DataModelFactory
    {
        public List<Rule> rules = new List<Rule>();
        public List<Group> groups = new List<Group>();

        public void addRule(String id, String name, String term)
        {
            rules.Add(new Rule(id, name, term));
        }

        public Group addGroup()
        {
            Group group = new Group();
            groups.Add(group);
            return group;
        }

        public Entity addEntityToGroup(Group group)
        {
            Entity entity = new Entity();
            group.Entities.Add(entity);
            return entity;
        }

        public ConfigModel getDatamodel()
        {
            var dataModel = new ConfigModel();
            dataModel.groups = groups;
            dataModel.rules = rules;

            return dataModel;
        }
    }
}
