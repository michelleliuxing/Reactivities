using System;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Controllers;

// Controller for managing activities
public class ActivitiesController(AppDbContext context) : BaseApiController
{
    // GET: api/activities
    [HttpGet]
    public async Task<ActionResult<List<Activity>>> GetActivities()
    {
        // Retrieve and return the list of activities from the database
        return await context.Activities.ToListAsync();
    }

    // GET: api/activities/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Activity>> GetActivityDetail(string id)
    {
        // Find the activity by ID
        var activity = await context.Activities.FindAsync(id);

        // If the activity is not found, return a 404 Not Found response
        if (activity == null) return NotFound();

        // Return the found activity
        return activity;
    }
}
