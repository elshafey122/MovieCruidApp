using CruidApplication.Models;
using CruidApplication.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace CruidApplication.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _toastNotification;
        public MoviesController(ApplicationDbContext context , IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }

        public async Task<IActionResult> Index()
        {
            var movies = await _context.Movies.OrderByDescending(x=>x.Rate).ToListAsync();
            return View(movies);
        }

        public async Task<IActionResult> Create()
        {
            var viewModel = new MovieFormViewModel
            {
                Genres = await _context.Genres.OrderBy(x => x.Name).ToListAsync(),
            };
            return View("MovieForm", viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> create(MovieFormViewModel model)
        {
            //if (!ModelState.IsValid)
            //{
            //    model.Genres = await _context.Genres.OrderBy(x => x.Name).ToListAsync();
            //    return View(model);
            //}
            var files = Request.Form.Files;
            if (!files.Any())
            {
                model.Genres = await _context.Genres.OrderBy(x => x.Name).ToListAsync();
                ModelState.AddModelError("Poster", "please select movie poster");
                return View("MovieForm", model);
            }
            var poster = files.FirstOrDefault();
            var validationpath = new List<string> { ".jpg", ".png" };
            if (!validationpath.Contains(Path.GetExtension(poster.FileName).ToLower()))
            {
                model.Genres = await _context.Genres.OrderBy(x => x.Name).ToListAsync();
                ModelState.AddModelError("Poster", "not allow for adding this path only jpg png");
                return View("MovieForm", model);
            }
            if (poster.Length > 1048576)
            {
                model.Genres = await _context.Genres.OrderBy(x => x.Name).ToListAsync();
                ModelState.AddModelError("Poster", "poster cannot be more than 1mb");
                return View("MovieForm", model);
            }
            using var datastream = new MemoryStream();
            await poster.CopyToAsync(datastream);

            var movies = new Film
            {
                GenreId = model.GenerId,
                Title = model.Title,
                Rate = model.Rate,
                StoreLine = model.StoreLine,
                Year = model.Year,
                Poster = datastream.ToArray()
            };
            _context.Movies.Add(movies);
            _context.SaveChanges();
            _toastNotification.AddSuccessToastMessage("movie created successfully");
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return BadRequest();
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return NotFound();

            var editmovie = new MovieFormViewModel
            {
                Id = movie.Id,
                GenerId = movie.GenreId,
                StoreLine = movie.StoreLine,
                Rate = movie.Rate,
                Title = movie.Title,
                Year = movie.Year,
                Poster = movie.Poster,
                Genres= await _context.Genres.OrderBy(x => x.Name).ToListAsync(),
            };
            return View("MovieForm", editmovie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MovieFormViewModel model)
        {
            if (model.Id == null)
                return BadRequest();
            var movie = await _context.Movies.FindAsync(model.Id);
            if (movie == null)
                return NotFound();

            var files = Request.Form.Files;

            if(files.Any())
            {
                var poster = files.FirstOrDefault();
                using var datastream = new MemoryStream();
                await poster.CopyToAsync(datastream);

                var validationpath = new List<string> { ".jpg", ".png" };

                model.Poster = datastream.ToArray();
                if (!validationpath.Contains(Path.GetExtension(poster.FileName).ToLower()))
                {
                    model.Genres = await _context.Genres.OrderBy(x => x.Name).ToListAsync();
                    ModelState.AddModelError("Poster", "not allow for adding this path only jpg png");
                    return View("MovieForm", model);
                }
                if (poster.Length > 1048576)
                {
                    model.Genres = await _context.Genres.OrderBy(x => x.Name).ToListAsync();
                    ModelState.AddModelError("Poster", "poster cannot be more than 1mb");
                    return View("MovieForm", model);
                }
                movie.Poster = model.Poster;
            }
            movie.Title = model.Title;
            movie.Year = model.Year;
            movie.StoreLine = model.StoreLine;
            movie.Rate = model.Rate;
            movie.GenreId = model.GenerId;

            _context.SaveChanges();
            _toastNotification.AddSuccessToastMessage("movie updated successfully");
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult>Details(int? id)
        {
            if (id == null)
                return BadRequest();

            var movie =await _context.Movies.Include(x=>x.Genre).SingleOrDefaultAsync(m=>m.Id ==id);

            if(movie==null)
            {
                return NotFound();
            }
            return View(movie);
        }

        public async Task<IActionResult>Delete(int ?id)
        {
            if (id == null)
                return BadRequest();
            var movie = _context.Movies.Find(id);

            if(movie==null)
                return NotFound();

            _context.Movies.Remove(movie);
            _context.SaveChanges();

            return Ok();
        }

    }
}
