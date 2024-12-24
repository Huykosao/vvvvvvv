using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ManageStudentsV2.Models;
using PagedList;

namespace ManageStudentsV2.Controllers
{
    [RoleAuthorize("Admin")]
    public class Mon_hocController : Controller
    {
        private Quan_Ly_Sinh_Vien_Entities db = new Quan_Ly_Sinh_Vien_Entities();

        // GET: Mon_hoc
        public ActionResult Index(int? size, int? page)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "5", Value = "5" });
            items.Add(new SelectListItem { Text = "10", Value = "10" });
            items.Add(new SelectListItem { Text = "20", Value = "20" });
            items.Add(new SelectListItem { Text = "25", Value = "25" });
            items.Add(new SelectListItem { Text = "50", Value = "50" });
            items.Add(new SelectListItem { Text = "100", Value = "100" });
            items.Add(new SelectListItem { Text = "200", Value = "200" });
            foreach (var item in items)
            {
                if (item.Value == size.ToString()) item.Selected = true;
            }
            ViewBag.size = items; // ViewBag DropDownList
            ViewBag.currentSize = size; // tạo biến kích thước trang hiện tại
            page = page ?? 1;

            var mon_hoc = db.Mon_hoc
                            .Include(mh => mh.Lop_hoc_phan.Select(lhp => lhp.Lop_chinh))
                            .Include(mh => mh.Nganh)
                            .ToList()
                            .OrderBy(mh => mh.ma_nganh);
            int pageSize = (size ?? 5);
            int pageNumber = (page ?? 1);

            return View(mon_hoc.ToPagedList(pageNumber, pageSize));
        }

        // GET: Mon_hoc/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mon_hoc mon_hoc = db.Mon_hoc.Find(id);
            if (mon_hoc == null)
            {
                return HttpNotFound();
            }
            return View(mon_hoc);
        }

        // GET: Mon_hoc/Create
        public ActionResult Create()
        {
            ViewBag.ma_nganh = new SelectList(db.Nganhs, "ma_nganh", "ten_nganh");
            return View();
        }

        // POST: Mon_hoc/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ten_mon,mo_ta_mon, ma_nganh")] Mon_hoc mon_hoc)
        {
            if (ModelState.IsValid)
            {
                db.Mon_hoc.Add(mon_hoc);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(mon_hoc);
        }

        // GET: Mon_hoc/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mon_hoc mon_hoc = db.Mon_hoc.Find(id);
            if (mon_hoc == null)
            {
                return HttpNotFound();
            }
            return View(mon_hoc);
        }

        // POST: Mon_hoc/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ma_mon,ten_mon,mo_ta_mon")] Mon_hoc mon_hoc)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mon_hoc).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(mon_hoc);
        }

        // GET: Mon_hoc/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mon_hoc mon_hoc = db.Mon_hoc.Find(id);
            if (mon_hoc == null)
            {
                return HttpNotFound();
            }
            return View(mon_hoc);
        }

        // POST: Mon_hoc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Mon_hoc mon_hoc = db.Mon_hoc.Find(id);
            db.Mon_hoc.Remove(mon_hoc);
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
    }
}
