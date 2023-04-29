﻿using HKCR.Application.Common.DTO.Document;
using HKCR.Application.Common.DTO.Rental;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKCR.Application.Common.Interface
{
    public interface IRentalDetails
    {
        Task<List<RentalResponseDto>> GetAllRentalAsync();
        Task<List<RentalResponseDto>> GetAllRental();
        Task<RentalResponseDto> AddRentalDetails(RentalRequestDto rental);
    }
}