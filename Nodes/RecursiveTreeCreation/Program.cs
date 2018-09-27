using NodesMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecursiveTreeCreation
{
    class Program
    {
        static readonly int HIERARCHY_COUNT = 2;
        static int CURRENT_HIERARCHY;
        static readonly int CHILDREN_COUNT = 5;
        static void Main(string[] args)
        {
            using (var context = new NodeContext())
            {
                var count = context.Nodes.Count();

                //context.Database.ExecuteSqlCommand("delete Nodes");

                var root = AddChild(null, "root", context);
                CURRENT_HIERARCHY = 0;
                CreateChildren(new List<Node> { root }, CURRENT_HIERARCHY, context);
                context.SaveChanges();
            }
        }

        static void CreateChildren(List<Node> parents, int hierarchy, NodeContext context)
        {
            if (hierarchy >= HIERARCHY_COUNT)
                return;

            List<Node> createdChildren = new List<Node>();
            foreach (var item in parents)
            {
                for (int i = 1; i <= CHILDREN_COUNT; i++)
                {
                    var child = AddChild(item, item.Name + "." + i, context);
                    createdChildren.Add(child);
                }
            }

            CURRENT_HIERARCHY = CURRENT_HIERARCHY + 1;


            CreateChildren(createdChildren, CURRENT_HIERARCHY, context);

        }

        static Node AddChild(Node parent, string name, NodeContext context)
        {
            name = (name.Contains("root.")) ? name.Replace("root.", string.Empty) : name;

            var res = new Node { Name = name, Parent = parent };
            context.Nodes.Add(res);

            return res;


        }
    }
}
