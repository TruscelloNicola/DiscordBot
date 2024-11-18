using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Discord.WebSocket;

namespace APIIntegratedBot.Api
{
   
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson();
			services.AddCors(options =>
			{
				options.AddPolicy("AllowLocalhost", builder =>
				{
					builder.WithOrigins("*")
                           .AllowAnyHeader()
						   .AllowAnyMethod();
				});
			});
		}

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseCors("AllowLocalhost");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


        }
    }

}
