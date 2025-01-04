using System.Diagnostics;
using Application.Contracts.Persistence.IRepository;
using Application.Features.Product.Queries.GetListProducts;
using Application.Features.Product.Queries.GetListProducts2;
using Application.Services.JWTAuthetication;
using Application.Services.Serilog;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebApi.Controllers.Base;

namespace WebApi.Controllers.V1;

[ApiVersion("1")]
public class ProductController : BaseController
{
    private readonly IJwtAuthenticatedService _iwJwtAuthenticatedService;
    private readonly ISerilogService _logger;
    private readonly IProductRepository iProductRepository;

    public ProductController(IJwtAuthenticatedService iwJwtAuthenticatedService, ISerilogService logger,
        IProductRepository _iProductRepository)
    {
        _iwJwtAuthenticatedService = iwJwtAuthenticatedService;
        _logger = logger;
        iProductRepository = _iProductRepository;
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> GetListProducts(GetListProductsQuerie command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }


    [HttpPost("[action]")]
    public async Task<IActionResult> GetListProducts2(GetListProducts2Querie command)
    {
        //try
        //{
        //    int.Parse("a");
        //}
        //catch (Exception ex)
        //{
        //    _logger.LogSystem(ex);
        //}

        var result = await Mediator.Send(command);
        return Ok(result);
    }


    [HttpPost("[action]")]
    public async Task<IActionResult> MultipleResults()
    {
        var result = await iProductRepository.MultipleResults();
        return Ok(result);
    }


    [HttpPost("[action]")]
    public async Task<IActionResult> BulkInsert()
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        await iProductRepository.BulkInsertAsync();

        stopwatch.Stop();
        var ts = stopwatch.Elapsed;

        return Ok("Seconds: " + ts.Seconds);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Insert()
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        await iProductRepository.InsertAsync();

        stopwatch.Stop();
        var ts = stopwatch.Elapsed;

        return Ok("Seconds: " + ts.Seconds);
    }


    [HttpPost("[action]")]
    public async Task<IActionResult> WorkManager([FromBody] List<Root> data)
    {
        var ids = new List<Guid>();

        foreach (var root in data)
        {
            var result = await iProductRepository.WorkManager(JsonConvert.SerializeObject(root));
            ids.Add(result);
        }

        return Ok(ids.Count == data.Count);
    }


    [HttpGet("[action]")]
    public async Task<IActionResult> WorkManager2(string data)
    {
        var result = await iProductRepository.WorkManager(data);
        return Ok(result);
    }


    [HttpPost("[action]")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        try
        {
            if (file != null && file.Length > 0)
            {
                var newFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var uploadsFolder =
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", newFileName);

                using (var stream = new FileStream(uploadsFolder, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return Ok($"https://ef2.noyankesht.ir/Uploads/{newFileName}");
            }

            return Ok(null);
        }
        catch (Exception e)
        {
            return Ok(null);
        }
    }


    public class Root
    {
        public DateTime id { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public DateTime timestamp { get; set; }
    }
}