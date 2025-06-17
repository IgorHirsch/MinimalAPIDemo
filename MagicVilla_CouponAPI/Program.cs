using MagicVilla_CouponAPI.Data;
using MagicVilla_CouponAPI.Models;
using MagicVilla_CouponAPI.Models.DTO;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Gibt alle Coupons mit Metadaten zurück
app.MapGet("/api/coupon", (ILogger<Program> logger) =>
{
    logger.LogInformation("GET /api/coupon wurde aufgerufen.");
    var coupons = CouponStore.couponList;
    var response = new
    {
        Count = coupons.Count,
        Coupons = coupons,
        ServerTime = DateTime.UtcNow
    };
    return Results.Ok(response);
})
.WithName("GetCoupons")
.Produces(StatusCodes.Status200OK);







// Gibt einen Coupon anhand der Id zurück
app.MapGet("/api/coupon/{id:int}", (int id) =>
{
    var coupon = CouponStore.couponList.FirstOrDefault(c => c.Id == id);
    if (coupon == null)
    {
        return Results.NotFound($"Kein Coupon mit der Id {id} gefunden.");
    }
    return Results.Ok(coupon);
})
.WithName("GetCouponById")
.Produces<Coupon>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);







// Neuen Coupon anlegen
app.MapPost("/api/coupon", ([FromBody] CouponCreateDTO couponCreateDto) =>
{
    if (string.IsNullOrWhiteSpace(couponCreateDto.Name))
        return Results.BadRequest("Ungültiger Coupon-Name.");

    if (CouponStore.couponList.Any(u => u.Name.Equals(couponCreateDto.Name, StringComparison.OrdinalIgnoreCase)))
        return Results.BadRequest("Coupon-Name existiert bereits.");

    var coupon = new Coupon
    {
        Id = (CouponStore.couponList.Max(u => (int?)u.Id) ?? 0) + 1,
        IsActive = couponCreateDto.IsActive,
        Name = couponCreateDto.Name,
        Percent = couponCreateDto.Percent,
        Created = DateTime.UtcNow,
        LastUpdated = DateTime.UtcNow
    };


    CouponStore.couponList.Add(coupon);


    // DTO zurückgeben
    var resultDto = new CouponDTO
    {
        Id = coupon.Id,
        Name = coupon.Name,
        Percent = coupon.Percent,
        IsActive = coupon.IsActive,
        Created = coupon.Created
    };

    return Results.CreatedAtRoute("GetCouponById", new { id = coupon.Id }, resultDto);
})
.WithName("CreateCoupon")
.Accepts<CouponCreateDTO>("application/json")
.Produces<CouponDTO>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status400BadRequest);








// Coupon aktualisieren
app.MapPut("/api/coupon/{id:int}", (int id, Coupon updatedCoupon) =>
{
    var coupon = CouponStore.couponList.FirstOrDefault(c => c.Id == id);
    if (coupon == null)
    {
        return Results.NotFound($"Kein Coupon mit der Id {id} gefunden.");
    }

    if (updatedCoupon == null || string.IsNullOrWhiteSpace(updatedCoupon.Name))
    {
        return Results.BadRequest("Ungültige Coupon-Daten.");
    }

    coupon.Name = updatedCoupon.Name;
    coupon.Percent = updatedCoupon.Percent;
    coupon.IsActive = updatedCoupon.IsActive;
    coupon.LastUpdated = DateTime.UtcNow;
    return Results.Ok(coupon);
})
.WithName("UpdateCoupon")
.Produces<Coupon>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status404NotFound);







// Coupon löschen
app.MapDelete("/api/coupon/{id:int}", (int id) =>
{
    var coupon = CouponStore.couponList.FirstOrDefault(c => c.Id == id);
    if (coupon == null)
    {
        return Results.NotFound($"Kein Coupon mit der Id {id} gefunden.");
    }
    CouponStore.couponList.Remove(coupon);
    return Results.NoContent();
})
.WithName("DeleteCoupon")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound);

app.UseHttpsRedirection();

app.Run();

