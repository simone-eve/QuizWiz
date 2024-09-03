using Microsoft.Extensions.Configuration;
using Google.Cloud.Firestore;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.DependencyInjection;

namespace QuizWizAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure Firestore (replace with your approach)
            string projectId = Environment.GetEnvironmentVariable("Firestore:ProjectId"); // Or use Configuration
            var credentials = GoogleCredential.FromFile("path/to/your/serviceAccount.json"); // Replace with your service account path

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Add Firestore service with credentials
            //builder.Services.AddFirestore(db => db.EnablePersistence = true) // Optional: Enable persistence
            //    .AddCredential(credentials);

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}