using courses.Data;
using courses.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.EntityFrameworkCore;


namespace courses.Controllers;


[ApiController]
[Route("[controller]")]
public class BaseController : ControllerBase
{
    public record CreateCourseRequest(string Name);
    public record CreateStudentRequest(string FullName);

    private readonly AppDbContext _context;

    public BaseController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Object>>> GetCourses()
    {
        var courses = await _context.Courses
            .Include(c => c.Students)
            .Select(c => new
            {
                id = c.Id,
                name = c.Name,
                student = c.Students.Select(s => new
                {
                    id = s.Id,
                    fullName = s.FullName,
                }
                )

            }).ToListAsync();
        return Ok(courses);
    }

    [HttpPost]
    public async Task<ActionResult<object>> CreateCourse([FromBody] CreateCourseRequest request)
    {
        var course = new Course
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
        };
        _context.Courses.Add(course);
        await _context.SaveChangesAsync();
        return Ok(new { id = course.Id });
    }

    [HttpPost("{id:guid}/students")]
    public async Task<ActionResult<object>> CreateStudent(Guid id, [FromBody] CreateStudentRequest request)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null) 
            return NotFound();
        var student = new Student
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName,
            CourseId = id
        };
        _context.Students.Add(student);
        await _context.SaveChangesAsync();
        return Ok(new { id = student.Id });

    }

    [HttpDelete]
    public async Task<ActionResult> DeleteCourse (Guid id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null)
            return NotFound();
        _context.Courses.Remove(course);    
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
