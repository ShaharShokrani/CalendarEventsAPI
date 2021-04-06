using CalendarEvents.DataAccess;
using CalendarEvents.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
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

                string shaniId = "b33f681e-df79-456c-b048-c9fc3e9416e9";
                string shaharId = "6f43e222-04b2-465c-8da3-a9e1f17b3b66";

                byte[] passwordHash, passwordSalt;
                CreatePasswordHash("password", out passwordHash, out passwordSalt);

                var users = new List<UserModel>()
                {
                    new UserModel()
                    {
                        Email = "shani.kotter@gmail.com",
                        Id = shaniId,
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt
                    },
                    new UserModel()
                    {
                        Email = "shahar.shokrani@gmail.com",
                        Id = shaharId,
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt
                    },
                };

                await context.Events.AddRangeAsync(
                    new Models.EventModel()
                    {
                        Base64Id = Guid.NewGuid().ToString(),
                        CreateDate = DateTime.UtcNow,
                        Description = "How can media be used as a tool to share messaging, ignite action, and change perspectives for your students? Join us for this one hour conversation to find out. In the third episode of our Virtual Professional Learning Series, Tools for Anti-Racist Teaching, middle and high school students will take center stage. While focusing on student voice, we will explore ways in which educators can use media to support and amplify their students in their own anti-racist learning journeys.",
                        Details = "",
                        End = DateTime.UtcNow.AddDays(1),
                        Id = Guid.NewGuid(),
                        ImagePath = "https://img.evbuc.com/https%3A%2F%2Fcdn.evbuc.com%2Fimages%2F105182604%2F88522793087%2F1%2Foriginal.20200702-185051?w=800&auto=format%2Ccompress&q=75&sharp=10&rect=0%2C0%2C2160%2C1080&s=0a3f0dfe85b3ef3743fcf24ea797d9c7",
                        IsAllDay = false,
                        OwnerId = shaniId,
                        Start = DateTime.UtcNow.AddDays(1),
                        Title = "Tools for Anti-Racist Teaching: Amplify Student Voice",
                        UpdateDate = DateTime.UtcNow,
                        Audience = 0,
                        Currency = CurrencyISO.GBP.ToString(),
                        Price = 100m,
                        Address = "Aston St, Birmingham B4 7ET, United Kingdom",
                        URL = "https://www.eventbrite.com/e/tools-for-anti-racist-teaching-amplify-student-voice-tickets-112139843290?aff=ebdssbonlinebrowse"
                    },
                    new Models.EventModel()
                    {
                        Base64Id = Guid.NewGuid().ToString(),
                        CreateDate = DateTime.UtcNow,
                        Description = "הכנר, המנצח והמלחין ההולנדי אנדרה ריו חוזר לישראל יחד עם התזמורת והמקהלה הסימפונית שלו Johann Strauss Orchestra ובהשתתפות סולנים אורחים. המופע יתקיים ב-4 וב-5 בנובמבר 2020 בהיכל מנורה מבטחים. סדרת ההופעות הקודמת שלו בישראל ב-2018 היתה סולד אאוט. מחירי הכרטיסים ינועו בין 295-895 שקלים. ",
                        Details = "",
                        End = DateTime.UtcNow.AddDays(5),
                        Id = Guid.NewGuid(),
                        ImagePath = "https://img.haarets.co.il/img/1.5905334/1891145365.jpg?precrop=1766,1025,x0,y121&width=1105&height=640",
                        IsAllDay = false,
                        OwnerId = shaharId,
                        Start = DateTime.UtcNow.AddDays(4),
                        Title = "אנדרה ריו בישראל 2020",
                        UpdateDate = DateTime.UtcNow,
                        Audience = 1,
                        Price = 50m,                        
                        Currency = CurrencyISO.ILS.ToString(),
                        Address = "שדרות תרס\"ט 2, תל אביב יפו",
                        URL = "https://www.haaretz.co.il/gallery/music/events/event-1.5491297"
                    },
                    new Models.EventModel()
                    {
                        Base64Id = Guid.NewGuid().ToString(),
                        CreateDate = DateTime.UtcNow,
                        Description = "אתם מוזמנים לעשות אצלי על האש, אני מזמין הכל ניפגש בזום",
                        Details = "",
                        End = DateTime.UtcNow.AddDays(6),
                        Id = Guid.NewGuid(),
                        ImagePath = "https://old.veg.co.il/CMS/content/uploads/images/veggie-skewers.jpg",
                        IsAllDay = false,
                        OwnerId = shaharId,
                        Start = DateTime.UtcNow.AddDays(6),
                        Title = "על האש צמחוני",
                        UpdateDate = DateTime.UtcNow,
                        Audience = 1,
                        Price = 0m,
                        Address = "המסגר 26, תל אביב, Israel",
                        URL = "https://www.haaretz.co.il/gallery/music/events/event-1.5491297"
                    }
                );
                await context.Users.AddRangeAsync(users);

                await context.SaveChangesAsync();
            }
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
