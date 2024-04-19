using System.ComponentModel.DataAnnotations;

namespace GavenPearl_P1.Models
{
    /// <summary>
    /// Represents a registered course.
    /// </summary>
    public class RegisteredCourse
    {
        /// <summary>
        /// Gets or sets the ID of the registered course.
        /// </summary>
        [Key]
        public int CourseId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisteredCourse"/> class.
        /// </summary>
        public RegisteredCourse()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisteredCourse"/> class with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the registered course.</param>
        public RegisteredCourse(int id)
        {
            CourseId = id;
        }
    }
}