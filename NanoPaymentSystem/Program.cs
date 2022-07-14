using NanoPaymentSystem.Application;
using NanoPaymentSystem.Database;
using NanoPaymentSystem.MessageBroker;
using NanoPaymentSystem.PaymentProviders;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddFakePaymentProvider();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddMessageBroker();
builder.Services.AddControllers();
builder.Services.AddApplication();

var app = builder.Build();

app.UseSwagger();
app.UseHttpsRedirection();
app.UseRouting();
app.UseEndpoints(endpoints => endpoints.MapControllers());
app.UseSwaggerUI();
app.Run();
