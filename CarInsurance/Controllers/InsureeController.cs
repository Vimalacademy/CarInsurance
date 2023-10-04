using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CarInsurance.Models;

namespace CarInsurance.Controllers
{
    public class InsureeController : Controller
    {
        private InsuranceEntities db = new InsuranceEntities();

        // GET: Insuree
        public ActionResult Index()
        {
            return View(db.Insurees.ToList());
        }

        // GET: Insuree/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // GET: Insuree/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Insuree/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType")] Insuree insuree)
        {
            if (ModelState.IsValid)
            {
                // Initialize the base monthly price
                decimal monthlyPrice = 50;

                // Calculate age based on DateOfBirth
                int age = DateTime.Now.Year - insuree.DateOfBirth.Year;

                // Apply age-related adjustments
                if (age <= 18)
                {
                    monthlyPrice += 100;
                }
                else if (age >= 19 && age <= 25)
                {
                    monthlyPrice += 50;
                }
                else
                {
                    monthlyPrice += 25;
                }

                // Check car year and make adjustments
                if (insuree.CarYear < 2000)
                {
                    monthlyPrice += 25;
                }
                else if (insuree.CarYear > 2015)
                {
                    monthlyPrice += 25;
                }

                // Check car make and model and make adjustments
                if (insuree.CarMake == "Porsche")
                {
                    monthlyPrice += 25;

                    if (insuree.CarModel == "911 Carrera")
                    {
                        monthlyPrice += 25;
                    }
                }

                // Add additional charges
                monthlyPrice += insuree.SpeedingTickets * 10;

                if (insuree.DUI)
                {
                    monthlyPrice += monthlyPrice * 0.25m; // 25% increase for DUI
                }

                if (insuree.CoverageType)
                {
                    monthlyPrice += monthlyPrice * 0.50m; // 50% increase for full coverage
                }

                // Update the quote property
                insuree.Quote = monthlyPrice;

                // Save to the database
                db.Insurees.Add(insuree);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(insuree);
        }

        // GET: Insuree/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // POST: Insuree/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType,Quote")] Insuree insuree)
        {
            if (ModelState.IsValid)
            {
                db.Entry(insuree).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(insuree);
        }

        // GET: Insuree/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // POST: Insuree/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Insuree insuree = db.Insurees.Find(id);
            db.Insurees.Remove(insuree);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Admin()
        {
            // Retrieve all Insuree records with quotes from the database
            var insureesWithQuotes = db.Insurees.Where(i => i.Quote.HasValue).ToList();

            return View(insureesWithQuotes);
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
