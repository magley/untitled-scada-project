using Microsoft.EntityFrameworkCore;
using USca_DbManager.User;

namespace USca_DbManager
{
    public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);
			builder.Services.AddControllers();

			builder.Services.AddScoped(typeof(IUserService), typeof(UserService));
			builder.Services.AddDbContext<UserContext>(opt => opt.UseSqlite("Data Source=Data/app.db"));

			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();
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