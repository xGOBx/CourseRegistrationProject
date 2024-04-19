using GavenPearl_P1.Controllers;
using GavenPearl_P1.Data;
using GavenPearl_P1.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace CourseControllerTest.ControllersTest
{
    public class CoursesControllerTests
    {

        [Fact]
        public async Task ViewRegisteredCoursesList_ReturnsViewWithCorrespondingCourses_IsValid()
        {

            // Arrange
            var mockContext = new Mock<ApplicationDbContext>();
            var controller = new CoursesController(mockContext.Object);

            Course course1 = new Course
            {
                CourseId = 1,
                CourseCode = "CSCI102",
                Name = "Introduction to Computer Science 2",
                Description = "This course provides an introduction to the fundamental concepts of computer science.",
                Capacity = 30,
                Subject = "Computer Science 2"
            };

            Course course2 = new Course
            {
                CourseId = 2,
                CourseCode = "CSCI101",
                Name = "Introduction to Computer Science",
                Description = "This course provides an introduction to the fundamental concepts of computer science.",
                Capacity = 30,
                Subject = "Computer Science"
            };


            var registeredCourses = new List<RegisteredCourse>
            {
                new RegisteredCourse { CourseId = 1 },
                new RegisteredCourse { CourseId = 2 }
            };

            var correspondingCourses = new List<Course>
            {
                course1, course2
            };

            mockContext.Setup(c => c.RegisteredCourses).ReturnsDbSet(registeredCourses);
            mockContext.Setup(c => c.Course).ReturnsDbSet(correspondingCourses);

            // Act
            var result = await controller.ViewRegisteredCoursesList();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Course>>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task ViewRegisteredCoursesList_ReturnsProblemResult_WhenExceptionThrown_NotValid()
        {
            // Arrange
            var mockContext = new Mock<ApplicationDbContext>();
            var controller = new CoursesController(mockContext.Object);

            mockContext.Setup(c => c.RegisteredCourses).Throws(new Exception("Test exception"));

            // Act
            var result = await controller.ViewRegisteredCoursesList();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            Assert.IsType<ProblemDetails>(objectResult.Value);
            var problemDetails = (ProblemDetails)objectResult.Value;
            Assert.Contains("Test exception", problemDetails.Detail);
        }



        [Fact]
        public async Task Index_WhenCoursesIsValid()
        {

            // Arrange

            Course course1 = new Course
            {
                CourseId = 1,
                CourseCode = "CSCI102",
                Name = "Introduction to Computer Science 2",
                Description = "This course provides an introduction to the fundamental concepts of computer science.",
                Capacity = 30,
                Subject = "Computer Science 2"
            };

            Course course2 = new Course
            {
                CourseId = 2,
                CourseCode = "CSCI101",
                Name = "Introduction to Computer Science",
                Description = "This course provides an introduction to the fundamental concepts of computer science.",
                Capacity = 30,
                Subject = "Computer Science"
            };


            var coursesList = new List<Course> { course1, course2 };

            var _DbMock = new Mock<ApplicationDbContext>();

            _DbMock.Setup(x => x.Course).ReturnsDbSet(coursesList);

            var controller = new CoursesController(_DbMock.Object);

            // Act
            var result = await controller.Index();

            // Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Course>>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task Index_ReturnsProblem_WhenCoursesAreNotValid()
        {
            // Arrange
            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(x => x.Course).Returns((Microsoft.EntityFrameworkCore.DbSet<Course>)null);
            var controller = new CoursesController(mockContext.Object);

            // Act
            var result = await controller.Index();

            // Assert
            var problemResult = Assert.IsType<ObjectResult>(result);
            var problemDetails = Assert.IsType<ProblemDetails>(problemResult.Value);
            Assert.Equal("Entity set 'ApplicationDbContext.Course' is null.", problemDetails.Detail, ignoreCase: true);
        }



        [Fact]
        public async Task ShowSearchForm_ReturnsView()
        {


            // Arrange
            var _DbMock = new Mock<ApplicationDbContext>();
            var controller = new CoursesController(_DbMock.Object);

            // Act
            var result = await controller.ShowSearchForm();

            // Assert
            Assert.IsType<ViewResult>(result);
        }


        [Fact]
        public void AddToCourseList_ReturnsRedirectToIndex_WhenCourseAddedSuccessfully()
        {
            Course course1 = new Course
            {
                CourseId = 1,
                CourseCode = "CSCI102",
                Name = "Introduction to Computer Science 2",
                Description = "This course provides an introduction to the fundamental concepts of computer science.",
                Capacity = 30,
                Subject = "Computer Science 2"
            };

            Course course2 = new Course
            {
                CourseId = 2,
                CourseCode = "CSCI101",
                Name = "Introduction to Computer Science",
                Description = "This course provides an introduction to the fundamental concepts of computer science.",
                Capacity = 30,
                Subject = "Computer Science"
            };

            var coursesList = new List<Course> { course1, course2 };
            var mockContext = new Mock<ApplicationDbContext>();
            var mockCourseSet = new Mock<Microsoft.EntityFrameworkCore.DbSet<Course>>();
            var controller = new CoursesController(mockContext.Object);
            var existingRegisteredCourse = new RegisteredCourse(course1.CourseId);


            mockCourseSet.Setup(c => c.Find(It.IsAny<object[]>()))
                .Returns<object[]>(ids => coursesList.FirstOrDefault(c => c.CourseId == (int)ids[0]));

            mockContext.Setup(x => x.Course).Returns(mockCourseSet.Object);

            mockContext.Setup(c => c.RegisteredCourses)
                .Returns(MockDbSetHelper.CreateMockDbSet(new List<RegisteredCourse> { existingRegisteredCourse }));


            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            controller.TempData = tempData;
            var result = controller.AddToCourseList(course2.CourseId);

            // Assert
            Assert.Null(controller.TempData["ErrorMessage"]);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal("Course added successfully.", controller.TempData["SuccessMessage"]);
        }

        public static class MockDbSetHelper
        {
            public static Microsoft.EntityFrameworkCore.DbSet<T> CreateMockDbSet<T>(List<T> data) where T : class
            {
                var queryableData = data.AsQueryable();
                var mockSet = new Mock<Microsoft.EntityFrameworkCore.DbSet<T>>();
                mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryableData.Provider);
                mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryableData.Expression);
                mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
                mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());
                return mockSet.Object;
            }
        }


        [Fact]
        public void AddToCourseList_ReturnsRedirectToIndex_WhenCourseAlreadyAdded()
        {
            Course course1 = new Course
            {
                CourseId = 1,
                CourseCode = "CSCI102",
                Name = "Introduction to Computer Science 2",
                Description = "This course provides an introduction to the fundamental concepts of computer science.",
                Capacity = 30,
                Subject = "Computer Science 2"
            };

            Course course2 = new Course
            {
                CourseId = 2,
                CourseCode = "CSCI101",
                Name = "Introduction to Computer Science",
                Description = "This course provides an introduction to the fundamental concepts of computer science.",
                Capacity = 30,
                Subject = "Computer Science"
            };

            var coursesList = new List<Course> { course1, course2 };
            var mockContext = new Mock<ApplicationDbContext>();
            var mockCourseSet = new Mock<Microsoft.EntityFrameworkCore.DbSet<Course>>();
            var controller = new CoursesController(mockContext.Object);
            var existingRegisteredCourse = new RegisteredCourse(course1.CourseId);


            mockCourseSet.Setup(c => c.Find(It.IsAny<object[]>()))
                .Returns<object[]>(ids => coursesList.FirstOrDefault(c => c.CourseId == (int)ids[0]));
           
            mockContext.Setup(x => x.Course).Returns(mockCourseSet.Object);
        
            mockContext.Setup(c => c.RegisteredCourses)
                .Returns(MockDbSetHelper.CreateMockDbSet(new List<RegisteredCourse> { existingRegisteredCourse }));


            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            controller.TempData = tempData;
            var result = controller.AddToCourseList(course1.CourseId);


            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal("This course is already added.", controller.TempData["ErrorMessage"]);
            Assert.Null(controller.TempData["SuccessMessage"]);
        }

        [Fact]
        public void Details_ReturnsViewWithCourseDetails_WhenIdIsValid()
        {
            // Arrange
            var expectedCourse = new Course
            {
                CourseId = 1,
                CourseCode = "CSCI101",
                Name = "Introduction to Computer Science",
                Description = "This course provides an introduction to the fundamental concepts of computer science.",
                Capacity = 30,
                Subject = "Computer Science"
            };

            var mockContext = new Mock<ApplicationDbContext>();
            var mockDbSet = new Mock<Microsoft.EntityFrameworkCore.DbSet<Course>>();
            var controller = new CoursesController(mockContext.Object);


            mockDbSet.Setup(m => m.Find(It.IsAny<object[]>()))
                .Returns<object[]>(ids =>
                    ids.FirstOrDefault(id => (int)id == expectedCourse.CourseId) != null ? expectedCourse : null);

            mockContext.Setup(x => x.Course).Returns(mockDbSet.Object);


            // Act
            var result = controller.Details(expectedCourse.CourseId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Course>(viewResult.ViewData.Model);
            Assert.Equal(expectedCourse.CourseId, model.CourseId);
            Assert.Equal(expectedCourse.CourseCode, model.CourseCode);
            Assert.Equal(expectedCourse.Name, model.Name);
            Assert.Equal(expectedCourse.Description, model.Description);
            Assert.Equal(expectedCourse.Capacity, model.Capacity);
            Assert.Equal(expectedCourse.Subject, model.Subject);
        }


        [Fact]
        public void Details_ReturnsNotFound_WhenIdIsNull()
        {
            // Arrange
            var mockContext = new Mock<ApplicationDbContext>();
            var controller = new CoursesController(mockContext.Object);

            // Act
            var result = controller.Details(null);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Details_ReturnsNotFound_WhenCourseIsNull()
        {
            // Arrange
            var courseId = 1;
            var mockContext = new Mock<ApplicationDbContext>();
            var mockDbSet = new Mock<Microsoft.EntityFrameworkCore.DbSet<Course>>();
            mockDbSet.Setup(m => m.FindAsync(courseId)).ReturnsAsync((Course)null);

            mockContext.Setup(x => x.Course).Returns(mockDbSet.Object);

            var controller = new CoursesController(mockContext.Object);

            // Act
            var result = controller.Details(courseId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }


        [Fact]
        public void Details_ReturnsNotFound_WhenCourseDbSetIsNull()
        {
            // Arrange
            var courseId = 1;
            var mockContext = new Mock<ApplicationDbContext>();
            var controller = new CoursesController(mockContext.Object);

            // Act
            var result = controller.Details(courseId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsRedirectToIndex_WhenCourseIsValid()
        {
            // Arrange
            var validCourse = new Course
            {
                CourseId = 1,
                CourseCode = "CSCI101",
                Name = "Introduction to Computer Science",
                Description = "This course provides an introduction to the fundamental concepts of computer science.",
                Capacity = 30,
                Subject = "Computer Science"
            };

            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(x => x.Add(It.IsAny<Course>())).Verifiable();
            mockContext.Setup(x => x.SaveChangesAsync(default(CancellationToken))).ReturnsAsync(1);

            var controller = new CoursesController(mockContext.Object);

            // Act
            var result = await controller.Create(validCourse);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);

            mockContext.Verify(x => x.Add(validCourse), Times.Once);
            mockContext.Verify(x => x.SaveChangesAsync(default(CancellationToken)), Times.Once);


        }

        [Fact]
        public async Task Create_ReturnsViewWithCourse_WhenCourseIsInvalid()
        {
            // Arrange
            var invalidCourse = new Course();

            var mockContext = new Mock<ApplicationDbContext>();

            var controller = new CoursesController(mockContext.Object);
            controller.ModelState.AddModelError("CourseCode", "Course code is required.");

            // Act
            var result = await controller.Create(invalidCourse);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Course>(viewResult.ViewData.Model);
            Assert.Equal(invalidCourse, model);
        }

        [Fact]
        public void DeleteFromCourseList_ReturnsRedirectToViewCourseList_WhenCourseExists()
        {
            // Arrange
            var courseId = 5;
            var mockContext = new Mock<ApplicationDbContext>();
            var mockDbSet = new Mock<Microsoft.EntityFrameworkCore.DbSet<RegisteredCourse>>();
            var registeredCourse = new RegisteredCourse { CourseId = courseId };
            mockDbSet.Setup(m => m.Find(courseId)).Returns(registeredCourse);
            mockContext.Setup(x => x.RegisteredCourses).Returns(mockDbSet.Object);
            var controller = new CoursesController(mockContext.Object);

            // Act
            var result = controller.DeleteFromCourseList(courseId);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ViewRegisteredCoursesList", redirectToActionResult.ActionName);

            mockDbSet.Verify(m => m.Remove(registeredCourse), Times.Once);
            mockContext.Verify(m => m.SaveChanges(), Times.Once);
        }

        [Fact]
        public void DeleteFromCourseList_ReturnsNotFound_WhenCourseDoesNotExist()
        {
            // Arrange
            var courseId = 5;
            var mockContext = new Mock<ApplicationDbContext>();
            var mockDbSet = new Mock<Microsoft.EntityFrameworkCore.DbSet<RegisteredCourse>>();
            mockDbSet.Setup(m => m.Find(courseId)).Returns((RegisteredCourse)null);
            mockContext.Setup(x => x.RegisteredCourses).Returns(mockDbSet.Object);
            var controller = new CoursesController(mockContext.Object);

            // Act
            var result = controller.DeleteFromCourseList(courseId);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            mockDbSet.Verify(m => m.Remove(It.IsAny<RegisteredCourse>()), Times.Never);
            mockContext.Verify(m => m.SaveChanges(), Times.Never);
        }
    }
}
