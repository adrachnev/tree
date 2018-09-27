﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NodesMVC.Models;

namespace NodesMVC.Controllers
{
    public class NodesController : Controller
    {
        private NodeContext db = new NodeContext();

        // GET: Nodes
        public ActionResult Index()
        {
            var nodes = db.Nodes.Include(n => n.Parent);
            return View(nodes.ToList());
        }

        // GET: Nodes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Node node = db.Nodes.Find(id);
            if (node == null)
            {
                return HttpNotFound();
            }
            return View(node);
        }

        // GET: Nodes/Create
        public ActionResult Create()
        {
            ViewBag.ParentId = new SelectList(db.Nodes, "Id", "Name");
            return View();
        }

        // POST: Nodes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ParentId,Name")] Node node)
        {
            if (ModelState.IsValid)
            {
                db.Nodes.Add(node);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ParentId = new SelectList(db.Nodes, "Id", "Name", node.ParentId);
            return View(node);
        }

        // GET: Nodes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Node node = db.Nodes.Find(id);
            if (node == null)
            {
                return HttpNotFound();
            }
            ViewBag.ParentId = new SelectList(db.Nodes, "Id", "Name", node.ParentId);
            return View(node);
        }

        // POST: Nodes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ParentId,Name")] Node node)
        {
            if (ModelState.IsValid)
            {
                db.Entry(node).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ParentId = new SelectList(db.Nodes, "Id", "Name", node.ParentId);
            return View(node);
        }

        // GET: Nodes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Node node = db.Nodes.Find(id);
            if (node == null)
            {
                return HttpNotFound();
            }
            return View(node);
        }

        // POST: Nodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Node node = db.Nodes.Find(id);
            db.Nodes.Remove(node);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }



        public void Load(int id)
        {


            var nodes = LoadNodes(id);
            var root = nodes.First(o => o.Id == id);
            MapChildren(root, nodes);




            //var node = db.Nodes.Find(id);
            //LoadChildren(node);

        }

        private void MapChildren(Node node, IEnumerable<Node> nodes)
        {
            node.Children = new List<Node>(nodes.Where(o => o.ParentId == node.Id));
            foreach (var child in node.Children)
            {
                child.Parent = node;
                MapChildren(child, nodes);
            }
        }

        private IEnumerable<Node> LoadNodes(int i)
        {
            var q = string.Format(sqlQuery, i);

            return db.Database.SqlQuery<Node>(q).ToList();
        }


        private readonly string sqlQuery = "WITH Emp_CTE AS ( " +
" SELECT Id, ParentId, Name " +
" FROM dbo.Nodes " +
" WHERE Id = {0} " +
" UNION ALL " +
" SELECT e.Id, e.ParentId, e.Name " +
" FROM dbo.Nodes e " +
" INNER JOIN Emp_CTE ecte ON ecte.Id = e.ParentId " +
" ) " +
" SELECT * " +
" FROM Emp_CTE";

        private void LoadChildren(Node node)
        {
            db.Entry(node).Collection(x => x.Children).Load();
            foreach (var child in node.Children)
            {
                LoadChildren(child);
            }
        }
    }
}
