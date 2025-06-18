using AutoMapper;
using FluentValidation;
using MagicVilla_CouponAPI.Data;
using MagicVilla_CouponAPI.Models;
using MagicVilla_CouponAPI.Models.DTO;
using MagicVilla_CouponAPI.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Service-Registrierung
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ICouponRepository, CouponRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ------------------- Coupon Endpunkte -------------------

// Alle Coupons abrufen
app.MapGet("/api/coupon", async (ILogger<Program> _logger, ICouponRepository _repo) =>
{
    APIResponse response = new();
    _logger.LogInformation("GET /api/coupon wurde aufgerufen.");

    var coupons = await _repo.GetAllAsync();
    response.Result = new
    {
        Coupons = coupons,
        ServerTime = DateTime.UtcNow
    };
    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.OK;

    return Results.Ok(response);
})
.WithName("GetCoupons")
.Produces<APIResponse>(StatusCodes.Status200OK);

// Einzelnen Coupon abrufen
app.MapGet("/api/coupon/{id:int}", async (ILogger<Program> _logger, int id, ICouponRepository _repo, IMapper _mapper) =>
{
    APIResponse response = new();
    var coupon = await _repo.GetByIdAsync(id);

    if (coupon == null)
    {
        _logger.LogWarning("Coupon mit Id {Id} nicht gefunden.", id);
        response.IsSuccess = false;
        response.StatusCode = HttpStatusCode.NotFound;
        response.ErrorMessages.Add($"Kein Coupon mit der Id {id} gefunden.");
        return Results.NotFound(response);
    }

    response.Result = _mapper.Map<CouponDTO>(coupon);
    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.OK;
    return Results.Ok(response);
})
.WithName("GetCoupon")
.Produces<APIResponse>(StatusCodes.Status200OK)
.Produces<APIResponse>(StatusCodes.Status404NotFound);

// Coupon anlegen
app.MapPost("/api/coupon",
    async (
        IMapper _mapper,
        IValidator<CouponCreateDTO> _validation,
        ICouponRepository _repo,
        [FromBody] CouponCreateDTO coupon_C_DTO) =>
    {
        APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

        var validationResult = await _validation.ValidateAsync(coupon_C_DTO);
        if (!validationResult.IsValid)
        {
            response.ErrorMessages = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return Results.BadRequest(response);
        }

        if (await _repo.ExistsByNameAsync(coupon_C_DTO.Name))
        {
            response.ErrorMessages.Add("Coupon Name already Exists");
            return Results.BadRequest(response);
        }

        Coupon coupon = _mapper.Map<Coupon>(coupon_C_DTO);
        coupon.Created = DateTime.UtcNow;
        coupon.LastUpdated = DateTime.UtcNow;
        await _repo.AddAsync(coupon);

        CouponDTO couponDTO = _mapper.Map<CouponDTO>(coupon);

        response.Result = couponDTO;
        response.IsSuccess = true;
        response.StatusCode = HttpStatusCode.Created;

        return Results.CreatedAtRoute("GetCoupon", new { id = coupon.Id }, response);
    }
)
.WithName("CreateCoupon")
.Accepts<CouponCreateDTO>("application/json")
.Produces<APIResponse>(StatusCodes.Status201Created)
.Produces<APIResponse>(StatusCodes.Status400BadRequest);

// Coupon aktualisieren
app.MapPut("/api/coupon", async (
    IMapper _mapper,
    IValidator<CouponUpdateDTO> _validation,
    ICouponRepository _repo,
    [FromBody] CouponUpdateDTO coupon_U_DTO) =>
{
    APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

    var validationResult = await _validation.ValidateAsync(coupon_U_DTO);
    if (!validationResult.IsValid)
    {
        response.ErrorMessages = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
        return Results.BadRequest(response);
    }

    var couponFromDb = await _repo.GetByIdAsync(coupon_U_DTO.Id);
    if (couponFromDb == null)
    {
        response.StatusCode = HttpStatusCode.NotFound;
        response.ErrorMessages.Add($"Kein Coupon mit der Id {coupon_U_DTO.Id} gefunden.");
        return Results.NotFound(response);
    }

    couponFromDb.IsActive = coupon_U_DTO.IsActive;
    couponFromDb.Name = coupon_U_DTO.Name;
    couponFromDb.Percent = coupon_U_DTO.Percent;
    couponFromDb.LastUpdated = DateTime.UtcNow;

    await _repo.UpdateAsync(couponFromDb);

    response.Result = _mapper.Map<CouponDTO>(couponFromDb);
    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.OK;
    return Results.Ok(response);
})
.WithName("UpdateCoupon")
.Accepts<CouponUpdateDTO>("application/json")
.Produces<APIResponse>(StatusCodes.Status200OK)
.Produces<APIResponse>(StatusCodes.Status400BadRequest)
.Produces<APIResponse>(StatusCodes.Status404NotFound);

// Coupon löschen
app.MapDelete("/api/coupon/{id:int}", async (int id, ICouponRepository _repo) =>
{
    APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.NotFound };

    var deleted = await _repo.DeleteAsync(id);
    if (!deleted)
    {
        response.ErrorMessages.Add("Ungültige Id: Coupon nicht gefunden.");
        return Results.NotFound(response);
    }

    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.NoContent;
    return Results.Ok(response);
})
.WithName("DeleteCoupon")
.Produces<APIResponse>(StatusCodes.Status200OK)
.Produces<APIResponse>(StatusCodes.Status404NotFound);

// --------------------------------------------------------

app.UseHttpsRedirection();
app.Run();

