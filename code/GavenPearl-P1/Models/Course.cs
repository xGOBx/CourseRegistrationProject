
namespace GavenPearl_P1.Models
{
    /// <summary>
    /// Represents a course.
    /// </summary>
    public class Course
    {
        /// <summary>
        /// Gets or sets the ID of the course.
        /// </summary>
        public int CourseId { get; set; }

        /// <summary>
        /// Gets or sets the code of the course.
        /// </summary>
        public string CourseCode { get; set; }

        /// <summary>
        /// Gets or sets the name of the course.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the course.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the capacity of the course.
        /// </summary>
        public int Capacity { get; set; }

        /// <summary>
        /// Gets or sets the subject of the course.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Course"/> class.
        /// </summary>
        public Course()
        {

        }
    }
}