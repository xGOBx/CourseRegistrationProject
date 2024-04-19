using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GavenPearl_P1.Data;
using GavenPearl_P1.Models;



namespace GavenPearl_P1.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;



        public CoursesController(ApplicationDbContext context)
        {
            _context = context;


        }



        // POST: Courses/AddToCourseList/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddToCourseList(int id)
        {

            var existingRegisteredCourse = _context.RegisteredCourses
                ?.FirstOrDefault(rc => rc.CourseId == id);

            if (existingRegisteredCourse != null)
            {
                TempData["ErrorMessage"] = "This course is already added.";
                return RedirectToAction(nameof(Index));
            }


            var course = _context.Course.Find(id);
            if (course == null)
            {
                return NotFound();
            }


            var registeredCourse = new RegisteredCourse
            {
                
                CourseId = course.CourseId,
            };

            _context.RegisteredCourses.Add(registeredCourse);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Course added successfully.";

            return RedirectToAction(nameof(Index)); 
        }



        // GET: Courses/ViewRegisteredCoursesList
        public async Task<IActionResult> ViewRegisteredCoursesList()
        {
            try
            {
                var registeredCourses = await _context.RegisteredCourses.ToListAsync();

                var correspondingCourses = new List<Course>();

                foreach (var registeredCourse in registeredCourses)
                {
                    var course = await _context.Course.FirstOrDefaultAsync(c => c.CourseId == registeredCourse.CourseId);

                    if (course != null)
                    {
                        correspondingCourses.Add(course);
                    }
                }

                return View(correspondingCourses);
            }
            catch (Exception ex)
            {
                return Problem(
                    title: "An error occurred while retrieving course information",
                    detail: "An error occurred: " + ex.Message,
                    statusCode: 500
                );
            }
        }



        // GET: Courses
        public async Task<IActionResult> Index()
        {
              return _context.Course != null ? 
                          View(await _context.Course.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Course' is null.");
        }

        // GET: Courses/ShowSearchForm
        public async Task<IActionResult> ShowSearchForm()
        {
            return View();
        }

        // Post: Courses/ShowSearchResults
        public async Task<IActionResult> ShowSearchResults(string courseCode, string subject)
        {
            if (_context.Course != null)
            {
                var searchResults = await _context.Course
                    .Where(c => c.CourseCode.Contains(courseCode) && c.Subject.Contains(subject))
                    .ToListAsync();

                return View("Index", searchResults);
            }
            else
            {
                return Problem("Entity set 'ApplicationDbContext.Course' is null.");
            }
        }

        // GET: Courses/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null || _context.Course == null)
            {
                return NotFound();
            }

            var course =  _context.Course
                .Find(id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // GET: Courses/Create
       
        public IActionResult Create()
        {
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseId,CourseCode,Name,Description,Capacity,Subject")] Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(course);
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Course == null)
            {
                return NotFound();
            }

            var course = await _context.Course.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CourseId,CourseCode,Name,Description,Capacity,Subject")] Course course)
        {
            if (id != course.CourseId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.CourseId))
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
            return View(course);
        }

        // POST: Courses/DeleteFromCourseList/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteFromCourseList(int id)
        {
            var registeredCourse = _context.RegisteredCourses.Find(id);
            if (registeredCourse == null)
            {
                return NotFound();
            }

            _context.RegisteredCourses.Remove(registeredCourse);
            _context.SaveChanges();

            return RedirectToAction(nameof(ViewRegisteredCoursesList)); 
        }

        // GET: Courses/Delete/5

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Course == null)
            {
                return NotFound();
            }

            var course = await _context.Course
                .FirstOrDefaultAsync(m => m.CourseId == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
      
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Course == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Course'  is null.");
            }
            var course = await _context.Course.FindAsync(id);
            if (course != null)
            {
                _context.Course.Remove(course);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
          return (_context.Course?.Any(e => e.CourseId == id)).GetValueOrDefault();
        }
    }
}
