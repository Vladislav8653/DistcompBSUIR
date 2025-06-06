using System.Text.Json.Serialization;
using Publisher.Extensions;
using Publisher.Infrastructure.Mapper;
using Publisher.Infrastructure.Validators;
using Publisher.Middleware;
using FluentValidation;
using Messaging;
using Messaging.Extensions;
using Publisher.Consumers;
using Publisher.DTO.RequestDTO;
using Publisher.DTO.ResponseDTO;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddValidatorsFromAssemblyContaining<EditorRequestDTOValidator>();

builder.Services.AddRepositories();
builder.Services.AddServices();
builder.Services.AddDiscussionClient();
builder.Services.AddDbContext(builder.Configuration)
    .AddKafkaMessageBus()
    .AddKafkaProducer<string, KafkaMessage<PostRequestDTO>>(options =>
    {
        options.Topic = "InTopic";
        options.BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BROKER");
        options.AllowAutoCreateTopics = true;
    })
    .AddKafkaConsumer<string, KafkaMessage<PostResponseDTO>, OutTopicHandler>(options =>
    {
        options.Topic = "OutTopic";
        options.AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Earliest;
        options.EnableAutoOffsetStore = false;
        options.GroupId = "posts-group";
        options.BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BROKER");
        options.AllowAutoCreateTopics = true;
    })
    .AddStackExchangeRedisCache(options =>
    {
        options.Configuration = Environment.GetEnvironmentVariable("REDIS_HOST");
        options.InstanceName = "Publisher";
    });

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTheme(ScalarTheme.DeepSpace);
    });
}

app.MapControllers();
app.ApplyMigrations(app.Services);
app.Run();