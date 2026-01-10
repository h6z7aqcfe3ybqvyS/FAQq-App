using FAQq.Models;
using Microsoft.EntityFrameworkCore;

namespace FAQq.Data
{
    public static class CategorySeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (await context.Categories.AnyAsync())
                return; // już istnieją to nie duplikuj

            var history = new Category { Name = "Historia" };
            var programming = new Category { Name = "Programowanie" };
            var science = new Category { Name = "Nauka" };
            var hardware = new Category { Name = "Sprzęt" };
            var gaming = new Category { Name = "Gry" };
            var movies = new Category { Name = "Filmy" };

            context.Categories.AddRange(
                history, programming, science, hardware, gaming, movies
            );
            await context.SaveChangesAsync();

            var ancient = new Category { Name = "Starożytność", ParentCategoryId = history.Id };
            var csharp = new Category { Name = "C#", ParentCategoryId = programming.Id };
            var physics = new Category { Name = "Fizyka", ParentCategoryId = science.Id };
            var pc = new Category { Name = "PC", ParentCategoryId = hardware.Id };

            var rpg = new Category { Name = "RPG", ParentCategoryId = gaming.Id };
            var fps = new Category { Name = "FPS", ParentCategoryId = gaming.Id };

            var drama = new Category { Name = "Dramat", ParentCategoryId = movies.Id };
            var comedy = new Category { Name = "Komedia", ParentCategoryId = movies.Id };

            context.Categories.AddRange(
                ancient, csharp, physics, pc,
                rpg, fps,
                drama, comedy
            );

            await context.SaveChangesAsync();
        }
    }
}
