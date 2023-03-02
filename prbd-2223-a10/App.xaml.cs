using System.Windows;
using MyPoll.Model;
using MyPoll.ViewModel;
using PRBD_Framework;

namespace MyPoll; 

public partial class App : ApplicationBase<User,MyPollContext> {
    protected override void OnStartup(StartupEventArgs e) {
        PrepareDatabase();
    }

    private static void PrepareDatabase() {
        // Clear database and seed data
        Context.Database.EnsureDeleted();
        Context.Database.EnsureCreated();

        // Cold start
        Console.Write("Cold starting database... ");
        Context.Users.Find(0);
        Console.WriteLine("done");
    }
}
