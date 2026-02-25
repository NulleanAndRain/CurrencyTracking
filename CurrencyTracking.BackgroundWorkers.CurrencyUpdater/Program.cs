using CurrencyTracking.BackgroundWorkers.CurrencyUpdater.Schedulers;

var builder = WebApplication.CreateBuilder(args);

CurrencyUpdaterScheduler.Start().Wait();



// TODO
// jobs DI
// unit tests
// docker infra
// keycloak config
// serilog config