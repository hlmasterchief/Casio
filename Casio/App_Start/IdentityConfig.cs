using System.Linq;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web;
using Casio.Models;

namespace IdentitySample.Models
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.

    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options,
            IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };
            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;
            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug in here.
            manager.RegisterTwoFactorProvider("PhoneCode", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is: {0}"
            });
            manager.RegisterTwoFactorProvider("EmailCode", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "SecurityCode",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    // Configure the RoleManager used in the application. RoleManager is defined in the ASP.NET Identity core assembly
    public class ApplicationRoleManager : RoleManager<IdentityRole>
    {
        public ApplicationRoleManager(IRoleStore<IdentityRole,string> roleStore)
            : base(roleStore)
        {
        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            return new ApplicationRoleManager(new RoleStore<IdentityRole>(context.Get<ApplicationDbContext>()));
        }
    }

    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your sms service here to send a text message.
            return Task.FromResult(0);
        }
    }

    // This is useful if you do not want to tear down the database each time you run the application.
    // public class ApplicationDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    // This example shows you how to create a new database if the Model changes
    // DropCreateDatabaseIfModelChanges
    public class ApplicationDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext> 
    {
        protected override void Seed(ApplicationDbContext context) {
            // groups
            var groups = new List<Group>
            {
                new Group {Name="DPI1SN"},
                new Group {Name="DPI2SN"},
                new Group {Name="DPI3SN"},
                new Group {Name="DIN14SN"},
                new Group {Name="DIN15SN"}
            };
            groups.ForEach(group => context.Groups.Add(group));
            context.SaveChanges();

            InitializeIdentityForEF(context);
            base.Seed(context);
        }

        //Create Admin User with email=admin@example.com and password=Admin@123456 in the Admin role        
        public static void InitializeIdentityForEF(ApplicationDbContext db) {
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var roleManager = HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();
            const string username = "admin";
            const string name = "Administrator";
            const string email = "admin@example.com";
            const string password = "Admin@123456";
            const string roleName = "Admin";

            //Create Role Admin if it does not exist
            var role = roleManager.FindByName(roleName);
            if (role == null)
            {
                role = new IdentityRole(roleName);
                var roleresult = roleManager.Create(role);
            }

            var user = userManager.FindByName(username);
            if (user == null) {
                user = new ApplicationUser { UserName = username, Email = email, Name = name };
                var result = userManager.Create(user, password);
                result = userManager.SetLockoutEnabled(user.Id, false);
            }


            // Add user admin to Role Admin if not already added
            var rolesForUser = userManager.GetRoles(user.Id);
            if (!rolesForUser.Contains(role.Name)) {
                var result = userManager.AddToRole(user.Id, role.Name);
            }

            // more roles
            string[] roles = new string[] { "Student", "Teacher" };
            foreach (string otherrole in roles)
            {
                var result = roleManager.FindByName(otherrole);
                if (result == null)
                {
                    result = new IdentityRole(otherrole);
                    var roleresult = roleManager.Create(result);
                }
            }


            // password for users
            const string passwordUser = "User@123456";

            // students
            var students = new List<Student>
            {
                new Student { UserName = "t1hemc00", Email = "t1hemc00@students.coamk.fi", Name = "Herbert Mckenzie" },
                new Student { UserName = "t2jera00", Email = "t2jera00@students.coamk.fi", Name = "Jesse Ramsey" },
                new Student { UserName = "t3wigl00", Email = "t3wigl00@students.coamk.fi", Name = "William Gladstone" },
                new Student { UserName = "t4labr00", Email = "t4labr00@students.coamk.fi", Name = "Lavern Bredeson" },
                new Student { UserName = "t5laja00", Email = "t5laja00@students.coamk.fi", Name = "Lanette Jarvie" },
                new Student { UserName = "t1libo00", Email = "t1libo00@students.coamk.fi", Name = "Lilla Bozarth", GroupId = 1 },
                new Student { UserName = "t2lede00", Email = "t2lede00@students.coamk.fi", Name = "Leia Dent", GroupId = 2 },
                new Student { UserName = "t3eltr00", Email = "t3eltr00@students.coamk.fi", Name = "Elvie Trickett", GroupId = 3 },
                new Student { UserName = "t4alka00", Email = "t4alka00@students.coamk.fi", Name = "Alena Kastner", GroupId = 4 },
                new Student { UserName = "t5liso00", Email = "t5liso00@students.coamk.fi", Name = "Lillie Sokoloff", GroupId = 5 },
                new Student { UserName = "t1hiad00", Email = "t1hiad00@students.coamk.fi", Name = "Hilde Adolphsen", GroupId = 1 },
                new Student { UserName = "t2dimu00", Email = "t2dimu00@students.coamk.fi", Name = "Dino Mustafa", GroupId = 2 },
                new Student { UserName = "t3irca00", Email = "t3irca00@students.coamk.fi", Name = "Irina Canez", GroupId = 3 },
                new Student { UserName = "t4kaen00", Email = "t4kaen00@students.coamk.fi", Name = "Kati Enriguez", GroupId = 4 },
                new Student { UserName = "t5jodu00", Email = "t5jodu00@students.coamk.fi", Name = "Jolanda Dunlap", GroupId = 5 }
            };
            foreach (Student student in students)
            {
                var result = roleManager.FindByName(student.UserName);
                if (result == null)
                {
                    var studentesult = userManager.Create(student, passwordUser);
                    studentesult = userManager.SetLockoutEnabled(student.Id, false); //disable fail login lock
                    var roleresult = userManager.AddToRole(student.Id, "Student");
                }
            }

            // teachers
            var teachers = new List<Teacher>
            {
                new Teacher { UserName = "Temple.Guttierrez", Email = "Temple.Guttierrez@students.coamk.fi", Name = "Temple Guttierrez" },
                new Teacher { UserName = "Saundra.Gardella", Email = "Saundra.Gardella@students.coamk.fi", Name = "Saundra Gardella", Title = "Lecturer" },
                new Teacher { UserName = "Freeman.Lirette", Email = "Freeman.Lirette@coamk.fi", Name = "Freeman Lirette", Title = "Senior Lecturer" },
                new Teacher { UserName = "Danyell.Vanfleet", Email = "Danyell.Vanfleet@coamk.fi", Name = "Danyell Vanfleet", Title = "Principal Lecturer" },
                new Teacher { UserName = "Alease.Volk", Email = "Alease.Volk@coamk.fi", Name = "Alease Volk", Title = "Senior Lecturer" }
            };
            foreach (Teacher teacher in teachers)
            {
                var result = roleManager.FindByName(teacher.UserName);
                if (result == null)
                {
                    var teacheresult = userManager.Create(teacher, passwordUser);
                    teacheresult = userManager.SetLockoutEnabled(teacher.Id, false); //disable fail login lock
                    var roleresult = userManager.AddToRole(teacher.Id, "Teacher");
                }
            }

            // courses
            teachers = db.Teachers.ToList();
            var courses = new List<Course>
            {
                new Course { Code = "T909000D", Name = "C++", Credit = 3, TeacherId = teachers.ElementAt(0).Id },
                new Course { Code = "T909001D", Name = "Javascript", Credit = 3, TeacherId = teachers.ElementAt(1).Id },
                new Course { Code = "T909002D", Name = "PHP", Description = "server-side scripting language designed for web development", Credit = 6, TeacherId = teachers.ElementAt(2).Id },
                new Course { Code = "T909003D", Name = "NodeJS", Description = "environment for developing server-side web applications", Credit = 10, TeacherId = teachers.ElementAt(3).Id },
                new Course { Code = "T909004D", Name = "SQL", Credit = 3, TeacherId = teachers.ElementAt(4).Id },
                new Course { Code = "T909005D", Name = "C#", Credit = 6, TeacherId = teachers.ElementAt(0).Id },
                new Course { Code = "T909006D", Name = "Python", Credit = 3, TeacherId = teachers.ElementAt(1).Id },
                new Course { Code = "T909007D", Name = "Swift", Description = "programming language for iOS, OS X", Credit = 6, TeacherId = teachers.ElementAt(2).Id },
                new Course { Code = "T909008D", Name = "Ruby", Description = "general-purpose programming language", Credit = 6, TeacherId = teachers.ElementAt(3).Id },
                new Course { Code = "T909009D", Name = "Java", Credit = 3, TeacherId = teachers.ElementAt(4).Id }
            };
            courses.ForEach(course => db.Courses.Add(course));
            db.SaveChanges();
        }
    }

    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager) : 
            base(userManager, authenticationManager) { }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}