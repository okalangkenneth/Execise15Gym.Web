using System;
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
using Gym.Core.Repositories;

namespace Gym.Web.Controllers
{
    //[Authorize(Roles = "Member")]
    public class GymClassesController : Controller
    {
        private readonly UserManager<ApplicationUser> usermanager;
        private readonly IMapper mapper;
        private readonly IUnitOfWork uow;

        public GymClassesController(UserManager<ApplicationUser> usermanager, IMapper mapper, IUnitOfWork uow)
        {
            this.usermanager = usermanager;
            this.mapper = mapper;
            this.uow = uow;
        }

        // GET: GymClasses
        // [Authorize(Roles = "Member")]
        [AllowAnonymous]
        public async Task<IActionResult> Index(IndexViewModel viewModel = null)
        {
            var model = new IndexViewModel();
            var userId = usermanager.GetUserId(User);


            if (!User.Identity.IsAuthenticated)
            {
                 model = mapper.Map<IndexViewModel>(await uow.GymClassRepository.GetAllAsync());
            }

            if (viewModel.ShowHistory)
            {
                model = mapper.Map<IndexViewModel>(await uow.GymClassRepository.GetHistoryAsync(), opt => opt.Items.Add("Id", userId));
            }

            if(User.Identity.IsAuthenticated && !viewModel.ShowHistory)
            {
             model = mapper.Map<IndexViewModel>(await uow.GymClassRepository.GetWithBookingsAsync(), opt => opt.Items.Add("Id", userId));

            }


            //var model = new IndexViewModel
            //{
            //    GymClasses = await db.GymClasses.Include(g => g.AttendingMembers)
            //                        .Select(g => new GymClassesViewModel
            //                        {
            //                            Id = g.Id,
            //                            Name = g.Name,
            //                            Duration = g.Duration,
            //                            Attending = g.AttendingMembers.Any(a => a.ApplicationUserId == userId)
            //                        })
            //                        .ToListAsync()
            //};


            return View(model);
        }

        public async Task<IActionResult> BookingToggle(int? id)
        {
            if (id is null) return BadRequest();

            var userId = usermanager.GetUserId(User);
            ApplicationUserGymClass attending = await uow.AppUserRepo.GetAttending(id, userId);

            if (attending is null)
            {
                var booking = new ApplicationUserGymClass
                {
                    ApplicationUserId = userId,
                    GymClassId = (int)id
                };

                uow.AppUserRepo.Add(booking);
            }
            else
            {
                uow.AppUserRepo.Remove(attending);
            }

            await uow.CompleteAsync();

            return RedirectToAction(nameof(Index));

        }



        public async Task<IActionResult> Bookings()
        {
            var userId = usermanager.GetUserId(User);

            var model = mapper.Map<IndexViewModel>(await uow.AppUserRepo.GetBookingsAsync(userId), opt => opt.Items.Add("Id", userId));

            return View(nameof(Index), model);
        }

        // GET: GymClasses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gymClass = await uow.GymClassRepository.GetAsync(id);
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
                uow.GymClassRepository.Add(gymClass);
                await uow.CompleteAsync();

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

            var gymClass = await uow.GymClassRepository.FindAsync(id);
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
                    uow.GymClassRepository.Update(gymClass);
                    await uow.CompleteAsync();
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


            var gymClass = await uow.GymClassRepository.FindAsync(id);

            if (await TryUpdateModelAsync(gymClass, "", g => g.Name, g => g.Duration))

                try
                {
                    // _context.Update(gymClass);
                    await uow.CompleteAsync();
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

            var gymClass = await uow.GymClassRepository.GetAsync(id);
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
            var gymClass = await uow.GymClassRepository.FindAsync(id);
            uow.GymClassRepository.Remove(gymClass);
            await uow.CompleteAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GymClassExists(int id)
        {
            return uow.GymClassRepository.Any(id);
        }
    }
}
