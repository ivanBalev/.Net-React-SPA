using Microsoft.AspNetCore.Mvc;
using MiniValidation;

public static class WebApplicationHouseExtensions
{
    public static void MapHouseEndpoints(this WebApplication app)
    {
        app.MapGet("/houses", async (IHouseRepository repo) => await repo.GetAll())
        .Produces<HouseDto[]>(StatusCodes.Status200OK);

        app.MapGet("/house/{houseId:int}", async (int houseId, IHouseRepository repo) =>
        {
            var house = await repo.Get(houseId);
            if (house == null)
            {
                return Results.Problem($"House with ID {houseId} not found.",
                statusCode: 404);
            }
            return Results.Ok(house);
        })
        // Swagger docs additional info
        .ProducesProblem(404)
        .Produces<HouseDetailDto>(StatusCodes.Status200OK);

        app.MapPost("/houses", async ([FromBody] HouseDetailDto dto, IHouseRepository repo) =>
        {
            if (!MiniValidator.TryValidate(dto, out var errors))
                // pretty elegant
                return Results.ValidationProblem(errors);

            var newHouse = await repo.Add(dto);
            return Results.Created($"/house/{newHouse.Id}", newHouse);
        })
        .ProducesValidationProblem()
        .Produces<HouseDetailDto>(StatusCodes.Status201Created);

        app.MapPut("/houses", async ([FromBody] HouseDetailDto dto, IHouseRepository repo) =>
        {
            if (!MiniValidator.TryValidate(dto, out var errors))
                // pretty elegant
                return Results.ValidationProblem(errors);


            if (await repo.Get(dto.Id) == null)
                return Results.Problem($"House {dto.Id} not found",
                statusCode: 404);

            var updatedHouse = await repo.Update(dto);
            return Results.Ok(updatedHouse);
        })
        .ProducesProblem(404)
        .ProducesValidationProblem()
        .Produces<HouseDetailDto>(StatusCodes.Status200OK);

        app.MapDelete("/houses/{houseId:int}", async (int houseId, IHouseRepository repo) =>
        {
            if (await repo.Get(houseId) == null)
                return Results.Problem($"House {houseId} not found",
                statusCode: 404);
            await repo.Delete(houseId);
            return Results.Ok();
        })
        .ProducesProblem(404)
        .Produces(StatusCodes.Status200OK);

        app.MapGet("house/{houseId:int}/bids", async (int houseId, IHouseRepository houseRepo, IBidRepository bidRepo) =>
        {
            if (await houseRepo.Get(houseId) == null)
                return Results.Problem($"House {houseId} not found",
                statusCode: 404);
            var bids = await bidRepo.Get(houseId);
            return Results.Ok(bids);
        })
        .ProducesProblem(404)
        .Produces<BidDto>(StatusCodes.Status200OK);
    }
}