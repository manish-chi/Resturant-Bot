// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.15.2

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Azure.Blobs;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RestroQnABot.Bots;
using RestroQnABot.Dialogs;
using RestroQnABot.Middlewares;
using RestroQnABot.Models;
using RestroQnABot.Utlities;

namespace RestroQnABot
{
    public class Startup
    {
        public Startup(IConfiguration configuration,IWebHostEnvironment env) {

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: false)
                .AddEnvironmentVariables();
           
            
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient().AddControllers().AddNewtonsoftJson();

            services.Configure<AppSettings>(Configuration.GetSection("ApplicationSettings"));

            //services.AddSingleton<AppSettings>();
            // Create the Bot Framework Authentication to be used with the Bot Adapter.
            services.AddSingleton<BotFrameworkAuthentication, ConfigurationBotFrameworkAuthentication>();

            // Create the Bot Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            // Create the storage we'll be using for User and Conversation state.
            // (Memory is great for testing purposes - examples of implementing storage with
            // Azure Blob Storage or Cosmos DB are below).
            //var storage = new MemoryStorage();

            /* AZURE BLOB STORAGE - Uncomment the code in this section to use Azure blob storage */

             var storage = new BlobsStorage(Configuration["StorageConnectionString"], "bot-state");

            /* END AZURE BLOB STORAGE */

            /* COSMOSDB STORAGE - Uncomment the code in this section to use CosmosDB storage */

            // var cosmosDbStorageOptions = new CosmosDbPartitionedStorageOptions()
            // {
            //     CosmosDbEndpoint = "<endpoint-for-your-cosmosdb-instance>",
            //     AuthKey = "<your-cosmosdb-auth-key>",
            //     DatabaseId = "<your-database-id>",
            //     ContainerId = "<cosmosdb-container-id>"
            // };
            // var storage = new CosmosDbPartitionedStorage(cosmosDbStorageOptions);

            /* END COSMOSDB STORAGE */

            // Create the User state passing in the storage layer.
            var userState = new UserState(storage);
            services.AddSingleton(userState);

            // Create the Conversation state passing in the storage layer.
            var conversationState = new ConversationState(storage);
            services.AddSingleton(conversationState);

            services.AddSingleton<InputTypeMiddleWare>();

            services.AddSingleton<TranslationMiddleWare>();
         
            services.AddSingleton(new TranslationManager(new AzureTranslationClient(Configuration)));

            services.AddSingleton<IntermediateDialog>();
            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            services.AddTransient<IBot, RestroBot<IntermediateDialog>>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseWebSockets()
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });

            // app.UseHttpsRedirection();
        }
    }
}
