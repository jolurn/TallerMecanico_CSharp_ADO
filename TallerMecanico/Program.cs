using Microsoft.EntityFrameworkCore;
using TallerMecanico.Data;
using TallerMecanico.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 👇 NUEVO: Registrar el DbContext (El ORM)
builder.Services.AddDbContext<TallerMecanicoContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ClienteRepository>();
builder.Services.AddScoped<VehiculoRepository>();
builder.Services.AddScoped<MecanicoRepository>();
builder.Services.AddScoped<RepuestoRepository>();
builder.Services.AddScoped<OrdenServicioRepository>();

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