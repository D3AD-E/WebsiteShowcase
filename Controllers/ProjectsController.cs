﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Website.Core;
using Website.Data;
using Website.Models;
using Website.Models.ViewModels;

namespace Website.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProjectsController(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> All()
        {
            var projectModels = await _context.ProjectModel.ToListAsync();
            if (projectModels == null)
            {
                return NotFound();
            }

            return View(projectModels);
        }

        public async Task<IActionResult> Filter([FromQuery]string language)
        {
            var projectModels = await _context.ProjectModel.Where(p => p.LanguageTag == language).ToListAsync();
            if (projectModels == null)
            {
                return NotFound();
            }

            return View(projectModels);
        }

        public IActionResult Featured() => View();

        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View(await _context.ProjectModel.ToListAsync());
        }

        [Authorize(Roles = "admin")]
        public async Task<RedirectToActionResult> GitGet()
        {
            GitSync gitSync = new GitSync();
            var projects = await gitSync.GetProjects();

            foreach (var project in projects)
            {
                if(!_context.ProjectModel.Any(p => p.Name == project.Name))
                {
                    _context.ProjectModel.Add(project);
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "admin")]
        public async Task<RedirectToActionResult> GitGetAndUpdate()
        {
            GitSync gitSync = new GitSync();
            var projects = await gitSync.GetProjects();

            foreach (var project in projects)
            {
                var dbProject = await _context.ProjectModel.SingleOrDefaultAsync(p => p.Name == project.Name);
                if (dbProject == null)
                {
                    _context.ProjectModel.Add(project);
                }
                else
                {
                    //BAD
                    dbProject.LanguageTag = project.LanguageTag;
                    dbProject.ImageLink = project.ImageLink;
                    dbProject.Description = project.Description;
                    dbProject.DescriptionShort = project.DescriptionShort;
                    //BAD
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Project(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectModel = await _context.ProjectModel
                .SingleOrDefaultAsync(m => m.Id == id);
            if (projectModel == null)
            {
                return NotFound();
            }
            var viewModel = new ProjectViewModel(projectModel);

            var rand = new Random();
            int maxValue = _context.ProjectModel.Count();
            int randId = 0;
            
            for (int i = 0; i < 4; i++)
            {
                ProjectModel gotProject;
                do
                {
                    randId = rand.Next(maxValue);
                } while (id == randId || viewModel.OtherProjects.ContainsKey(randId));

                gotProject = await _context.ProjectModel.SingleOrDefaultAsync(m => m.Id == randId);
                if (gotProject != null)
                    viewModel.OtherProjects.Add(randId, gotProject.ImageLink);
                else
                    i--;      //BAD          
            }

            return View(viewModel);
        }

        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectModel = await _context.ProjectModel.SingleOrDefaultAsync(m => m.Id == id);
            if (projectModel == null)
            {
                return NotFound();
            }

            return View(projectModel);
        }

        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,DescriptionShort,Description,LanguageTag,ImageLink,GithubLink")] ProjectModel projectModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(projectModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(projectModel);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectModel = await _context.ProjectModel.FindAsync(id);
            if (projectModel == null)
            {
                return NotFound();
            }
            return View(projectModel);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,DescriptionShort,Description,LanguageTag,ImageLink,GithubLink")] ProjectModel projectModel)
        {
            if (id != projectModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(projectModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectModelExists(projectModel.Id))
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
            return View(projectModel);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectModel = await _context.ProjectModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (projectModel == null)
            {
                return NotFound();
            }

            return View(projectModel);
        }

        [Authorize(Roles = "admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var projectModel = await _context.ProjectModel.FindAsync(id);
            _context.ProjectModel.Remove(projectModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectModelExists(int id)
        {
            return _context.ProjectModel.Any(e => e.Id == id);
        }
    }
}
