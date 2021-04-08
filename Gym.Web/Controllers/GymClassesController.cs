﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Gym.Core.Entities;
using Gym.Data.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Gym.Web.Extensions;
using Gym.Core.ViewModels;
using AutoMapper;
using Gym.Data.Repositories;

namespace Gym.Web.Controllers
{
    //[Authorize(Roles = "Member")]
    public class GymClassesController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly ApplicationUserGymClassRepository applicationUserGymClassRepository;
        private GymClassRepository gymClassRepository;
        private readonly UserManager<ApplicationUser> usermanager;
        private readonly IMapper mapper;

        public GymClassesController(ApplicationDbContext context, UserManager<ApplicationUser> usermanager, IMapper mapper)
        {
            db = context;
            applicationUserGymClassRepository = new ApplicationUserGymClassRepository(context);
            gymClassRepository = new GymClassRepository(context);
            this.usermanager = usermanager;
            this.mapper = mapper;
        }

        // GET: GymClasses
        // [Authorize(Roles = "Member")]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
               
                var model1 = new IndexViewModel
                {
                    GymClasses = await db.GymClasses.Include(g => g.AttendingMembers)
                                    .Select(g => new GymClassesViewModel
                                    {
                                        Id = g.Id,
                                        Name = g.Name,
                                        Duration = g.Duration,
                                        // Attending = g.AttendingMembers.Any(a => a.ApplicationUserId == userId)
                                    })
                                    .ToListAsync()
                };

               return View(model1);

            }


            var userId = usermanager.GetUserId(User);
           // var m = mapper.Map<IEnumerable<GymClassesViewModel>>(db.GymClasses, opt => opt.Items.Add("Id", userId));
            var model = new IndexViewModel
            {
                GymClasses = await db.GymClasses.Include(g => g.AttendingMembers)
                                    .Select(g => new GymClassesViewModel
                                    {
                                        Id = g.Id,
                                        Name = g.Name,
                                        Duration = g.Duration,
                                        Attending = g.AttendingMembers.Any(a => a.ApplicationUserId == userId)
                                    })
                                    .ToListAsync()
            };

       
            return View(model);
        }

        public async Task<IActionResult> BookingToggle(int? id)
        {
            if (id is null) return BadRequest();

            var userId = usermanager.GetUserId(User);
            ApplicationUserGymClass attending = await applicationUserGymClassRepository.GetAttending(id, userId);

            if (attending is null)
            {
                var booking = new ApplicationUserGymClass
                {
                    ApplicationUserId = userId,
                    GymClassId = (int)id
                };

                db.ApplicationUserGyms.Add(booking);
            }
            else
            {
                db.ApplicationUserGyms.Remove(attending);
            }

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

      

        public async Task<IActionResult> Bookings()
        {
            var userId = usermanager.GetUserId(User);

            var model = db.ApplicationUserGyms
                            .IgnoreQueryFilters()
                            .Where(u => u.ApplicationUserId == userId)
                            .Select(a => a.GymClass);

            return View(nameof(Index), await model.ToListAsync());
        }

        // GET: GymClasses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gymClass = await db.GymClasses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gymClass == null)
            {
                return NotFound();
            }

            return View(gymClass);
        }

        // GET: GymClasses/Create
        public IActionResult Create()
        {
            return Request.IsAjax() ? PartialView("CreatePartial") : View();
        }

        // POST: GymClasses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,StartDate,Duration,Description")] GymClass gymClass)
        {
            if (ModelState.IsValid)
            {
                db.Add(gymClass);
                await db.SaveChangesAsync();

                if (Request.IsAjax())
                {
                    return PartialView("GymClassPartial", gymClass);
                }

                return RedirectToAction(nameof(Index));
            }
            return View(gymClass);
        }

      //ToDo: Fix!
      //  [IsAjax]
        public ActionResult Fetch()
        {
            return PartialView("CreatePartial");
        }

        // GET: GymClasses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gymClass = await db.GymClasses.FindAsync(id);
            if (gymClass == null)
            {
                return NotFound();
            }
            return View(gymClass);
        }

        // POST: GymClasses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,StartDate,Duration,Description")] GymClass gymClass)
        {
            if (id != gymClass.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(gymClass);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GymClassExists(gymClass.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(gymClass);
        } 
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewEdit(int? id)
        {
            if (id is null)
                return BadRequest();


            var gymClass = db.GymClasses.Find(id);

            if(await TryUpdateModelAsync(gymClass, "", g => g.Name, g => g.Duration))

                try
                {
                   // _context.Update(gymClass);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GymClassExists(gymClass.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
              //  return RedirectToAction(nameof(Index));
            
            return View(gymClass);
        }

        // GET: GymClasses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gymClass = await db.GymClasses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gymClass == null)
            {
                return NotFound();
            }

            return View(gymClass);
        }

        // POST: GymClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gymClass = await db.GymClasses.FindAsync(id);
            db.GymClasses.Remove(gymClass);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GymClassExists(int id)
        {
            return db.GymClasses.Any(e => e.Id == id);
        }
    }
}
