using Microsoft.AspNetCore.Mvc;
using MovieBooking.Models.Interfaces;

public class UpcomingMoviesViewComponent : ViewComponent
{
    private readonly IMovieRepository _movieRepo;

    public UpcomingMoviesViewComponent(IMovieRepository movieRepo)
    {
        _movieRepo = movieRepo;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var movies = await _movieRepo.GetAllAsync();

        var upcoming = movies
            .Where(m => m.ReleaseDate > DateTime.Today)
            .ToList();

        return View(upcoming);
    }
}
