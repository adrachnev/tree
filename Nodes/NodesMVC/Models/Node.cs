using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace NodesMVC.Models
{
    public class Node
    {

        public int Id { get; set; }

        [ForeignKey(nameof(Parent))]
        public int? ParentId { get; set; }
        public Node Parent { get; set; }

        public string Name { get; set; }

        public ICollection<Node> Children { get; set; } = new List<Node>();
    }

    public class NodeContext : DbContext
    {
        public DbSet<Node> Nodes { get; set; }

        public NodeContext():base ("NodesContext")
        {
           
        }
    }
}

