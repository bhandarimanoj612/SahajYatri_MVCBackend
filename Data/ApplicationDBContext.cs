using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Sahaj_Yatri.Models;
//using Sahaj_Yatri.Models.Shared;

namespace Sahaj_Yatri.Data
{
    public class ApplicationDBContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDBContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

       // db set for hotel

        public DbSet<Hotel> Hotels { get; set; } //for pushing data to the databases
        public DbSet<Review> Reviews { get; set; } //for pushing data to the databases


        //travel packages
        public DbSet<Travel> Travel { get; set; }

        public DbSet<Vehicle> Vehicles { get; set; }

        public DbSet<Offer> Offers { get; set; }
        public DbSet<SafetyTip> SafetyTips { get; set; }

        //Hotel booking
        public DbSet<HotelBooking> HotelBookings { get; set; }
        
        public DbSet<VehicleBooking> VehicleBookings { get; set; }
        public DbSet<TravelBooking> TravelBookings { get; set; }
        public DbSet<Favorite> Favorites { get; set; }

        //public object Message { get; internal set; }

        //for budget management 
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Expense> Expenses { get; set; }


        //seeding data to the database of hotels
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);// this is need because of the identity table 
                                               // Seed safety tip data
            modelBuilder.Entity<SafetyTip>().HasData(
                new SafetyTip { Id = 1, Text = "Carry identification and emergency contact information with you at all times." },
                new SafetyTip { Id = 2, Text = "Secure your belongings and avoid leaving them unattended." },
                new SafetyTip { Id = 3, Text = "Be cautious about sharing personal information, especially with strangers." },
                new SafetyTip { Id = 4, Text = "Follow safety instructions provided by tour guides or authorities." },
                new SafetyTip { Id = 5, Text = "Stay vigilant and aware of your surroundings." },
                new SafetyTip { Id = 6, Text = "In case of emergency, call the provided emergency number." },
                new SafetyTip { Id = 7, Text = "Avoid traveling alone, especially at night or in unfamiliar areas." },
                new SafetyTip { Id = 8, Text = "Keep a copy of important documents such as passports and visas in a secure location." },
                new SafetyTip { Id = 9, Text = "Stay hydrated and carry a first aid kit with essential medications." },
                new SafetyTip { Id = 10, Text = "Trust your instincts and avoid situations that feel unsafe." }
            );
        }
    }
       

    }

