using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers;
 
public class SlidesController : BaseApiController
{
    private readonly IUnitOfWork _unitOfWork;

    public SlidesController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var slides = await _unitOfWork.UserPhotoMessageRepository.GetAllAsync();

        return Ok(slides);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var slides = await _unitOfWork.UserPhotoMessageRepository.DeleteAsync(id);

        return Ok(slides);
    }

}
