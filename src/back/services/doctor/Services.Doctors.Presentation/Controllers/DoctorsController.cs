﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Services.Doctors.Presentation.Controllers.Base;
using Microsoft.AspNetCore.Http;
using Services.Doctors.Application.UseCases;
using System.ComponentModel.DataAnnotations;
using Shared.Common.Helper.ErrorsHandler;
using Shared.Common.Helper.Extensions;

namespace Services.Doctors.Presentation.Controllers;

[ApiController]
[Route("doctors")]
[Produces("application/json")]
public sealed class DoctorsController : BaseController
{
    public DoctorsController(ISender sender) : base(sender)
    {
    }

    #region Queries

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(DoctorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById([FromRoute, Required] Guid id, CancellationToken cancellationToken)
    {
        GetDoctorByIdQuery query = new(id);
        Result<DoctorResponse> response = await _sender.Send(query, cancellationToken);

        return response.Match(Ok, HandleErrorResults);
    }
    
    [HttpGet("by-name")]
    [ProducesResponseType(typeof(IEnumerable<DoctorResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCollectionByName([FromQuery, Required] string name, [FromQuery, Required] int pageNumber, CancellationToken cancellationToken)
    {
        GetDoctorCollectionByNameQuery query = new(name, pageNumber);
        Result<IEnumerable<DoctorResponse>> response = await _sender.Send(query, cancellationToken);

        return response.Match(Ok, HandleErrorResults);
    }
    
    [HttpGet("by-specialty")]
    [ProducesResponseType(typeof(IEnumerable<DoctorResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetDoctorCollectionBySpecialty([FromQuery, Required] string specialty, [FromQuery, Required] int pageNumber, CancellationToken cancellationToken)
    {
        GetDoctorCollectionBySpecialtyQuery query = new(specialty, pageNumber);
        Result<IEnumerable<DoctorResponse>> response = await _sender.Send(query, cancellationToken);

        return response.Match(Ok, HandleErrorResults);
    }
    
    [HttpGet("specialties")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSpecialtyCollection(CancellationToken cancellationToken)
    {
        GetSpecialtyCollectionQuery query = new();
        Result<IEnumerable<string>> response = await _sender.Send(query, cancellationToken);

        return response.Match(Ok, HandleErrorResults);
    }

    #endregion

    #region Commands

    [HttpPost]
    [ProducesResponseType(typeof(DoctorResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateDoctorCommand command, CancellationToken cancellationToken)
    {
        Result<DoctorResponse> response = await _sender.Send(command, cancellationToken);

        return response.Match(
                success: data => Created(string.Empty, data),
                error: HandleErrorResults);
    }

    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(
        [FromRoute, Required] Guid id,
        [FromHeader, Required] Guid credentialId,
        [FromBody] UpdateDoctorRequest request, CancellationToken cancellationToken)
    {
        UpdateDoctorCommand command = new(id, credentialId, request);
        Result<DoctorResponse> response = await _sender.Send(command, cancellationToken);

        return response.Match(Ok, HandleErrorResults);
    }
    
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Remove(
        [FromRoute, Required] Guid id, 
        [FromHeader, Required] Guid credentialId, 
        CancellationToken cancellationToken)
    {
        RemoveDoctorCommand command = new(id, credentialId);
        Result response = await _sender.Send(command, cancellationToken);

        return response.Match(Ok, HandleErrorResults);
    }

    #endregion
}