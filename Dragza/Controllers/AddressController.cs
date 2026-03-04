using Dragza.Application.Interface;
using Dragza.Domain.DTO;
using Dragza.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class AddressController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public AddressController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // GET: api/Address
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var addresses = await _unitOfWork.AddressRepository.GetAllAsync();
        var dto = addresses.Select(a => new AddressResponseDto
        {
            Id = a.Id,
            UserId = a.UserId,
            AddressLine = a.AddressLine
        }).ToList();

        return Ok(dto);
    }

    // GET: api/Address/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var address = await _unitOfWork.AddressRepository.GetByIdAsync(id);
        if (address == null) return NotFound();

        var dto = new AddressResponseDto
        {
            Id = address.Id,
            UserId = address.UserId,
            AddressLine = address.AddressLine
        };
        return Ok(dto);
    }

    // POST: api/Address
    [HttpPost]
    public async Task<IActionResult> Create(CreateAddressDto createDto)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(createDto.UserId);
        if (user == null) return BadRequest("User not found");

        var address = new Address
        {
            Id = Guid.NewGuid(),
            UserId = createDto.UserId,
            AddressLine = createDto.AddressLine
        };

        await _unitOfWork.AddressRepository.AddAsync(address);
        await _unitOfWork.SaveChangesAsync();

        return Ok(new AddressResponseDto
        {
            Id = address.Id,
            UserId = address.UserId,
            AddressLine = address.AddressLine
        });
    }

    // PUT: api/Address/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateAddressDto updateDto)
    {
        var address = await _unitOfWork.AddressRepository.GetByIdAsync(id);
        if (address == null) return NotFound();

        address.AddressLine = updateDto.AddressLine;
        _unitOfWork.AddressRepository.Update(address);
        await _unitOfWork.SaveChangesAsync();

        return Ok(new AddressResponseDto
        {
            Id = address.Id,
            UserId = address.UserId,
            AddressLine = address.AddressLine
        });
    }

    // DELETE: api/Address/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var address = await _unitOfWork.AddressRepository.GetByIdAsync(id);
        if (address == null) return NotFound();

        _unitOfWork.AddressRepository.Delete(address);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }
}