using USca_Server.Alarms;
using USca_Server.Measures;
using USca_Server.Shared;
using USca_Server.Tags;
using USca_Server.Users;

namespace USca_Server
{
    public class Program
	{
		public static void Main(string[] args)
		{
			//CryptoUtil.ImportPublicKey("C:/Users/aaa/Desktop/USca_RTU_Key.pub", "USca_RTU_Key");

			var builder = WebApplication.CreateBuilder(args);
			builder.Services.AddControllers();

			builder.Services.AddScoped(typeof(IUserService), typeof(UserService));
			builder.Services.AddScoped(typeof(IMeasureService), typeof(MeasureService));
            builder.Services.AddScoped(typeof(ITagService), typeof(TagService));
            builder.Services.AddScoped(typeof(IAlarmService), typeof(AlarmService));
            builder.Services.AddDbContext<ServerDbContext>();
            builder.Services.AddControllers().AddJsonOptions(options =>options.JsonSerializerOptions.PropertyNamingPolicy = null);

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
			app.UseWebSockets();
			app.MapControllers();
			app.Run();
		}
	}
}