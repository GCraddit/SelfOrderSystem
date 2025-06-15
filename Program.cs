using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows.Forms;
using System.Configuration;
using Giles_Chen_test_1;



namespace Giles_Chen_test_1
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //TestQueryStaffs();

            var serviceProvider = ConfigureServices();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ApplicationConfiguration.Initialize();
            Application.Run(new WelcomeForm(serviceProvider));
            //Console.WriteLine("Press any key to exit...");
            //Console.ReadKey();
        }


        public static IServiceProvider ConfigureServices()
        {
            var serviceCollection = new ServiceCollection();

            //  app.config 
            string connectionString = ConfigurationManager.ConnectionStrings["CafeDatabase"].ConnectionString;

            //  DbContext
            serviceCollection.AddDbContext<CafeContext>(options =>
                options.UseSqlServer(connectionString));

           
            serviceCollection.AddTransient<WelcomeForm>();
            serviceCollection.AddTransient<StaffLoginForm>();
            serviceCollection.AddTransient<StaffMenuForm>();
            serviceCollection.AddTransient<AddMenuItemForm>();
            serviceCollection.AddTransient<AddMerchForm>();
            serviceCollection.AddTransient<AddProductForm>();
            serviceCollection.AddTransient<OrderForm>();
            serviceCollection.AddTransient<MerchModel>();
            
            // maybe for future
            serviceCollection.AddTransient<PaymentForm>();
            serviceCollection.AddTransient<Checkout>();
            serviceCollection.AddTransient<MembershipForm>();
            serviceCollection.AddTransient<MembershipSignUpForm>();
            serviceCollection.AddTransient<Membership3>();
            serviceCollection.AddTransient<RedeemForm>();
            return serviceCollection.BuildServiceProvider();
        }


    }
}
