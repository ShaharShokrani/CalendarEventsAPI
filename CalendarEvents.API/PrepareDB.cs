using CalendarEvents.DataAccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace CalendarEvents.API
{
    public static class PrepareDB
    {
        public static async Task PreparePopulation(IApplicationBuilder app)
        {            
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                await SeedData(serviceScope.ServiceProvider.GetService<ApplicationDbContext>());
            }
        }

        private static async Task SeedData(ApplicationDbContext context)
        {
            System.Console.WriteLine("Appling Migrations...");
            context.Database.Migrate();

            bool hasAny = await context.Events.AnyAsync();
            if (hasAny)
            {
                Console.WriteLine("Already have data - not seeding");
            }
            else
            {
                Console.WriteLine("Seeding...");

                await context.Events.AddRangeAsync(
                    new Models.EventModel()
                    {
                        Base64Id = Guid.NewGuid().ToString(),
                        CreateDate = DateTime.UtcNow,
                        Description = "How can media be used as a tool to share messaging, ignite action, and change perspectives for your students? Join us for this one hour conversation to find out. In the third episode of our Virtual Professional Learning Series, Tools for Anti-Racist Teaching, middle and high school students will take center stage. While focusing on student voice, we will explore ways in which educators can use media to support and amplify their students in their own anti-racist learning journeys.",
                        Details = "PBS Digital Innovator All Star/PBS KIDS Early Learning Champion Mike Lang and Roshanna Beard will be joined by guests from PBS NEWS HOUR's Student Reporting Labs, to discuss the importance of media and student voice in anti-racist teaching.",
                        End = DateTime.UtcNow.AddDays(1),
                        Id = Guid.NewGuid(),
                        ImagePath = "https://img.evbuc.com/https%3A%2F%2Fcdn.evbuc.com%2Fimages%2F105182604%2F88522793087%2F1%2Foriginal.20200702-185051?w=800&auto=format%2Ccompress&q=75&sharp=10&rect=0%2C0%2C2160%2C1080&s=0a3f0dfe85b3ef3743fcf24ea797d9c7",
                        IsAllDay = false,
                        OwnerId = null,
                        Start = DateTime.UtcNow.AddDays(1),
                        Title = "Tools for Anti-Racist Teaching: Amplify Student Voice",
                        UpdateDate = DateTime.UtcNow,
                        URL = "https://www.eventbrite.com/e/tools-for-anti-racist-teaching-amplify-student-voice-tickets-112139843290?aff=ebdssbonlinebrowse"
                    },
                    new Models.EventModel()
                    {
                        Base64Id = Guid.NewGuid().ToString(),
                        CreateDate = DateTime.UtcNow,
                        Description = "How can media be used as a tool to share messaging, ignite action, and change perspectives for your students? Join us for this one hour conversation to find out. In the third episode of our Virtual Professional Learning Series, Tools for Anti-Racist Teaching, middle and high school students will take center stage. While focusing on student voice, we will explore ways in which educators can use media to support and amplify their students in their own anti-racist learning journeys.",
                        Details = "PBS Digital Innovator All Star/PBS KIDS Early Learning Champion Mike Lang and Roshanna Beard will be joined by guests from PBS NEWS HOUR's Student Reporting Labs, to discuss the importance of media and student voice in anti-racist teaching.",
                        End = DateTime.UtcNow.AddDays(1),
                        Id = Guid.NewGuid(),
                        ImagePath = "https://img.evbuc.com/https%3A%2F%2Fcdn.evbuc.com%2Fimages%2F105182604%2F88522793087%2F1%2Foriginal.20200702-185051?w=800&auto=format%2Ccompress&q=75&sharp=10&rect=0%2C0%2C2160%2C1080&s=0a3f0dfe85b3ef3743fcf24ea797d9c7",
                        IsAllDay = false,
                        OwnerId = null,
                        Start = DateTime.UtcNow,
                        Title = "Tools for Anti-Racist Teaching: Amplify Student Voice",
                        UpdateDate = DateTime.UtcNow,
                        URL = "https://www.eventbrite.com/e/tools-for-anti-racist-teaching-amplify-student-voice-tickets-112139843290?aff=ebdssbonlinebrowse"
                    }
                );

                await context.SaveChangesAsync();
            }
        }
    }
}
