using AutoMapper;
using FluentValidation;
using MagicVilla_CouponAPI.Data;
using MagicVilla_CouponAPI.Models;
using MagicVilla_CouponAPI.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingProfile)); // <--- AutoMapper hinzufügen
builder.Services.AddValidatorsFromAssemblyContaining<Program>(); // <--- FluentValidation hinzufügen

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Gibt alle Coupons mit Metadaten zurück
app.MapGet("/api/coupon", (ILogger<Program> _logger) =>
{
    APIResponse response = new();
    _logger.LogInformation("GET /api/coupon wurde aufgerufen.");

    // Metadaten und Coupon-Liste als Objekt im Result
    response.Result = new
    {
        Coupons = CouponStore.couponList,
        ServerTime = DateTime.UtcNow
    };
    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.OK;

    return Results.Ok(response);
})
.WithName("GetCoupons")
.Produces<APIResponse>(StatusCodes.Status200OK);







// Gibt einen Coupon anhand der Id zurück
app.MapGet("/api/coupon/{id:int}", (ILogger<Program> _logger, int id) =>
{
    APIResponse response = new();
    var coupon = CouponStore.couponList.FirstOrDefault(u => u.Id == id);

    if (coupon == null)
    {
        _logger.LogWarning("Coupon mit Id {Id} nicht gefunden.", id);
        response.IsSuccess = false;
        response.StatusCode = HttpStatusCode.NotFound;
        response.ErrorMessages.Add($"Kein Coupon mit der Id {id} gefunden.");
        return Results.NotFound(response);
    }

    response.Result = coupon;
    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.OK;
    return Results.Ok(response);
})
.WithName("GetCoupon")
.Produces<APIResponse>(StatusCodes.Status200OK)
.Produces<APIResponse>(StatusCodes.Status404NotFound);







// Neuen Coupon anlegen
app.MapPost("/api/coupon", async (
    IMapper _mapper,
    IValidator<CouponCreateDTO> _validation,
    [FromBody] CouponCreateDTO coupon_C_DTO) =>
{
    APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

    // Validierung mit FluentValidation
    var validationResult = await _validation.ValidateAsync(coupon_C_DTO);
    if (!validationResult.IsValid)
    {
        response.ErrorMessages = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
        return Results.BadRequest(response);
    }

    // Name auf Eindeutigkeit prüfen
    if (CouponStore.couponList.Any(u => u.Name.Equals(coupon_C_DTO.Name, StringComparison.OrdinalIgnoreCase)))
    {
        response.ErrorMessages.Add("Coupon Name already Exists");
        return Results.BadRequest(response);
    }

    // Mapping und Speicherung
    Coupon coupon = _mapper.Map<Coupon>(coupon_C_DTO);
    coupon.Id = (CouponStore.couponList.Max(u => (int?)u.Id) ?? 0) + 1;
    coupon.Created = DateTime.UtcNow;
    coupon.LastUpdated = DateTime.UtcNow;
    CouponStore.couponList.Add(coupon);

    CouponDTO couponDTO = _mapper.Map<CouponDTO>(coupon);

    response.Result = couponDTO;
    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.Created;

    // REST-konform: CreatedAtRoute mit APIResponse
    return Results.CreatedAtRoute("GetCoupon", new { id = coupon.Id }, response);
})
.WithName("CreateCoupon")
.Accepts<CouponCreateDTO>("application/json")
.Produces<APIResponse>(StatusCodes.Status201Created)
.Produces<APIResponse>(StatusCodes.Status400BadRequest);








// Coupon aktualisieren
app.MapPut("/api/coupon", async (
    IMapper _mapper,
    IValidator<CouponUpdateDTO> _validation,
    [FromBody] CouponUpdateDTO coupon_U_DTO) =>
{
    APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

    // Validierung mit FluentValidation
    var validationResult = await _validation.ValidateAsync(coupon_U_DTO);
    if (!validationResult.IsValid)
    {
        response.ErrorMessages = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
        return Results.BadRequest(response);
    }

    // Coupon aus Store suchen
    var couponFromStore = CouponStore.couponList.FirstOrDefault(u => u.Id == coupon_U_DTO.Id);
    if (couponFromStore == null)
    {
        response.StatusCode = HttpStatusCode.NotFound;
        response.ErrorMessages.Add($"Kein Coupon mit der Id {coupon_U_DTO.Id} gefunden.");
        return Results.NotFound(response);
    }

    // Werte aktualisieren
    couponFromStore.IsActive = coupon_U_DTO.IsActive;
    couponFromStore.Name = coupon_U_DTO.Name;
    couponFromStore.Percent = coupon_U_DTO.Percent;
    couponFromStore.LastUpdated = DateTime.UtcNow;

    response.Result = _mapper.Map<CouponDTO>(couponFromStore);
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
app.MapDelete("/api/coupon/{id:int}", (int id) =>
{
    APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.NotFound };

    var couponFromStore = CouponStore.couponList.FirstOrDefault(u => u.Id == id);
    if (couponFromStore == null)
    {
        response.ErrorMessages.Add("Ungültige Id: Coupon nicht gefunden.");
        return Results.NotFound(response);
    }

    CouponStore.couponList.Remove(couponFromStore);
    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.NoContent;
    return Results.Ok(response);
})
.WithName("DeleteCoupon")
.Produces<APIResponse>(StatusCodes.Status200OK)
.Produces<APIResponse>(StatusCodes.Status404NotFound);

app.UseHttpsRedirection();

app.Run();

